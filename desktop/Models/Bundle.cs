using desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class Bundle
    {
        public ViewModelBase OwnerViewModel { get; private set; }
        private Dictionary<string, object> _dictionary;
        public Bundle(ViewModelBase ownerViewModel)
        {
            OwnerViewModel = ownerViewModel;
            _dictionary = new Dictionary<string, object>();
        }
        public bool AddNewParameter(string key, object value) => _dictionary.TryAdd(key, value);
        public bool RemoveParameter(string key) => _dictionary.Remove(key);
        public object? GetParameter(string key) => _dictionary.GetValueOrDefault(key);
    }
}
