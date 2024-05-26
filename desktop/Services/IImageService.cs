using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services
{
    public interface IImageService
    {
        [Multipart]
        [Post("/Image/UploadImage")]
        public Task<string> UploadImage([Authorize("Bearer")] string accessToken, [AliasAs("file")] StreamPart file);
        [Get("/Image/GetProductImages")]
        public Task<IEnumerable<ProductImage>> GetProductImages();
    }
}
