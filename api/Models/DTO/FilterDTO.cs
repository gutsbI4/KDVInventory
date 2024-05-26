namespace api.Models.DTO
{
    public class FilterDTO
    {
        private string _nameFilter;
        private bool _isPick;
        private int _value;
        public string NameFilter
        {
            get => _nameFilter;
            set { _nameFilter = value; }
        }

        public bool IsPick
        {
            get => _isPick;
            set { _isPick = value; }
        }
        public int Value
        {
            get => _value;
            set {
                _value = value;
            }
        }
    }
}
