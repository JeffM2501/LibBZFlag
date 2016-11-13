using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.BZDB
{
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
