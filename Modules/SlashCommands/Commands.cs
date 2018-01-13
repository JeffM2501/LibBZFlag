using BZFlag.Game.Host.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZFS.SlashCommands
{
    public static class Commands
    {
        public delegate void CommandProcessorCallback(string command, string arguments, ServerPlayer caller);

        private static Dictionary<string, CommandProcessorCallback> CommandHandlers = new Dictionary<string, CommandProcessorCallback>();

        public static bool RegisterHandler(string name, CommandProcessorCallback handler)
        {
            string cmd = name.ToUpperInvariant();

            lock (CommandHandlers)
            {
                if (CommandHandlers.ContainsKey(cmd))
                    return false;

                CommandHandlers.Add(cmd, handler);
                return true;
            }
        }

        public static bool RemoveHandler(string name, CommandProcessorCallback handler)
        {
            string cmd = name.ToUpperInvariant();

            lock (CommandHandlers)
            {
                if (!CommandHandlers.ContainsKey(cmd) || CommandHandlers[cmd] != handler)
                    return false;

                CommandHandlers.Remove(cmd);
                return true;
            }
        }

        internal static bool CallHandler(string name, string body, ServerPlayer caller)
        {
            string cmd = name.ToUpperInvariant();

            CommandProcessorCallback callback = null;

            lock(CommandHandlers)
            {
                if (CommandHandlers.ContainsKey(cmd))
                    callback = CommandHandlers[cmd];
            }

            if (callback == null)
                return false;

            callback(cmd, body, caller);
            return true;
        }
    }
}
