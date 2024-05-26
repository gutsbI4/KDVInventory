using desktop.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Tools
{
    public class SortElement : ReactiveObject
    {
        private bool _isSelected;
        public Sort Sort { get; set; }
        public string NameSort { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
    }
}
