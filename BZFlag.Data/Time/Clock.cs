using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZFlag.Data.Time
{
    public class Clock
    {
        protected Stopwatch Ticker = new Stopwatch();

        public double LastUpdateTime { get; protected set; }
        public double Now { get; protected set; }
        public double Delta { get; protected set; }

        public event EventHandler Updated;

        public Clock()
        {
            lock(Ticker)
            {
                Ticker.Start();
                Now = Double.MinValue;
                LastUpdateTime = Ticker.ElapsedMilliseconds * 0.001;
            }
           
            Update();
        }

        public void Update()
        {
            lock (Ticker)
            {
                LastUpdateTime = Now;
                Now = Ticker.ElapsedMilliseconds * 0.001;
                Delta = Now - LastUpdateTime;
            }
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
