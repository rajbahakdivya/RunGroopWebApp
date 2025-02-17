namespace RunGroopWebApp.Models
{
    public class ImageUploadResult
    {
        public string SecureUrl { get; set; }
        public string PublicId { get; set; }
        public object Url { get; internal set; }
    }
}
