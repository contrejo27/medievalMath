using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameDataEditor
{
    public abstract partial class IGDEData
    {
		public IGDEData()
		{
			_key = string.Empty;
		}

		public IGDEData(string key)
		{
			Dictionary<string, object> dict;
			if (GDEDataManager.Get(key, out dict))
				LoadFromDict(key, dict);
			else
				LoadFromSavedData(key);
		}

		protected string _key;
		public string Key
		{
			get { return _key; }
			private set { _key = value; }
		}

        public abstract void LoadFromDict(string key, Dictionary<string, object> dict);
		public abstract void LoadFromSavedData(string key);

        public virtual void UpdateCustomItems(bool rebuildKeyList) {}
        public virtual Dictionary<string, object> SaveToDict() { return null; }
    }
}

