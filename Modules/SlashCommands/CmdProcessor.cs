using BZFlag.Game.Host;
using BZFlag.Game.Host.API;
using BZFlag.Game.Host.Players;
using BZFlag.Networking.Messages.BZFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BZFS.SlashCommands
{
    public class CmdProcessor : PlugIn
    {
        public string Name => "CmdProcessor";

        public string Description => "Processes Slash Commands";


        protected class PendingCommand
        {
            public string Text = string.Empty;
            public ServerPlayer Player = null;
        }

        private List<PendingCommand> PendingCommands = new List<PendingCommand>();

        protected Thread Worker = null;

        protected Server Instance = null;

        public void Shutdown(Server serverInstance)
        {
            lock (PendingCommands)
            {
                if (Worker != null)
                {
                    Worker.Abort();
                    Worker = null;
                }
            }
        }

        public void Startup(Server serverInstance)
        {
            Instance = serverInstance;

            Instance.State.Chat.AcceptTextCommand = TextCommandCallback;

            RegisterStandardCommands();
        }

        private bool TextCommandCallback(ServerPlayer player, MsgMessage message)
        {
            if (player == null || message.MessageText == string.Empty || message.To != BZFlag.Data.Players.PlayerConstants.AllPlayersID || message.MessageType == MsgMessage.MessageTypes.ActionMessage)
                return true;

            if (message.MessageText[0] == '/')
            {
                if (!player.Allowances.AllowCommands)
                    return false;

                PendingCommand cmd = new PendingCommand();
                cmd.Text = message.MessageText;
                cmd.Player = player;
                lock (PendingCommands)
                {
                    PendingCommands.Add(cmd);

                    if (Worker == null)
                    {
                        Worker = new Thread(new ThreadStart(ProcessCommands));
                        Worker.Start();
                    }
                }

                return false;
            }

            return true;
        }

        protected PendingCommand PopCommand()
        {
            lock(PendingCommands)
            {
                if (PendingCommands.Count == 0)
                    return null;

                PendingCommand cmd = PendingCommands[0];
                PendingCommands.RemoveAt(0);
                return cmd;
            }
        }

        protected virtual void ProcessCommands()
        {
            PendingCommand cmd = PopCommand();
            while (cmd != null)
            {
                string[] parts = cmd.Text.Split(" ".ToCharArray(), 2);
                string command = parts[0].Substring(1);

                string args = string.Empty;
                if (parts.Length == 2)
                    args = parts[1];

                Commands.CallHandler(command, args, cmd.Player);

                cmd = PopCommand();
            }

            lock (PendingCommands)
                Worker = null;
        }


        void RegisterStandardCommands()
        {
            Commands.RegisterHandler("DATE", DateTimeCommand);
            Commands.RegisterHandler("TIME", DateTimeCommand);
        }

        protected void DateTimeCommand(string command, string arguments, ServerPlayer caller)
        {
            Instance.State.Chat.SendChatToUser(null, caller, DateTime.Now.ToString(), false);
        }
    }
}
