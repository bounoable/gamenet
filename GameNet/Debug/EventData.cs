using System.Collections.Generic;

namespace GameNet.Debug
{
    public class EventData
    {
        public object this[string key]
        {
            get => args[key];
            set => Add(key, value);
        }
        
        Dictionary<string, object> args = new Dictionary<string, object>();

        public EventData(object data)
        {
            var props = data.GetType().GetProperties();

            foreach (var prop in props) {
                var value = prop.GetValue(data);
                this[prop.Name] = value;
            }
        }

        public void Add(string key, object value) => args[key] = value;
        public void Remove(string key) => args.Remove(key);

        public bool Has(string key) => args.ContainsKey(key);
        public bool Has<T>(string key) where T: class => args.ContainsKey(key) && args[key] is T;

        public T Get<T>(string key) where T: class
            => args.ContainsKey(key) ? args[key] as T : default(T);
        
        public T Get<T>() where T: class
        {
            var results = new HashSet<object>();

            foreach (var entry in args) {
                if (entry.Value is T) {
                    return entry.Value as T;
                }
            }

            return default(T);
        }
    }
}