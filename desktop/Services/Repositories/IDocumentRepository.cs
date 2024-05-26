using desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IDocumentRepository
    {
        public Task GenerateBill(OrderEdit orderEdit);
        public Task GenerateCheck(OrderEdit orderEdit);
        public Task GenerateNakladnaya(OrderEdit orderEdit);
        public Task GeneratePrihod(ReceiptOrderEdit orderEdit);
        public Task GenerateSpisanie(ExpenseOrderEdit orderEdit);
        public Task GenerateListKomplekt(OrderEdit orderEdit);
    }
}
