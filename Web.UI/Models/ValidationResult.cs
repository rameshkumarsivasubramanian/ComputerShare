namespace Web.UI.Models
{
    public class ValidationResult
    {
        public bool isValid { get; set; }
        public string message { get; set; }

        public ValidationResult()
        {
            isValid = false;
            message = "Yet to validate";
        }
    }
}
