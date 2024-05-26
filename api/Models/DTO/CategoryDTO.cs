using api.Models;

public class CategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Icon { get;set; }
    public CategoryDTO(Category category)
    {
        Id = category.CategoryId;
        Name = category.Name;
        Icon = category.Icon;
    }
}
