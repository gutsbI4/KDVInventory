using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class DateRange : ReactiveObject
    {
        private DateTime? _dateOne;
        private DateTime? _dateTwo;
        public DateTime? DateOne
        {
            get => _dateOne;
            set => this.RaiseAndSetIfChanged(ref _dateOne, value);
        }
        public DateTime? DateTwo
        {
            get => _dateTwo;
            set => this.RaiseAndSetIfChanged(ref _dateTwo, value);
        }
    }
}
