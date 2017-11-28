using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Networking.Messages;
using BZFlag.Networking.Messages.BZFS.BZDB;
using BZFlag.Data.BZDB;

namespace BZFlag.Game
{
    public partial class Client
    {
        public BZFlag.Data.BZDB.Database BZDatabase = new BZFlag.Data.BZDB.Database();

        protected bool InitalSetVarsStarted = false;
        protected bool InitalSetVarsFinished = false;


        protected void InitDBCallbacks()
        {
            BZDatabase.RegisterVariableChangeNotifiacation(BZDBVarNames.Gravity, HandleGravityChanged);
        }

        private void HandleSetVarsMessage(NetworkMessage msg)
        {
            if (!InitalSetVarsFinished)
                InitalSetVarsStarted = true;

            MsgSetVars vars = msg as MsgSetVars;

            BZDatabase.SetValues(vars.BZDBVariables, false);
        }
    }
}
