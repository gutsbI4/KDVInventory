using desktop.Models;
using desktop.Services.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateEngine.Docx;
using Tmds.DBus.Protocol;

namespace desktop.Services
{
    public class DocumentService : IDocumentRepository
    {
        private readonly IFilePickerService _filePickerService;
        private readonly IDialogService _dialogService;
        public DocumentService(IFilePickerService filePickerService,IDialogService dialogService)
        {
            _filePickerService = filePickerService;
            _dialogService = dialogService;
        }
        public async Task GenerateBill(OrderEdit order)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = Path.Combine("DocumentTemplate", "BillTemplate.docx");

                string templatePath = Path.Combine(baseDirectory, relativePath);

                if (!File.Exists(templatePath))
                {
                    Console.WriteLine("Шаблон не найден по пути: " + templatePath);
                    return;
                }
                Uri document = await _filePickerService.SaveFile(IFilePickerService.Filter.Docx);
                if (document == null) return;
                if (File.Exists(document.LocalPath))
                {
                    File.Delete(document.LocalPath);
                }
                File.Copy(templatePath, document.LocalPath);
                string fileCompany = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json");
                string jsonCompany = File.ReadAllText(fileCompany);

                dynamic data = JsonConvert.DeserializeObject<dynamic>(jsonCompany);
                var companyName = (string)data.CompanyInformation.CompanyName;
                var address = (string)data.CompanyInformation.Address;
                var inn = (string)data.CompanyInformation.INN;
                var kpp = (string)data.CompanyInformation.KPP;
                var ogrnip = (string)data.CompanyInformation.OGRNIP;
                var phone = (string)data.CompanyInformation.Phone;
                var facs = (string)data.CompanyInformation.Facs;
                var raschet = (string)data.CompanyInformation.RacChet;
                var bank = (string)data.CompanyInformation.Bank;
                var kadchet = (string)data.CompanyInformation.KorChet;
                var bik = (string)data.CompanyInformation.Bik;

                
                TableContent tableContent = new TableContent("TableProduct");
                int numberPosistion = 0;
                foreach (var product in order.OrderProduct)
                {
                    numberPosistion++;
                    tableContent.AddRow(
                    new FieldContent("NumberPosition", numberPosistion.ToString()),
                    new FieldContent("NameProduct", product.Product.Name),
                    new FieldContent("QuantityProduct", product.Quantity.ToString()),
                    new FieldContent("PriceProduct", product.Price.ToString("F2")),
                    new FieldContent("SumProduct", product.Sum.ToString("F2"))
                );
                    
                }
                var valuesToFill = new Content(
                        new FieldContent("CompanyName", companyName),
                        new FieldContent("Address", address),
                        new FieldContent("INN", inn),
                        new FieldContent("KPP", kpp),
                        new FieldContent("OGRNIP", ogrnip),
                        new FieldContent("Phone", phone),
                        new FieldContent("Facs", facs),
                        new FieldContent("RasChet", raschet),
                        new FieldContent("Bank", bank),
                        new FieldContent("KadChet", kadchet),
                        new FieldContent("BIK", bik),
                        new FieldContent("AddressKlient", order.Address),
                        new FieldContent("IDZakaz", order.Id.ToString()),
                        new FieldContent("DateZakaz", order.DateOfOrder),
                        new FieldContent("Total", order.Total.ToString("N2")),
                        new FieldContent("PriceToText", DecimalToText(order.Total)),
                        new FieldContent("CountPosition",numberPosistion.ToString())
                    );
                valuesToFill.Tables.Add(tableContent);
                using (var outputDocument = new TemplateProcessor(document.LocalPath)
                    .SetRemoveContentControls(true))
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();
                    
                }
                OpenFile(document.LocalPath);
            }
            catch (Exception e)
            {

                throw;
            }
           
        }
        public async Task GenerateCheck(OrderEdit order)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = Path.Combine("DocumentTemplate", "check.docx");

                string templatePath = Path.Combine(baseDirectory, relativePath);

                if (!File.Exists(templatePath))
                {
                    Console.WriteLine("Шаблон не найден по пути: " + templatePath);
                    return;
                }
                Uri document = await _filePickerService.SaveFile(IFilePickerService.Filter.Docx);
                if (document == null) return;
                if (File.Exists(document.LocalPath))
                {
                    File.Delete(document.LocalPath);
                }
                File.Copy(templatePath, document.LocalPath);
                string fileCompany = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json");
                string jsonCompany = File.ReadAllText(fileCompany);

                dynamic data = JsonConvert.DeserializeObject<dynamic>(jsonCompany);
                var inn = (string)data.CompanyInformation.INN;

                var valuesToFill = new Content(
                        new FieldContent("INN", inn),
                        new FieldContent("Employee", order.Employee),
                        new FieldContent("OrderNumber", order.Id.ToString()),
                        new FieldContent("DateOfOrder", order.DateOfOrder),
                        new FieldContent("Total", order.Total.ToString("N2")),
                        new FieldContent("TextPrice", DecimalToText(order.Total)),
                        new FieldContent("DateTimeNow", DateTime.Now.ToLocalTime().ToString("dd.MM.yy HH:mm")),
                        new FieldContent("AddressKlient", order.Address)
                    );
                TableContent tableContent = new TableContent("ProductTable");
                foreach (var product in order.OrderProduct)
                {
                    tableContent.AddRow(
                    new FieldContent("ProductName", product.Product.Name),
                    new FieldContent("Quantity", product.Quantity.ToString()),
                    new FieldContent("Price", product.Price.ToString("F2")),
                    new FieldContent("Sum", product.Sum.ToString("F2"))
                );
                }
                valuesToFill.Tables.Add(tableContent);
                using (var outputDocument = new TemplateProcessor(document.LocalPath)
                    .SetRemoveContentControls(true))
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();

                }
                OpenFile(document.LocalPath);
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task GenerateNakladnaya(OrderEdit order)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = Path.Combine("DocumentTemplate", "Nakladnaya.docx");

                string templatePath = Path.Combine(baseDirectory, relativePath);

                if (!File.Exists(templatePath))
                {
                    Console.WriteLine("Шаблон не найден по пути: " + templatePath);
                    return;
                }
                Uri document = await _filePickerService.SaveFile(IFilePickerService.Filter.Docx);
                if (document == null) return;
                if (File.Exists(document.LocalPath))
                {
                    File.Delete(document.LocalPath);
                }
                File.Copy(templatePath, document.LocalPath);
                string fileCompany = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json");
                string jsonCompany = File.ReadAllText(fileCompany);

                dynamic data = JsonConvert.DeserializeObject<dynamic>(jsonCompany);
                var inn = (string)data.CompanyInformation.INN;

                
                TableContent tableContent = new TableContent("TableProduct");
                int numberPosistion = 0;
                foreach (var product in order.OrderProduct)
                {
                    numberPosistion++;
                    tableContent.AddRow(
                    new FieldContent("NumberPosition", numberPosistion.ToString()),
                    new FieldContent("NameProduct", product.Product.Name),
                    new FieldContent("Articul", product.Product.ProductNumber),
                    new FieldContent("Quantity", product.Quantity.ToString()),
                    new FieldContent("Price", product.Price.ToString("F2")),
                    new FieldContent("Sum", product.Sum.ToString("F2"))
                );
                    
                }
                var valuesToFill = new Content(
                        new FieldContent("INN", inn),
                        new FieldContent("AddressKlient", order.Address),
                        new FieldContent("OrderID", order.Id.ToString()),
                        new FieldContent("DateZakaz", order.DateOfOrder),
                        new FieldContent("Total", order.Total.ToString("N2")),
                        new FieldContent("TotalPosition",numberPosistion.ToString())
                    );
                valuesToFill.Tables.Add(tableContent);
                using (var outputDocument = new TemplateProcessor(document.LocalPath)
                    .SetRemoveContentControls(true))
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();

                }
                OpenFile(document.LocalPath);
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task GeneratePrihod(ReceiptOrderEdit order)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = Path.Combine("DocumentTemplate", "Prihod.docx");

                string templatePath = Path.Combine(baseDirectory, relativePath);

                if (!File.Exists(templatePath))
                {
                    Console.WriteLine("Шаблон не найден по пути: " + templatePath);
                    return;
                }
                Uri document = await _filePickerService.SaveFile(IFilePickerService.Filter.Docx);
                if (document == null) return;
                if (File.Exists(document.LocalPath))
                {
                    File.Delete(document.LocalPath);
                }
                File.Copy(templatePath, document.LocalPath);
                string fileCompany = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json");
                string jsonCompany = File.ReadAllText(fileCompany);

                dynamic data = JsonConvert.DeserializeObject<dynamic>(jsonCompany);
                var inn = (string)data.CompanyInformation.INN;


                TableContent tableContent = new TableContent("TableProduct");
                int numberPosistion = 0;
                foreach (var product in order.ReceiptOrderProduct)
                {
                    numberPosistion++;
                    tableContent.AddRow(
                    new FieldContent("NumberPosition", numberPosistion.ToString()),
                    new FieldContent("ProductName", product.Product.Name),
                    new FieldContent("Articul", product.Product.ProductNumber),
                    new FieldContent("Quantity", product.Quantity.ToString()),
                    new FieldContent("Price", product.PurchasePrice.ToString("F2")),
                    new FieldContent("Sum", product.Sum.ToString("F2"))
                );
                    
                }
                var valuesToFill = new Content(
                        new FieldContent("INN", inn),
                        new FieldContent("PrixodID", order.Id.ToString()),
                        new FieldContent("DatePrixod", order.DateOfCreate),
                        new FieldContent("Total", order.Total.ToString("N2")),
                        new FieldContent("TotalPosition", numberPosistion.ToString())
                    );
                valuesToFill.Tables.Add(tableContent);
                using (var outputDocument = new TemplateProcessor(document.LocalPath)
                    .SetRemoveContentControls(true))
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();

                }
                OpenFile(document.LocalPath);
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task GenerateSpisanie(ExpenseOrderEdit order)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = Path.Combine("DocumentTemplate", "Spisanie.docx");

                string templatePath = Path.Combine(baseDirectory, relativePath);

                if (!File.Exists(templatePath))
                {
                    Console.WriteLine("Шаблон не найден по пути: " + templatePath);
                    return;
                }
                Uri document = await _filePickerService.SaveFile(IFilePickerService.Filter.Docx);
                if (document == null) return;
                if (File.Exists(document.LocalPath))
                {
                    File.Delete(document.LocalPath);
                }
                File.Copy(templatePath, document.LocalPath);
                string fileCompany = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json");
                string jsonCompany = File.ReadAllText(fileCompany);

                dynamic data = JsonConvert.DeserializeObject<dynamic>(jsonCompany);
                var inn = (string)data.CompanyInformation.INN;


                TableContent tableContent = new TableContent("TableProduct");
                int numberPosistion = 0;
                foreach (var product in order.ExpenseOrderProduct)
                {
                    numberPosistion++;
                    tableContent.AddRow(
                    new FieldContent("NumberPosition", numberPosistion.ToString()),
                    new FieldContent("ProductName", product.Product.Name),
                    new FieldContent("Articul", product.Product.ProductNumber),
                    new FieldContent("Quantity", product.Quantity.ToString()),
                    new FieldContent("Price", product.Price.ToString("F2")),
                    new FieldContent("Sum", product.Sum.ToString("F2"))
                );
                    
                }
                var valuesToFill = new Content(
                        new FieldContent("INN", inn),
                        new FieldContent("SpisanieID", order.Id.ToString()),
                        new FieldContent("DateSpisanie", order.DateOfCreate),
                        new FieldContent("Total", order.Total.ToString("N2")),
                        new FieldContent("TotalPosition", numberPosistion.ToString())
                    );
                valuesToFill.Tables.Add(tableContent);
                using (var outputDocument = new TemplateProcessor(document.LocalPath)
                    .SetRemoveContentControls(true))
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();

                }
                OpenFile(document.LocalPath);
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task GenerateListKomplekt(OrderEdit order)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = Path.Combine("DocumentTemplate", "ListKomplekt.docx");

                string templatePath = Path.Combine(baseDirectory, relativePath);

                if (!File.Exists(templatePath))
                {
                    Console.WriteLine("Шаблон не найден по пути: " + templatePath);
                    return;
                }
                Uri document = await _filePickerService.SaveFile(IFilePickerService.Filter.Docx);
                if (document == null) return;
                if (File.Exists(document.LocalPath))
                {
                    File.Delete(document.LocalPath);
                }
                File.Copy(templatePath, document.LocalPath);


                TableContent tableContent = new TableContent("TableProduct");
                int numberPosistion = 0;
                foreach (var product in order.OrderProduct)
                {
                    numberPosistion++;
                    tableContent.AddRow(
                    new FieldContent("NumberPosition", numberPosistion.ToString()),
                    new FieldContent("ProductName", product.Product.Name),
                    new FieldContent("Quantity", product.Quantity.ToString())
                );

                }
                var valuesToFill = new Content();
                valuesToFill.Tables.Add(tableContent);
                using (var outputDocument = new TemplateProcessor(document.LocalPath)
                    .SetRemoveContentControls(true))
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();

                }
                OpenFile(document.LocalPath);
            }
            catch (Exception e)
            {

                throw;
            }

        }
        private string DecimalToText(decimal number)
        {
            if (number == 0)
                return "ноль рублей 00 копеек";

            string[] unitMale = { "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
            string[] unitFemale = { "", "одна", "две", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
            string[] teens = { "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };
            string[] tens = { "", "десять", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };
            string[] hundreds = { "", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };
            string[] thousands = { "", "тысяча", "тысячи", "тысяч" };
            string[] millions = { "", "миллион", "миллиона", "миллионов" };
            string[] billions = { "", "миллиард", "миллиарда", "миллиардов" };


            string[] parts = number.ToString().Split('.');
            long rubles = long.Parse(parts[0]);
            int kopecks = int.Parse(parts[1]);

            List<string> words = new List<string>();

            void TranslateGroup(long num, string[] forms, string[] units)
            {
                if (num > 0)
                {
                    int h = (int)((num % 1000) / 100);
                    int t = (int)((num % 100) / 10);
                    int u = (int)(num % 10);
                    int g = (int)(num % 100);

                    if (h > 0)
                        words.Add(hundreds[h]);

                    if (g > 10 && g < 20)
                    {
                        words.Add(teens[g - 10]);
                    }
                    else
                    {
                        if (t > 0)
                            words.Add(tens[t]);
                        if (u > 0)
                            words.Add(units[u]);
                    }

                    words.Add(forms[GetPluralIndex(num)]);
                }
            }

            int GetPluralIndex(long num)
            {
                int n = (int)(num % 100);
                if (n >= 11 && n <= 19)
                    return 3;

                switch (n % 10)
                {
                    case 1: return 1;
                    case 2:
                    case 3:
                    case 4: return 2;
                    default: return 3;
                }
            }

            void SplitNumber(long num, string[] forms, string[] units)
            {
                if (num >= 1_000_000_000)
                {
                    TranslateGroup((num / 1_000_000_000) % 1_000, billions, unitMale);
                    num %= 1_000_000_000;
                }
                if (num >= 1_000_000)
                {
                    TranslateGroup((num / 1_000_000) % 1_000, millions, unitMale);
                    num %= 1_000_000;
                }
                if (num >= 1_000)
                {
                    TranslateGroup((num / 1_000) % 1_000, thousands, unitFemale);
                    num %= 1_000;
                }
                TranslateGroup(num, new[] { "", "рубль", "рубля", "рублей" }, unitMale);
            }

            SplitNumber(rubles, new[] { "рубль", "рубля", "рублей" }, unitMale);

            if (kopecks > 0)
            {
                TranslateGroup(kopecks, new[] { "", "копейка", "копейки", "копеек" }, unitFemale);
            }
            else
            {
                words.Add("00 копеек");
            }

            string result = string.Join(" ", words);
            return result.Trim();
        }
        private void OpenFile(string filePath)
        {
            _dialogService.ShowDialog("Создание документа", "Документ успешно сохранен",IDialogService.DialogType.Standard);
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(filePath);
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось открыть файл: " + ex.Message);
            }
        }

    }
}
