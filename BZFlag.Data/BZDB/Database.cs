using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.BZDB
{
    public static class BZDBVarNames
    {
        public static readonly string Gravity = "_gravity";
    }

	public class Database
	{
		protected Dictionary<string, string> RawBZDBVariables = new Dictionary<string, string>();

    
		public class DatabaseChangedEventArgs : EventArgs
		{
			public string Key = string.Empty;
			public string NewValue = string.Empty;
			public string OldValue = string.Empty;
		}

		public event EventHandler<DatabaseChangedEventArgs> ValueChanged = null;
		public event EventHandler InitalLoadCompleted = null;

        public Dictionary<string, EventHandler> NotificationEvents = new Dictionary<string, EventHandler>();

        public void RegisterVariableChangeNotifiacation(string name, EventHandler handler)
        {
            if (NotificationEvents.ContainsKey(name))
                NotificationEvents[name] += handler;
            else
                NotificationEvents[name] = handler;
        }

        public string GetValue(string key)
        {
            if (RawBZDBVariables.ContainsKey(key))
                return RawBZDBVariables[key];
            return string.Empty;
        }

        public double GetValueD(string key)
        {
            double v = 0;
            double.TryParse(GetValue(key), out v);

            return v;
        }

        public float GetValueF(string key)
        {
            float v = 0;
            float.TryParse(GetValue(key), out v);

            return v;
        }

        public bool GetValueB(string key)
        {
            return GetValue(key) == "1";
        }

        public void SetValue(string key, string value)
		{
			DatabaseChangedEventArgs args = new DatabaseChangedEventArgs();
			if(RawBZDBVariables.ContainsKey(key))
			{
				args.OldValue = RawBZDBVariables[key];
				RawBZDBVariables[key] = value;
			}
			else
				RawBZDBVariables.Add(key, value);

			args.Key = key;
			args.NewValue = value;

			if(ValueChanged != null)
				ValueChanged.Invoke(this, args);

            if (NotificationEvents.ContainsKey(key))
                NotificationEvents[key].Invoke(this, args);
		}

		public void SetValues(Dictionary<string,string> values, bool callEvents)
		{
			foreach(KeyValuePair<string,string> i in values)
			{
				if(callEvents)
					SetValue(i.Key, i.Value);
				else
				{
					if(RawBZDBVariables.ContainsKey(i.Key))
						RawBZDBVariables[i.Key] = i.Value;
					else
						RawBZDBVariables.Add(i.Key, i.Value);
				}
			}
		}

		public void FinishLoading()
		{
			if(InitalLoadCompleted != null)
				InitalLoadCompleted.Invoke(this, EventArgs.Empty);
		}
	}
}
