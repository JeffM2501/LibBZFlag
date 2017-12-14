using BZFlag.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using BZFlag.LinearMath;

namespace BZFlag.Data.BZDB
{
    public static class BZDBVarNames
    {
        public static readonly string Gravity = "_gravity";
    }

    public class Database
    {
        public class DatabaseItem : EventArgs
        {
            public string Key { get; internal set; }
            public string Value { get; internal set; }

            public string RawValue { get; internal set; }
            public string DefaultValue { get; internal set; }

            public double DoubleValue { get; internal set; }
            public Vector3F VectorValue { get; internal set; }

            public enum ComputationTypes
            {
                None,
                Assignment,
                Addition,
                Subtraction,
                Multiplication,
                Division,
            }
            public ComputationTypes Computation = ComputationTypes.None;

            public class ComputationArgument
            {
                public bool IsContant = false;
                public double Constant = 0;
                public DatabaseItem ItemReference = null;
            }

            public List<ComputationArgument> Arguments = new List<ComputationArgument>();

            public event EventHandler<DatabaseItem> Changed = null;

            public bool Trasmit = true;

            public DatabaseItem() { Key = string.Empty; Value = string.Empty; }
            public DatabaseItem(string k, string v) { Key = k; Value = v; }

            public void CallChanged()
            {
                Changed?.Invoke(this, this);
            }

            internal void InitValue(string value)
            {
                RawValue = value;
                DefaultValue = value;
                Value = value;
            }

            internal void SetValue(string value, Database db)
            {
                Value = value;
                RawValue = value;
                if (db != null)
                {
                    SetupValue(db);
                    ResolveValue();
                }    
            }

            internal void SetupValue(Database db)
            {
                foreach (var arg in Arguments)
                {
                    if (arg.ItemReference != null)
                        arg.ItemReference.Changed -= DependentItem_Changed;

                }
                Arguments.Clear();

                Value = RawValue;
                VectorValue = Vector3F.Zero;
                DoubleValue = double.MinValue;
                Computation = ComputationTypes.None;

                double v = double.MinValue;
                if (double.TryParse(Value, out v))
                {
                    DoubleValue = v;
                    return;
                }

                if (Value.Contains("+"))
                    SetupAdd(db);
                else if (Value.Contains("-"))
                    SetupSubtract(db);
                else if (Value.Contains("*"))
                    SetupMultiply(db);
                else if (Value.Contains("/"))
                    SetupDivide(db);
                else if (Value != string.Empty && Value[0] == '_')
                    SetupAssignment(db);
            }

            private void SetupDualArguments(Database db, string operatorSign)
            {
                string[] parts = Value.Split(operatorSign.ToCharArray());
                if (parts.Length != 2)
                {
                    Computation = ComputationTypes.None;
                    return;
                }

                foreach (string p in parts)
                {
                    string text = p.Trim();

                    ComputationArgument arg = new ComputationArgument();

                    if (double.TryParse(text, out arg.Constant))
                        arg.IsContant = true;
                    else
                    {
                        arg.IsContant = false;
                        arg.ItemReference = db.FindItem(text);
                        if (arg.ItemReference == null)
                        {
                            Computation = ComputationTypes.None;
                            return;
                        }

                        arg.ItemReference.Changed += DependentItem_Changed;
                    }
                    Arguments.Add(arg);
                }
            }

            private double ResolveArgumentValue(ComputationArgument arg)
            {
                if (arg.IsContant)
                    return arg.Constant;

                if (arg.ItemReference != null)
                    return arg.ItemReference.DoubleValue;

                return 0;
            }

            private void SetupAdd(Database db)
            {
                Computation = ComputationTypes.Addition;
                SetupDualArguments(db, "+");
            }

            private void ResolveAddition()
            {
                if (Arguments.Count != 2 )
                    return;

                DoubleValue = ResolveArgumentValue(Arguments[0]) + ResolveArgumentValue(Arguments[1]);
                Value = DoubleValue.ToString();
            }

            private void SetupSubtract(Database db)
            {
                Computation = ComputationTypes.Subtraction;
                SetupDualArguments(db, "-");
            }

            private void ResolveSubtraction()
            {
                if (Arguments.Count != 2)
                    return;

                DoubleValue = ResolveArgumentValue(Arguments[0]) - ResolveArgumentValue(Arguments[1]);
                Value = DoubleValue.ToString();
            }

            private void SetupDivide(Database db)
            {
                Computation = ComputationTypes.Division;

                SetupDualArguments(db, "/");
            }

            private void ResolveDivide()
            {
                if (Arguments.Count != 2)
                    return;

                double v = ResolveArgumentValue(Arguments[1]);
                if (v == 0)
                    DoubleValue = 0;
                else
                    DoubleValue = ResolveArgumentValue(Arguments[0]) / v;

                Value = DoubleValue.ToString();
            }

            private void SetupMultiply(Database db)
            {
                Computation = ComputationTypes.Multiplication;
                SetupDualArguments(db, "*");
            }

