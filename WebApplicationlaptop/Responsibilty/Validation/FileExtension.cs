using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Responsibilty.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;

        public FileExtensionAttribute(string[] allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult($"Only files with extensions {string.Join(", ", _allowedExtensions)} are allowed.");
                }
            }

            return ValidationResult.Success;
        }
    }
}