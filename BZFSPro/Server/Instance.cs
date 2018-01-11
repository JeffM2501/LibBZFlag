using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Game.Host;

namespace BZFSPro.Server
{
    public class Instance
    {
        private bool IsDone = false;
        public bool Done()
        {
            lock (this)
                return IsDone;
        }

        public GameState State = new GameState();

        public void Run( ServerConfig config )
        {
            // startup
            State.Init(config);

            // run
            while (!Done())
            {

            }

            // cleanup
        }
    }
}