            private void ResolveMultipy()
            {
                if (Arguments.Count != 2)
                    return;

                DoubleValue = ResolveArgumentValue(Arguments[0]) * ResolveArgumentValue(Arguments[1]);
                Value = DoubleValue.ToString();
            }

            private void ResolveAssignment()
            {
                if (Arguments.Count == 0 || Arguments[0].IsContant || Arguments[0].ItemReference == null)
                    return;

                Value = Arguments[0].ItemReference.Value;
                DoubleValue = Arguments[0].ItemReference.DoubleValue;
                VectorValue = Arguments[0].ItemReference.VectorValue;
            }

            private void SetupAssignment(Database db)
            {
                Computation = ComputationTypes.Assignment;

                ComputationArgument arg = new ComputationArgument();

                arg.IsContant = false;
                arg.ItemReference = db.FindItem(Value);
                if (arg.ItemReference == null)
                    return;
                arg.ItemReference.Changed += DependentItem_Changed;
                Arguments.Add(arg);
            }

            public void ResolveValue()
            {
                if (Arguments.Count == 0 || Computation == ComputationTypes.None)
                    return;

                VectorValue = Vector3F.Zero;

                switch (Computation)
                {
                    case ComputationTypes.Assignment:
                        ResolveAssignment();
                        return;

                    case ComputationTypes.Addition:
                        ResolveAddition();
                        return;

                    case ComputationTypes.Subtraction:
                        ResolveSubtraction();
                        return;

                    case ComputationTypes.Division:
                        ResolveDivide();
                        return;

                    case ComputationTypes.Multiplication:
                        ResolveMultipy();
                        return;
                }
            }

            private void DependentItem_Changed(object sender, DatabaseItem e)
            {
                ResolveValue();
                Changed?.Invoke(this, this);
            }   
        }

        internal Dictionary<string, DatabaseItem> RawBZDBVariables = new Dictionary<string, DatabaseItem>();

        public DatabaseItem[] GetVars() { lock(RawBZDBVariables) return RawBZDBVariables.Values.ToArray(); }

        internal DatabaseItem FindItem(string name)
        {
            if (RawBZDBVariables.ContainsKey(name))
                return RawBZDBVariables[name];

            return null;
        }
    
        public event EventHandler<DatabaseItem> ValueChanged = null;
        public event EventHandler InitalLoadCompleted = null;

        public Dictionary<string, EventHandler<DatabaseItem>> NotificationEvents = new Dictionary<string, EventHandler<DatabaseItem>>();

        public void RegisterVariableChangeNotifiacation(string name, EventHandler<DatabaseItem> handler)
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
            if (RawBZDBVariables.ContainsKey(key))
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

        internal void AddItem(string key, string value)
        {
            DatabaseItem item = new DatabaseItem(key, value);
            item.Changed += DBItem_Changed;
            RawBZDBVariables.Add(key,item);
        }

        private void DBItem_Changed(object sender, DatabaseItem item)
        {
            if (item == null)
                return;

            ValueChanged?.Invoke(this, item);

            if (NotificationEvents.ContainsKey(item.Key))
                NotificationEvents[item.Key].Invoke(this, item);
        }

        internal bool ChangeValue(string key, string value)
        {
            if (!RawBZDBVariables.ContainsKey(key))
                AddItem(key, value);

            RawBZDBVariables[key].Value = value;

            RawBZDBVariables[key].SetupValue(this);
            RawBZDBVariables[key].ResolveValue();

            RawBZDBVariables[key].CallChanged();
            return true;
        }

        public void SetValue(string key, string value)
        {
            ChangeValue(key, value);
        }

        public void InitValues(string key, string value)
        {
            if (!RawBZDBVariables.ContainsKey(key))
                AddItem(key, value);

            RawBZDBVariables[key].InitValue(value);
        }

        public void SetValues(Dictionary<string, string> values, bool callEvents)
        {
            foreach (KeyValuePair<string, string> i in values)
                SetValue(i.Key, i.Value);
        }

        public void FinishLoading()
        {
            foreach (var item in RawBZDBVariables.Values)
                item.SetupValue(this);

            // resolve all the constants
            foreach (var item in RawBZDBVariables.Values)
            {
                if (item.Computation == DatabaseItem.ComputationTypes.None)
                    item.ResolveValue();
            }

            // resolve all the Assignments
            foreach (var item in RawBZDBVariables.Values)
            {
                if (item.Computation == DatabaseItem.ComputationTypes.Assignment)
                    item.ResolveValue();
            }

            // resolve all the math
            foreach (var item in RawBZDBVariables.Values)
            {
                if (item.Computation != DatabaseItem.ComputationTypes.None && item.Computation != DatabaseItem.ComputationTypes.Assignment)
                    item.ResolveValue();
            }

            if (InitalLoadCompleted != null)
                InitalLoadCompleted.Invoke(this, EventArgs.Empty);
        }
    }
}
