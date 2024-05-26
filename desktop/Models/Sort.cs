using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class Sort : ReactiveObject
    {
        private bool? _direction;
        public string NameColumn { get; set; }
        public bool Direction
        {
            get => _direction.GetValueOrDefault();
            set => this.RaiseAndSetIfChanged(ref _direction, value);
        }
    }
}
