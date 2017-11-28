using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Game
{
    public class GameTime
    {
        protected static readonly double FilterTime = 10.0;
        protected static readonly int MaxRecords = 1024;
        protected static readonly Int64 MaxRecordAge = 120 * 1000000;
        protected static readonly Int64 MaxTime = 2345678;
        protected static readonly double MinRate = 0.50;
        protected static readonly double MaxRate = 2.00;

        public class TimeRecord
        {
            public Int64 NetTime = 0;
            public Int64 RawTime = 0;

            public TimeRecord(Int64 nt, Int64 rt)
            {
                NetTime = nt;
                RawTime = rt;
            }
        }

        public List<TimeRecord> TimeRecs = new List<TimeRecord>();

        protected Int64 StepTicks = 0;
        protected double AvgRate = 1.0;
        protected TimeRecord AvgPoint = new TimeRecord(0, 0);

        public double StepDelta { get; protected set; }
        public double StepTime { get; protected set; }


        public static Int64 RealNow() { return System.DateTime.UtcNow.ToFileTime(); }

        public void AddTimeUpdate(Int64 netTime)
        {
            TimeRecord r = new TimeRecord(netTime, RealNow());
            TimeRecs.Insert(0, r);
            while (TimeRecs.Count > MaxRecords)
                TimeRecs.RemoveAt(TimeRecs.Count - 1);

            if (TimeRecs.Count > 0)
            {
                Int64 now = RealNow();
                foreach (var tr in TimeRecs.ToArray().Reverse())
                {
                    if ((now - tr.RawTime) < MaxRecordAge)
                        break;
                    TimeRecs.Remove(tr);
                }
            }

            Update();
        }

        public void Reset()
        {
            StepTime = 0.0;
            StepDelta = 0.0;
            StepTicks = 0;

            AvgRate = 1.0;
            AvgPoint = new TimeRecord(0, 0);

            TimeRecs.Clear();
        }

        public void ResetToRecord(TimeRecord record)
        {
            AvgRate = 1.0;
            AvgPoint = record;

            StepTicks = record.NetTime;

            TimeRecs.Clear();
            TimeRecs.Add(new TimeRecord(record.NetTime, record.RawTime));
        }

        protected void CalcAvgRate()
        {
            // FIXME - this is weak
            if (TimeRecs.Count <= 0)
            {
                AvgRate = 1.0;
                AvgPoint = new TimeRecord(0, 0);
                return;
            }
            else if (TimeRecs.Count == 1)
            {
                AvgRate = 1.0;
                AvgPoint = TimeRecs[0];
            }
            else
            {
                TimeRecord last = TimeRecs[0];
                TimeRecord first = TimeRecs[TimeRecs.Count - 1];

                Int64 netDiff = last.NetTime - first.NetTime;
                Int64 locDiff = last.RawTime - first.RawTime;

                if (locDiff != 0.0)
                {
                    AvgRate = ((double)netDiff / (double)locDiff);
                    AvgPoint = last;
                }
                else
                {
                    // don't update
                }
            }
        }

        public void Update()
        {
            if (TimeRecs.Count == 0)
                ResetToRecord(new TimeRecord(0, 0));
            else if (TimeRecs.Count == 1)
                ResetToRecord(TimeRecs[0]);
            else
            {
                CalcAvgRate();
                TimeRecord tr = TimeRecs[0];

                Int64 diffTime = StepTicks - tr.NetTime;
                if ((diffTime < -MaxTime) || (diffTime > MaxTime) || (AvgRate < MinRate) || (AvgRate > MaxRate))
                {
                    // discontinuity
                    ResetToRecord(tr);
                }
            }
        }

        private Int64 LastStep = 0;

        public void SetStepTime()
        {
            Int64 thisStep = RealNow();
            if (TimeRecs.Count == 0)
                StepTicks = thisStep;
            else
            {
                // long term prediction
                double diffLocal = (double)(thisStep - AvgPoint.RawTime);
                double longPred = (double)AvgPoint.NetTime + (diffLocal * AvgRate);

                // short term prediction
                double skipTime = (double)(thisStep - LastStep);
                double shortPred = (double)StepTicks + (skipTime * AvgRate);

                // filtering
                double c = (skipTime * 1.0e-6) / FilterTime;
                double a = (c > 0.0) && (c < 1.0) ? c : 0.5;
                double b = 1.0 - a;
                StepTicks = (Int64)((a * longPred) + (b * (double)shortPred));
            }

            double secs = (double)StepTicks * 1.0e-6;
            StepDelta = secs - StepTime;

            StepTime = secs;
            LastStep = thisStep;
        }

    }
}
