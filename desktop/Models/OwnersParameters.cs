using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Models
{
    public class OwnersParameters : ReactiveObject
    {
        private int? _pageNumber = 1;
        private int? _sizePage = 10;
        private string? _searchString;
        private string? _filterParameters;
        private int? _sortsParameters = 2;
        public int PageNumber
        {
            get => _pageNumber.GetValueOrDefault(1);
            set => this.RaiseAndSetIfChanged(ref _pageNumber, value);
        }
        public int SizePage
        {
            get => _sizePage.GetValueOrDefault(10);
            set => this.RaiseAndSetIfChanged(ref _sizePage, value);
        }
        public string SearchString
        {
            get => _searchString;
            set => this.RaiseAndSetIfChanged(ref _searchString, value);
        }
        public string Filter
        {
            get => _filterParameters;
            set => this.RaiseAndSetIfChanged(ref _filterParameters, value);
        }
        public int Sorts
        {
            get => _sortsParameters.GetValueOrDefault(2);
            set => this.RaiseAndSetIfChanged(ref _sortsParameters, value);
        }
    }
}
