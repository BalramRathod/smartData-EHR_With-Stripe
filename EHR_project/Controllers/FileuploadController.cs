using Dapper;
using EHR_project.Data;
using EHR_project.Dto;
using EHR_project.encryption;

using EHR_project.Models;
using Microsoft.AspNetCore.Mvc;

namespace EHR_project.Controllers
{
    public class FileuploadController : Controller
    {

        private readonly DapperContext _dapperContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileuploadController(DapperContext dapperContext, IWebHostEnvironment webHostEnvironment)
        {
            _dapperContext = dapperContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("uploadProfile")]
        public async Task<ActionResult> UploadImage([FromForm] UploadFileDto profile)
        {
            try
            {
                var query = "";
                var connection = _dapperContext.CreateConnection();
                Upload obj = new Upload(_webHostEnvironment);

                if (profile.ImageFile != null)
                {
                    query = "Select * from Users where UserId = @UserId";
                    var user = await connection.QueryFirstOrDefaultAsync<Users>(query, new { UserId = profile.UserId });

                    var fileResult = obj.SaveImage(user.UserId, profile.ImageName, profile.ImageFile);



                    query = "Update Users set Profile_Path = @path where UserId = @UserId";

                    await connection.ExecuteAsync(query, new { path = fileResult, UserId = profile.UserId });

                    return Ok(new
                    {
                        message = "Success"
                    });

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Ok(new
            {
                message = "Failed"
            });

        }
    }
}

