using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IProductRepository
    {
        [Get("/Product/GetProducts")]
        public Task<ProductsCollection> GetProducts([Authorize("Bearer")] string accessToken, [Query] OwnersParameters ownersParameters, int? categoryId = null);
        [Get("/Product/GetProduct/{id}")]
        public Task<ProductDetails> GetProduct([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
        [Delete("/Product/DeleteProduct/{id}")]
        public Task DeleteProduct([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
        [Get("/Product/GetProductEdit/{id}")]
        public Task<ProductEdit> GetProductEdit([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
        [Put("/Product/UpdateProduct")]
        public Task UpdateProduct([Authorize("Bearer")] string accessToken, [Body] ProductEdit productEdit);
        [Post("/Product/AddProduct")]
        public Task AddProduct([Authorize("Bearer")] string accessToken, [Body] ProductEdit productEdit);
        [Get("/Product/GetRandomBarcode")]
        public Task<long> GetRandomBarcode([Authorize("Bearer")] string accessToken);
        [Get("/Product/GetRandomArticle")]
        public Task<string> GetRandomArticle([Authorize("Bearer")] string accessToken);
    }
}
