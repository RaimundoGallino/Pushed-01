using System;

namespace DreadZitoTypes
{
    public class DreDictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue> where TValue : new()
    {
        public TValue GetOrCreate(TKey key)
        {
            if (!this.TryGetValue(key, out TValue val))
            {
                val = new TValue();
                this.Add(key, val);
            }

            return val;
        }
    }

    public class DreList<T> : System.Collections.Generic.List<T>
    {
        public override string ToString()
        {
            string response = "DREADZITOLIST-uwu-[ ";

            foreach (var item in this) {
                response += item.ToString();
                if (this.IndexOf(item) != this.Count - 1)
                    response += ", ";
            }
            response += " ]";

            return response;
        }
    }
}
