public class OwnerParameters
{
    public const int MaxSizePage = 100;
    private int _sizePage = 10;
    public int PageNumber { get; set; } = 1;
    public int SizePage
    {
        get => _sizePage;
        set => _sizePage = value > MaxSizePage ? MaxSizePage : value;
    }
    public string? SearchString { get;set; }
    public string? Filter { get; set; }
    public int? Sorts { get; set; }
}
