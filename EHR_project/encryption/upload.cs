namespace EHR_project.encryption
{
    public class Upload
    {

        private readonly IWebHostEnvironment _environment;

        public Upload(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public string SaveImage(int id, string name, IFormFile imageFile)
        {
            try
            {
                var contentPath = this._environment.WebRootPath;
                var path = Path.Combine(contentPath, "Profile");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var newFileName = id + name;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();
                return GetFilePath(newFileName);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Failed";
            }
        }


        private string GetFilePath(string name)
        {
            return "https://localhost:7015/" + "/Profile/" + name;
        }
    }
}
