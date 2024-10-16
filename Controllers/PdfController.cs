using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using SecureFilesJoaoDias.Models;

namespace SecureFilesJoaoDias.Controllers
{
    public class PdfController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new PdfViewModel());
        }

        [HttpPost]
        public IActionResult SecurePdf(PdfViewModel model)
        {
            if (model.PdfFile == null || model.PdfFile.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a valid PDF file.");
                return View("Index", model);
            }

            string inputFilePath = Path.Combine(Path.GetTempPath(), model.PdfFile.FileName);
            string outputFilePath = Path.Combine(Path.GetTempPath(), "secured_" + model.PdfFile.FileName);

            using (var stream = new FileStream(inputFilePath, FileMode.Create))
            {
                model.PdfFile.CopyTo(stream);
            }

            SecurePdf(inputFilePath, outputFilePath, model.OwnerPassword, model.UserPassword);

            var securedPdfBytes = System.IO.File.ReadAllBytes(outputFilePath);
            return File(securedPdfBytes, "application/pdf", "secured_" + model.PdfFile.FileName);
        }

        private void SecurePdf(string inputFile, string outputFile, string ownerPassword, string userPassword)
        {
            using PdfReader pdfReader = new(inputFile);

            byte[]? userPasswordBytes = string.IsNullOrEmpty(userPassword)
                ? null
                : System.Text.Encoding.UTF8.GetBytes(userPassword);

            byte[]? ownerPasswordBytes = string.IsNullOrEmpty(ownerPassword)
                ? null
                : System.Text.Encoding.UTF8.GetBytes(ownerPassword);

            var writerProperties = new WriterProperties()
                .SetStandardEncryption(
                    userPasswordBytes,
                    ownerPasswordBytes,
                    0,
                    EncryptionConstants.ENCRYPTION_AES_256
                );

            using PdfWriter pdfWriter = new(outputFile, writerProperties);
            using PdfDocument pdfDocument = new(pdfReader, pdfWriter);
        }

    }
}
