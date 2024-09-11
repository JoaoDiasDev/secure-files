namespace SecureFilesJoaoDias.Models
{
    public class PdfViewModel
    {
        public IFormFile PdfFile { get; set; } // Uploaded PDF file
        public string OwnerPassword { get; set; } // Owner password for permissions
        public string UserPassword { get; set; } // User password for opening the file
    }
}
