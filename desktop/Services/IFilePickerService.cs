using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services
{
    public interface IFilePickerService
    {
        public enum Filter
        {
            Image,
            Docx
        }
        public void RegisterProvider(object provider);
        public Task<Stream> OpenFile(Filter filter);
        public Task<Uri> SaveFile(Filter filter);
    }
}
