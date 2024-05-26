public class ProductCollectionDTO
{
    public ProductCollectionDTO(IEnumerable<ProductDTO> products, int count)
    {
        Products = products;
        Count = count;
    }

    public IEnumerable<ProductDTO> Products { get; set; }
    public int Count { get; set; }
}
