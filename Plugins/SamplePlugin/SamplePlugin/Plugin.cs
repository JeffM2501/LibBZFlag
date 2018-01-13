using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

using BZFlag.Data.Teams;
using BZFlag.Game.Host;
using BZFlag.Game.Host.Players;

namespace SamplePlugin
{
    public class Plugin : BZFlag.Game.Host.API.PlugIn
    {
        /// <summary>
        /// The Display Name
        /// </summary>
        public string Name => "SamplePlugin";

        /// <summary>
        /// The public description
        /// </summary>
        public string Description => "Just shows how to use the API";

        /// <summary>
        /// Cached server for use later
        /// </summary>
        protected Server ServerInstance = null;

        protected Thread BackgroundUpdateThread = null; 

        /// <summary>
        /// Called for each server instance that loads the plugin
        /// </summary>
        /// <param name="serverInstance">The server that the plugin is attached to</param>
        public void Startup(Server serverInstance)
        {
            ServerInstance = serverInstance;

            ServerInstance.PlayerAccepted += ServerInstance_PlayerAccepted;
            ServerInstance.PlayerBanned += ServerInstance_PlayerBanned;

            ServerInstance.SetTeamSelector(PickTeam);

            Logger.LineLogged += Logger_LineLogged;

            BackgroundUpdateThread = new Thread(new ThreadStart(DoBackGroundChecks));
            BackgroundUpdateThread.Start();
        }

        /// <summary>
        /// called when a plugin is unloaded or a server instance is shutdown
        /// </summary>
        public void Shutdown()
        {
            BackgroundUpdateThread.Abort();
        }

   
        protected void DoBackGroundChecks()
        {
            while (true)
            {
                Thread.Sleep(10000);

                StringBuilder payload = new StringBuilder();

                for (TeamColors team = TeamColors.RogueTeam; team <= TeamColors.ObserverTeam; team++ )
                    payload.AppendLine(team.ToString() + ":" + ServerInstance.State.Players.GetTeamPlayerCount(team).ToString());

                WebClient web = new WebClient();

                web.UploadData("http://your.logingurl.com", Encoding.ASCII.GetBytes(payload.ToString()));
            }
        }

        /// <summary>
        /// Custom Team Picker
        /// </summary>
        /// <param name="player">The player who needs a team</param>
        /// <returns>The team to assign to the player, NoTeam means kick the player.</returns>
        public TeamColors PickTeam(ServerPlayer player)
        {
            return ServerInstance.GetLargestTeam(false);
        }

        /// <summary>
        /// Called when a line is logged by the server
        /// </summary>
        /// <param name="e">Data about the line being loged</param>
        private void Logger_LineLogged(object sender, Logger.LogEventArgs e)
        {
            // do custom logging here
        }

        /// <summary>
        /// Called when a player is banned for any reason
        /// </summary>
        /// <param name="sender">Server Instance that did the ban</param>
        /// <param name="e">The player who is banned (contains ban info )</param>
        private void ServerInstance_PlayerBanned(object sender, ServerPlayer e)
        {

        }

        /// <summary>
        /// Called when a player is added
        /// </summary>
        /// <param name="sender">Server Instance that added the player</param>
        /// <param name="e">The player added</param>
        private void ServerInstance_PlayerAccepted(object sender, ServerPlayer e)
        {
        }
    }
}
