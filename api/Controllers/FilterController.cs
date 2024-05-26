using api.Data;
using api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilterController : Controller
    {
        private KDVDbContext _dbContext;
        public FilterController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost]
        [Route("GetFilters")]
        public async Task<ActionResult<List<FilterCategoryDTO>>> GetFilters([FromBody]List<ProductDTO> products)
        {
            List<FilterCategoryDTO> filters = new List<FilterCategoryDTO>();
            if (products == null || products.Count == 0) return filters;
            var productIds = products.Select(p => p.ProductId).ToList();

            var productsWithDetails = await _dbContext.Product
                .Where(p => productIds.Contains(p.ProductId))
                .Include(p => p.ProductDetails)
                .ToListAsync();

            var brandIds = productsWithDetails
                .Select(p => p.ProductDetails.BrandId)
                .Distinct()
                .ToList();
            var brands = await _dbContext.Brand
                .Where(p => brandIds.Contains(p.BrandId))
                .ToListAsync();

            filters.Add(new FilterCategoryDTO
            {
                NameCategory = "Торговая марка",
                ParameterName = "brand",
                Filters = new ObservableCollection<FilterDTO>(brands.Select(x => new FilterDTO
                {
                    NameFilter = x.Name,
                    Value = x.BrandId
                }))
            });

            var manufacturersIds = productsWithDetails
                .Select(p => p.ProductDetails.ManufacturerId)
                .Distinct()
                .ToList();
            var manufacturers = await _dbContext.Manufacturer
                .Where(p => manufacturersIds.Contains(p.ManufacturerId))
                .ToListAsync();

            filters.Add(new FilterCategoryDTO
            {
                NameCategory = "Производитель",
                ParameterName = "manufacturer",
                Filters = new ObservableCollection<FilterDTO>(manufacturers.Select(x => new FilterDTO
                {
                    NameFilter = x.Name,
                    Value = x.ManufacturerId
                }))
            });


            var packagesIds = productsWithDetails
                .Select(p => p.ProductDetails.PackageId)
                .Distinct()
                .Where(p => p.HasValue)
                .ToList();

            if (packagesIds.Any())
            {
                var packages = await _dbContext.Package
                    .Where(p => packagesIds.Contains(p.PackageId))
                    .ToListAsync();

                filters.Add(new FilterCategoryDTO
                {
                    NameCategory = "Упаковка",
                    ParameterName = "package",
                    Filters = new ObservableCollection<FilterDTO>(packages.Select(x => new FilterDTO
                    {
                        NameFilter = x.Name,
                        Value = x.PackageId
                    }))
                });
            }

            var typesIds = productsWithDetails
                .Select(p => p.ProductDetails.TypeId)
                .Distinct()
                .Where(p => p.HasValue)
                .ToList();

            if (typesIds.Any())
            {
                var types = await _dbContext.Type
                    .Where(p => typesIds.Contains(p.TypeId))
                    .ToListAsync();

                filters.Add(new FilterCategoryDTO
                {
                    NameCategory = "Вид",
                    ParameterName = "type",
                    Filters = new ObservableCollection<FilterDTO>(types.Select(x => new FilterDTO
                    {
                        NameFilter = x.Name,
                        Value = x.TypeId
                    }))
                });
            }

            var tastesIds = productsWithDetails
                .Select(p => p.ProductDetails.TasteId)
                .Distinct()
                .Where(p => p.HasValue)
                .ToList();

            if (tastesIds.Any())
            {
                var tastes = await _dbContext.Taste
                    .Where(p => tastesIds.Contains(p.TasteId))
                    .ToListAsync();

                filters.Add(new FilterCategoryDTO
                {
                    NameCategory = "Вкус",
                    ParameterName = "taste",
                    Filters = new ObservableCollection<FilterDTO>(tastes.Select(x => new FilterDTO
                    {
                        NameFilter = x.Name,
                        Value = x.TasteId
                    }))
                });
            }

            var fillingIds = productsWithDetails
                .Select(p => p.ProductDetails.FillingId)
                .Distinct()
                .Where(p => p.HasValue)
                .ToList();

            if (fillingIds.Any())
            {
                var fillings = await _dbContext.Filling
                    .Where(p => fillingIds.Contains(p.FillingId))
                    .ToListAsync();

                filters.Add(new FilterCategoryDTO
                {
                    NameCategory = "Начинка",
                    ParameterName = "filling",
                    Filters = new ObservableCollection<FilterDTO>(fillings.Select(x => new FilterDTO
                    {
                        NameFilter = x.Name,
                        Value = x.FillingId
                    }))
                });
            }

            var dietIds = productsWithDetails
               .Select(p => p.ProductDetails.DietId)
               .Distinct()
               .Where(p => p.HasValue)
               .ToList();

            if (dietIds.Any())
            {
                var diets = await _dbContext.Diet
                    .Where(p => dietIds.Contains(p.DietId))
                    .ToListAsync();

                filters.Add(new FilterCategoryDTO
                {
                    NameCategory = "Диета",
                    ParameterName = "diet",
                    Filters = new ObservableCollection<FilterDTO>(diets.Select(x => new FilterDTO
                    {
                        NameFilter = x.Name,
                        Value = x.DietId
                    }))
                });
            }

            return Ok(filters);
        }
    }
}
