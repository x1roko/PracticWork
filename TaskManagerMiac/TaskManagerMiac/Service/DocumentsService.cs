using DinkToPdf;
using DinkToPdf.Contracts;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.EntityFrameworkCore;
using OpenXmlPowerTools;
using System.IO.Packaging;
using System.Xml.Linq;
using TaskManagerMiac.Data;
using TaskManagerMiac.Models;

namespace TaskManagerMiac.Service
{
    /// <summary>
    /// Сервис, созданный для работы с документами
    /// </summary>
    public class DocumentsService
    {
        private readonly IConverter _converter;
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;
        private string _path;
        private string _pathDocuments;

        public DocumentsService(AppDbContext context, IConverter converter, IWebHostEnvironment environment)
        {
            _context = context;
            _converter = converter;
            _environment = environment;
            _path = Path.Combine(_environment.WebRootPath, "uploads");
            _pathDocuments = Path.Combine(_environment.WebRootPath, "documents", "HTML");
        }
        /// <summary>
        /// В данный момент не используемый метод, отвечающий за конвертация ворд документа в HTML
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// HTML документ в строке
        /// </returns>
        private string ConvertToHTML(string path)
        {
            var source = Package.Open(path);
            var document = WordprocessingDocument.Open(source);
            HtmlConverterSettings settings = new HtmlConverterSettings();
            XElement html = HtmlConverter.ConvertToHtml(document, settings);
            document.Close();
            return html.ToString();
        }
        /// <summary>
        /// Метод, создающий ПДФ файл для таска на основе переданного документа
        /// </summary>
        /// <param name="idTaskBody">Id заявки</param>
        /// <param name="document">Объект документа</param>
        /// <returns>
        /// true - успешно
        /// false - неуспешно
        /// </returns>
        public async Task<bool> GeneratePDFToTaskAsync(int idTaskBody, Interfaces.IDocument document)
        {
            var taskBody = await _context.TaskBodies
                .Include(tb => tb.IdUserCreatorNavigation)
                    .ThenInclude(u => u.IdGroupNavigation)
                .FirstOrDefaultAsync(tb => tb.IdTask == idTaskBody);
            if (taskBody == null)
                return false;
            var documentName = document.DocumentName;
            // var inputPath = $"{Directory.GetCurrentDirectory()}/Documents/HTML/{documentName}"; // Откуда берём базовый HTML
            //var outputPath = $"{Directory.GetCurrentDirectory()}/uploads"; // куда будем выгружать
            var inputPath = Path.Combine(_pathDocuments,documentName);
            var outputPath = _path; // куда будем выгружать
            if (!File.Exists(inputPath))
            {
                return false;
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var htmlText = await File.ReadAllTextAsync(inputPath);
            var parameters = document.GetParameters();
            foreach (var item in parameters) // Key - переменная в документе, Value - значение на которое заменяем
            {
                htmlText = htmlText.Replace(item.Key, item.Value);
            }
            htmlText = htmlText.Replace("FULLNAME", taskBody.IdUserCreatorNavigation.FullName);
            htmlText = htmlText.Replace("GROUP", taskBody.IdUserCreatorNavigation.IdGroupNavigation.Title);
            htmlText = htmlText.Replace("DATE", DateTime.Now.ToString("D"));
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 }
                 },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = htmlText,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    },
                }
            };
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                Document documentDB = new Document
                {
                    Title = Path.GetFileNameWithoutExtension(documentName),
                    Extension = ".pdf"
                };
                document.IdDocumentNavigation = documentDB;
                _context.Update(document);
                taskBody.Documents.Add(documentDB);
                await _context.SaveChangesAsync();
                doc.GlobalSettings.Out = Path.Combine(outputPath, $"{documentDB.IdDocument}.pdf");
                _converter.Convert(doc);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsService - GeneratePDFToTaskAsync - {ex.Message}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Метод для обновления информации в PDF документе
        /// </summary>
        /// <param name="idDocument">Id документа</param>
        /// <param name="document">Объект документа</param>
        /// <param name="idTaskBody">Id заявки</param>
        /// <returns>
        /// true - успешно
        /// false - неуспешно
        /// </returns>
        public async Task<bool> UpdatePDFToTaskAsync(int idDocument, Interfaces.IDocument document, int idTaskBody)
        {
            var taskBody = await _context.TaskBodies
                .Include(tb => tb.IdUserCreatorNavigation)
                    .ThenInclude(u => u.IdGroupNavigation)
                .FirstOrDefaultAsync(tb => tb.IdTask == idTaskBody);
            if (taskBody == null)
                return false;
            var documentName = document.DocumentName;
            // var inputPath = $"{Directory.GetCurrentDirectory()}/Documents/HTML/{documentName}";
            //var outputPath = $"{Directory.GetCurrentDirectory()}/uploads";
            var inputPath = Path.Combine(_pathDocuments, documentName);
            var outputPath = _path;
            var htmlText = await File.ReadAllTextAsync(inputPath);
            var parameters = document.GetParameters();
            foreach (var item in parameters) // Key - переменная в документе, Value - значение на которое заменяем
            {
                htmlText = htmlText.Replace(item.Key, item.Value);
            }
            htmlText = htmlText.Replace("FULLNAME", taskBody.IdUserCreatorNavigation.FullName);
            htmlText = htmlText.Replace("GROUP", taskBody.IdUserCreatorNavigation.IdGroupNavigation.Title);
            htmlText = htmlText.Replace("DATE", taskBody.CreationDate.ToString("D"));
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 }
                 },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = htmlText,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    },
                }
            };
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                _context.Update(document);
                await _context.SaveChangesAsync();
                doc.GlobalSettings.Out = Path.Combine(outputPath, $"{idDocument}.pdf");
                _converter.Convert(doc);
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsService - UpdatePDFToTaskAsync - {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Метод для загрузки файла для таска на сервер
        /// </summary>
        /// <param name="idTaskBody"></param>
        /// <param name="file"></param>
        /// <returns>
        /// Document - возвращает загруженный документ
        /// null - произошла ошибка при загрузке документа
        /// </returns>
        public async Task<Document?> LoadFile(int idTaskBody, IFormFile file)
        {
            //var uploadPath = $"{Directory.GetCurrentDirectory()}/uploads";
            var uploadPath = _path;
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var ext = Path.GetExtension(file.FileName);
                Document document = new Document
                {
                    Title = Path.GetFileNameWithoutExtension(file.FileName),
                    Extension = ext
                };
                var taskBody = await _context.TaskBodies.FindAsync(idTaskBody);
                if (taskBody == null)
                    return null;
                taskBody.Documents.Add(document);
                await _context.SaveChangesAsync();
                string fullPath = $"{uploadPath}/{document.IdDocument}{ext}";
                // сохраняем файл в папку uploads
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                transaction.Commit();
                return document;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsService - LoadFile - {ex.Message}");
                return null;
            }
        }
    }
}
