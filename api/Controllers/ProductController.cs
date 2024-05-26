using api.Data;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using api.Services;
using System.Security.Claims;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    KDVDbContext _dbContext;
    public ProductController(KDVDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    [Authorize]
    [HttpGet]
    [Route("GetProducts")]
    public async Task<ActionResult<ProductCollectionDTO>> GetProducts([FromQuery] OwnerParameters ownerParametrs, int? categoryId = null)
    {
        int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        try
        {
            IQueryable<Product> productsQuery;
            if (categoryId != null && categoryId !=0)
            {
                productsQuery = _dbContext.Product.Include(p => p.ProductImage).Include(p=>p.ProductDetails)
                    .Include(p=>p.PriceUnit)
                    .Include(p=>p.ReceiptOrderProduct).ThenInclude(p=>p.ReceiptOrder)
                    .Include(p=>p.ExpenseOrderProduct).ThenInclude(p=>p.ExpenseOrder)
                    .Include(p=>p.OrderProduct).ThenInclude(p=>p.Order)
                    .Where(p=>p.CategoryId == categoryId);
            }
            else productsQuery = _dbContext.Product.Include(p => p.ProductImage).Include(p => p.ProductDetails).Include(p => p.PriceUnit)
                    .Include(p => p.ReceiptOrderProduct).ThenInclude(p => p.ReceiptOrder)
                    .Include(p => p.ExpenseOrderProduct).ThenInclude(p => p.ExpenseOrder)
                    .Include(p => p.OrderProduct).ThenInclude(p => p.Order);
            if (!String.IsNullOrEmpty(ownerParametrs.SearchString))
            {
                string search = ownerParametrs.SearchString.ToLower();
                productsQuery = productsQuery.Where(b => b.Name.ToLower().Contains(search)
                || b.Description.ToLower().Contains(search) || b.ProductNumber.ToLower().Contains(search));
            }
            if (!String.IsNullOrEmpty(ownerParametrs.Filter))
            {
                List<FilterParameter>? filterParameters = JsonSerializer.Deserialize<List<FilterParameter>>(ownerParametrs.Filter);
                if (filterParameters != null && filterParameters.Count > 0)
                {
                    var groupFilters = filterParameters.GroupBy(p => p.NameParameter);
                    foreach (var group in groupFilters)
                    {
                        switch (group.Key.ToLower())
                        {
                            case "brand":
                                productsQuery = productsQuery.Where(p => group.Select(p => p.Value).Contains(p.ProductDetails.BrandId));
                                break;
                            case "manufacturer":
                                productsQuery = productsQuery.Where(p => group.Select(p => p.Value).Contains(p.ProductDetails.ManufacturerId));
                                break;
                            case "package":
                                productsQuery = productsQuery.Where(p => group.Select(p => p.Value).Contains(p.ProductDetails.PackageId));
                                break;
                            case "type":
                                productsQuery = productsQuery.Where(p => group.Select(p => p.Value).Contains(p.ProductDetails.TypeId));
                                break;
                            case "taste":
                                productsQuery = productsQuery.Where(p => group.Select(p => p.Value).Contains(p.ProductDetails.TasteId));
                                break;
                            case "filling":
                                productsQuery = productsQuery.Where(p => group.Select(p => p.Value).Contains(p.ProductDetails.FillingId));
                                break;
                            case "diet":
                                productsQuery = productsQuery.Where(p => group.Select(p => p.Value).Contains(p.ProductDetails.DietId));
                                break;
                        }
                    }
                }
            }
            if (ownerParametrs.Sorts != null)
            {
                switch (ownerParametrs.Sorts)
                {
                    case 0:
                        productsQuery = productsQuery.OrderBy(x => x.PriceOfSale);
                        break;
                    case 1:
                        productsQuery = productsQuery.OrderByDescending(x => x.PriceOfSale);
                        break;
                    case 2:
                        productsQuery = productsQuery.OrderBy(x => x.ProductId);
                        break;
                    default:
                        break;
                }
            }
            int count = await productsQuery.CountAsync();
            var products = await productsQuery.Skip((ownerParametrs.PageNumber - 1) * ownerParametrs.SizePage)
                                              .Take(ownerParametrs.SizePage)
                                              .ToListAsync();

            await AuditLogger.AddAuditRecord(_dbContext, idUser, "Получил список продукции.");

            return Ok(new ProductCollectionDTO(products.ConvertAll(x => new ProductDTO(x)), count));
        }
        catch (JsonException)
        {
            return BadRequest();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
    [Authorize]
    [HttpDelete]
    [Route("DeleteProduct/{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        try
        {
            var product = await _dbContext.Product.FindAsync(id);
            if (product == null)
                return NotFound("Товар с указанным id не найден");
            _dbContext.Product.Remove(product);
            await _dbContext.SaveChangesAsync();
            await AuditLogger.AddAuditRecord(_dbContext, idUser, $"Удалил продукцию. ({product.Name})");
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    [Authorize]
    [HttpGet]
    [Route("GetProductEdit/{id}")]
    public async Task<ActionResult<ProductEditDTO>> GetProductEdit(int id)
    {
        try
        {
            var product = await _dbContext.Product
                                .Include(p => p.ProductDetails)
                                .Include(p => p.EnergyValue)
                                .Include(p => p.Category)
                                .Include(p => p.PriceUnit)
                                .Include(p=>p.ProductImage)
                                .FirstOrDefaultAsync(b => b.ProductId == id);
            if (product == null) return NotFound();
            return Ok(new ProductEditDTO(product));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    [Authorize]
    [HttpPut]
    [Route("UpdateProduct")]
    public async Task<ActionResult> UpdateProduct([FromBody] ProductEditDTO productEditDTO)
    {
        int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        try
        {
            var product = await _dbContext.Product
                                .Include(p => p.ProductDetails)
                                .Include(p => p.EnergyValue)
                                .Include(p => p.Category)
                                .Include(p => p.PriceUnit)
                                .Include(p => p.ProductImage)
                                .FirstOrDefaultAsync(b => b.ProductId == productEditDTO.ProductId);
            if (product == null) return NotFound();
            product.Name = productEditDTO.Name;
            product.Barcode = productEditDTO.Barcode;
            product.ProductNumber = productEditDTO.ProductNumber;
            product.Description = productEditDTO.Description;
            product.PriceOfSale = productEditDTO.PriceOfSale;
            product.MinPriceOfSale = productEditDTO.MinPriceOfSale;
            product.PurchasePrice = productEditDTO.PurchasePrice;
            product.LowStockThreshold = productEditDTO.LowStockThreshold;
            product.CategoryId = productEditDTO.CategoryId;
            product.PriceUnitId = productEditDTO.PriceUnitId;
            List<ProductImage> productImages = new List<ProductImage>();
            if (productEditDTO.ProductImage != null)
            {
                foreach (var newImage in productEditDTO.ProductImage)
                {
                    productImages.Add(new ProductImage { Path = newImage.Path });
                }
                product.ProductImage = productImages;
            }
            if (productEditDTO.Energy != null)
            {
                var energyValue = new EnergyValue();
                energyValue.Proteins = productEditDTO.Energy.Proteins;
                energyValue.Carbs = productEditDTO.Energy.Carbs;
                energyValue.Calories = productEditDTO.Energy.Calories;
                energyValue.Fats = productEditDTO.Energy.Fats;
                product.EnergyValue = energyValue;
            }
            if (productEditDTO.Details != null)
            {
                var productDetails = new ProductDetails();
                productDetails.ManufacturerId = productEditDTO.Details.ManufacturerId;
                productDetails.BrandId = productEditDTO.Details.BrandId;
                productDetails.DietId = productEditDTO.Details.DietId;
                productDetails.PackageId = productEditDTO.Details.PackageId;
                productDetails.CountryOfProduction = productEditDTO.Details.CountryOfProduction;
                productDetails.FillingId = productEditDTO.Details.FillingId;
                productDetails.ShelfLife = productEditDTO.Details.ShelfLife;
                productDetails.TasteId = productEditDTO.Details.TasteId;
                productDetails.TypeId = productEditDTO.Details.TypeId;
                productDetails.Weight = productEditDTO.Details.Weight;
                product.ProductDetails = productDetails;
            }
            await _dbContext.SaveChangesAsync();
            await AuditLogger.AddAuditRecord(_dbContext, idUser, $"Обновил информацию о продукте. ID: {product.ProductId}");
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "Не удалось сохранить товар. Проверьте все ли поля заполнены!");
        }
    }
    [Authorize]
    [HttpPost]
    [Route("AddProduct")]
    public async Task<ActionResult> AddProduct([FromBody] ProductEditDTO productEditDTO)
    {
        int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        try
        {
            Product product = new Product();
            product.Name = productEditDTO.Name;
            product.Barcode = productEditDTO.Barcode;
            product.ProductNumber = productEditDTO.ProductNumber;
            product.Description = productEditDTO.Description;
            product.PriceOfSale = productEditDTO.PriceOfSale;
            product.MinPriceOfSale = productEditDTO.MinPriceOfSale;
            product.PurchasePrice = productEditDTO.PurchasePrice;
            product.LowStockThreshold = productEditDTO.LowStockThreshold;
            product.CategoryId = productEditDTO.CategoryId;
            product.PriceUnitId = productEditDTO.PriceUnitId;
            List<ProductImage> productImages = new List<ProductImage>();
            if (productEditDTO.ProductImage != null)
            {
                foreach (var newImage in productEditDTO.ProductImage)
                {
                    productImages.Add(new ProductImage { Path = newImage.Path});
                }
                product.ProductImage = productImages;
            }
            if (productEditDTO.Energy != null)
            {
                var energyValue = new EnergyValue();
                energyValue.Proteins = productEditDTO.Energy.Proteins;
                energyValue.Carbs = productEditDTO.Energy.Carbs;
                energyValue.Calories = productEditDTO.Energy.Calories;
                energyValue.Fats = productEditDTO.Energy.Fats;
                product.EnergyValue = energyValue;
            }
            if (productEditDTO.Details != null)
            {
                var productDetails = new ProductDetails();
                productDetails.ManufacturerId = productEditDTO.Details.ManufacturerId;
                productDetails.BrandId = productEditDTO.Details.BrandId;
                productDetails.DietId = productEditDTO.Details.DietId;
                productDetails.PackageId = productEditDTO.Details.PackageId;
                productDetails.CountryOfProduction = productEditDTO.Details.CountryOfProduction;
                productDetails.FillingId = productEditDTO.Details.FillingId;
                productDetails.ShelfLife = productEditDTO.Details.ShelfLife;
                productDetails.TasteId = productEditDTO.Details.TasteId;
                productDetails.TypeId = productEditDTO.Details.TypeId;
                productDetails.Weight = productEditDTO.Details.Weight;
                product.ProductDetails = productDetails;
            }
            await _dbContext.Product.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            await AuditLogger.AddAuditRecord(_dbContext, idUser, $"Добавил новую продукцию. ID: {product.ProductId}");
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented,"Не удалось добавить товар. Проверьте все ли поля заполнены!");
        }
    }
    [Authorize]
    [HttpGet]
    [Route("GetRandomBarcode")]
    public async Task<ActionResult<long>> GetRandomBarcode()
    {
        try
        {
            var barcodes = await _dbContext.Product.Select(x => x.Barcode != null ? x.Barcode.ToString() : String.Empty).ToListAsync();

            string countryCode = "460";
            string companyCode = "0012";

            var random = new Random();
            string barcode;

            do
            {
                var productCode = random.Next(0, 99999).ToString("D5");

                string barcodeWithoutChecksum = countryCode + companyCode + productCode;
                long checksum = CalculateEAN13Checksum(barcodeWithoutChecksum);

                barcode = barcodeWithoutChecksum + checksum.ToString();
            }
            while (barcodes != null && barcodes.Contains(barcode));

            return Ok(long.Parse(barcode));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
    [Authorize]
    [HttpGet]
    [Route("GetRandomArticle")]
    public async Task<ActionResult<string>> GetRandomArticle()
    {
        try
        {
            List<string> articles = await _dbContext.Product
                                .Select(x => x.ProductNumber != null ? x.ProductNumber.ToString() : String.Empty)
                                .ToListAsync();

            var random = new Random();
            string article;

            do
            {
                char[] letters = Enumerable.Range(0x0410, 32).Select(i => (char)i).ToArray();
                string randomLetters = new string(Enumerable.Repeat(letters, 3).Select(s => s[random.Next(s.Length)]).ToArray());
                string randomNumbers = random.Next(0, 1000).ToString("D3");
                article = randomLetters + randomNumbers;
            }
            while (articles.Contains(article));

            return Ok(article);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }

    private long CalculateEAN13Checksum(string data)
    {
        long evenSum = data.Where((c, i) => (i + 1) % 2 == 0).Sum(c => c - '0');

        long oddSum = data.Where((c, i) => (i + 1) % 2 != 0).Sum(c => c - '0');

        long totalSum = oddSum + evenSum * 3;

        long checksum = (10 - (totalSum % 10)) % 10;

        return checksum;
    }
}
