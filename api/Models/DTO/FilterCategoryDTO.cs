using System.Collections.ObjectModel;

namespace api.Models.DTO
{
    public class FilterCategoryDTO
    {
        private string _nameCategory;
        private ObservableCollection<FilterDTO> _filters;
        public string ParameterName { get; set; }
        public string NameCategory
        {
            get => _nameCategory;
            set {
                _nameCategory = value;
            }
        }
        public ObservableCollection<FilterDTO> Filters
        {
            get => _filters;
            set {
                _filters = value;
            }
        }
    }
}
