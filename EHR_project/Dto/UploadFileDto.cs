namespace EHR_project.Dto
{
    public class UploadFileDto
    {
        public int UserId{ get; set; }
        public string? ImageName { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
