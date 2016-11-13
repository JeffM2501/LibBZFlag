using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Networking.Messages;
using BZFlag.Networking.Messages.BZFS.BZDB;

namespace BZFlag.Game
{
	public partial class Client
	{
		public BZFlag.Data.BZDB.Database BZDatabase = new BZFlag.Data.BZDB.Database();

		private void HandleSetVarsMessage(NetworkMessage msg)
		{
			MsgSetVars vars = msg as MsgSetVars;

			BZDatabase.SetValues(vars.BZDBVariables, false);
		}
	}
}
