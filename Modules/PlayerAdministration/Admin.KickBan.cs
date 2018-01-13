using BZFlag.Game.Host.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Game.Host;

namespace BZFS.PlayerAdministration
{
    public partial class Admin
    {
        protected void Kick(string command, string arguments, ServerPlayer caller)
        {
            if (!PermissionProcessor.PlayerHasPermision(caller, PermissionNames.Kick))
            {
                State.Chat.SendChatToUser(null, caller, Resources.KickNonAuthMessage, false);
                return;
            }

            string targetCallsign = string.Empty;
            ServerPlayer target = PlayerFromArgs(arguments, ref targetCallsign);

            if (target == null)
            {
                State.Chat.SendChatToUser(null, caller, Resources.ArgumentNoUserMessage + targetCallsign, false);
                return;
            }

            if (target == caller)
            {
                State.Chat.SendChatToUser(null, caller, Resources.KickSelfMessage, false);
                return;
            }

            if (!PermissionProcessor.PlayerHasPermision(target, PermissionNames.KickImmunity))
            {
                State.Chat.SendChatToUser(null, caller, Resources.KickImmunityMessage + targetCallsign, false);
                return;
            }

            string logMessage = "Player " + caller.Callsign + " kicking target " + target.Callsign;
            Logger.Log1(logMessage);
            State.Chat.SendChatToGroup(null, State.Chat.AdminGroup, logMessage, false);

            string[] parts = arguments.Split(" ".ToCharArray(), 2);
            string reason = "Kicked";
            if (parts.Length > 1 && parts[1] != string.Empty)
                reason = parts[1];

            State.Chat.SendChatToUser(null, target, Resources.KickTargetMessagePrefix + reason, false);
            target.FlushTCP();

            API.KickUser(target);
            State.Chat.SendChatToUser(null, caller, Resources.KickCompleteMessage + targetCallsign, false);
        }

        protected void Ban(string command, string arguments, ServerPlayer caller)
        {
            if (!PermissionProcessor.PlayerHasPermision(caller, PermissionNames.Ban))
            {
                State.Chat.SendChatToUser(null, caller, Resources.BanNonAuthMessage, false);
                return;
            }

            string targetCallsign = string.Empty;
            ServerPlayer target = PlayerFromArgs(arguments, ref targetCallsign);

            if (target == null)
            {
                State.Chat.SendChatToUser(null, caller, Resources.ArgumentNoUserMessage + targetCallsign, false);
                return;
            }

            if (target == caller)
            {
                State.Chat.SendChatToUser(null, caller, Resources.BanSelfMessage, false);
                return;
            }

            if (!PermissionProcessor.PlayerHasPermision(target, PermissionNames.BanImmunity))
            {
                State.Chat.SendChatToUser(null, caller, Resources.BanImmunityMessage + targetCallsign, false);
                return;
            }

            string[] parts = arguments.Split(" ".ToCharArray(), 4);

            string banType = string.Empty;
            string reason = Resources.BanDefaultReason;
            string banDurration = string.Empty;

            if (parts.Length >= 3)
            {
                banType = parts[1].ToUpperInvariant();
                banDurration = parts[2].ToUpperInvariant();
                if (parts.Length == 4)
                    reason = parts[3];
            }

            bool validArgs = true;

            bool strongBan = false;
            if (banType == "WEAK")
                strongBan = false;
            else if (banType == "STRONG")
                strongBan = true;
            else
                validArgs = false;

            int banTime = ParseBanTime(banDurration);
            
            if (banTime == int.MinValue)
                validArgs = false;

            if (!validArgs)
            {
                State.Chat.SendChatToUser(null, caller, Resources.BanMalformedMessage, false);
                return;
            }

            string logMessage = "Player " + caller.Callsign + " kick/baning target " + target.Callsign;
            Logger.Log1(logMessage);
            State.Chat.SendChatToGroup(null, State.Chat.AdminGroup, logMessage, false);

            // build up the ban

            string address = target.ConnectionData.GetIPAsString();
            string host = target.ConnectionData.HostEntry.ToString();

            if (!strongBan)
            {
                if (target.ConnectionData.GetIPAddress().AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    string[] addressParts = address.Split(":".ToCharArray());
                    if (addressParts.Length > State.ConfigData.Security.IPV6SubnetBanRange)
                        Array.Resize(ref addressParts, addressParts.Length - State.ConfigData.Security.IPV6SubnetBanRange);

                    address = String.Join(":", addressParts);
                }
                else
                {
                    string[] addressParts = address.Split(".".ToCharArray());
                    if (addressParts.Length > State.ConfigData.Security.IPV4SubnetBanRange)
                        Array.Resize(ref addressParts, addressParts.Length - State.ConfigData.Security.IPV4SubnetBanRange);

                    address = String.Join(".", addressParts);
                }
            }

            API.AddBan(target.BZID, address, host, reason, caller.Callsign, 0, banTime);

            if (reason != string.Empty)
            {
                State.Chat.SendChatToUser(null, target, Resources.KickTargetMessagePrefix + parts[1], false);
                target.FlushTCP();
            }

            API.KickUser(target);
            State.Chat.SendChatToUser(null, caller, Resources.BanCompleteMessage + targetCallsign, false);
        }

        int ParseBanTime(string text)
        {
            if (text == "FULL")
                return -1;

            string dayString = string.Empty;
            string minString = string.Empty;

            if (text.Contains("D") && text.Contains("M"))
            {
                int i = text.IndexOf("M");
                dayString = text.Substring(1, i - 2);
                minString = text.Substring(i + 1);
            }
            else if (text.Contains("D"))
            {
                int day = 0;
                if (!int.TryParse(text.Substring(1), out day))
                    return int.MinValue;

                return day * 24 * 60;
            }
            else if (text.Contains("M"))
            {
                int min = 0;
                if (!int.TryParse(text.Substring(1), out min))
                    return int.MinValue;

                return min;
            }

            if (dayString == string.Empty && minString == string.Empty)
                return int.MinValue;

            int val = 0;
            if (dayString != string.Empty)
            {
                int days = 0;
                if (!int.TryParse(dayString, out days))
                    return int.MinValue;

                val += days * 24 * 60;
            }

            if (minString != string.Empty)
            {
                int min = 0;
                if (!int.TryParse(minString, out min))
                    return int.MinValue;

                val += min;
            }
            return val;
        }

        private ServerPlayer PlayerFromArgs(string args, ref string callsign)
        {
            string[] parts = args.Split(" ".ToCharArray(), 2);
            callsign = parts[0];

            return State.Players.GetPlayerByCallsign(callsign);
        }
    }
}
