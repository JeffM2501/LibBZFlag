using BZFlag.Data.Types;
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
		public class DatabaseItem
		{
			public string Key = string.Empty;
			public string Value = string.Empty;

			public double DoubleValue = double.MinValue;
			public Vector3F VectorValue = Vector3F.Zero;
			public bool IsComputation = false;

			public List<DatabaseItem> DependentItems = new List<DatabaseItem>();
			public event EventHandler Changed = null;

			public bool Trasmit = true;
		}

		protected Dictionary<string, DatabaseItem> RawBZDBVariables = new Dictionary<string, DatabaseItem>();

    
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

        public string GetValueS(string key)
        {
            if (RawBZDBVariables.ContainsKey(key))
                return RawBZDBVariables[key].Value;
            return string.Empty;
        }

        public double GetValueD(string key)
        {
			if(RawBZDBVariables.ContainsKey(key))
				return RawBZDBVariables[key].DoubleValue;
			return double.MinValue;
		}

        public float GetValueF(string key)
        {
            return (float)GetValueD(key);
        }

        public bool GetValueB(string key)
        {
            return GetValueS(key) == "1";
        }

		internal bool ChangeValue(string key, string value)
		{
			DatabaseChangedEventArgs args = new DatabaseChangedEventArgs();
			if(RawBZDBVariables.ContainsKey(key))
			{
				args.OldValue = RawBZDBVariables[key].Value;
			}
			else
				RawBZDBVariables.Add(key, string.Empty);
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
