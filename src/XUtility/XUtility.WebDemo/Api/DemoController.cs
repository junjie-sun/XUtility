using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XUtility.WebDemo.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace XUtility.WebDemo.Api
{
    [Route("api/[controller]")]
    public class DemoController : Controller
    {
        [HttpGet("Test")]
        public string Test()
        {
            return "This is a Test Api.";
        }

        [HttpGet("TestUser")]
        public IActionResult GetTestUser(string param1, string param2)
        {
            return Json(new User() { FirstName = "Michael", LastName = "孙", Param1 = param1, Param2 = param2, Description = Request.Headers["Description"] });
        }

        [HttpPost("PostTestUser")]
        public IActionResult PostTestUser([FromBody]User user, string param1, string param2)
        {
            var res = new ApiResponse<User>()
            {
                Status = 0,
                Message = "Ok",
                Data = new User()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Param1 = param1,
                    Param2 = param2,
                    Description = Request.Headers["Description"]
                }
            };
            return Json(res);
        }

        [HttpPost("FormTestUser")]
        public IActionResult PostFormTestUser([FromForm]User user, string param1, string param2)
        {
            var res = new ApiResponse<User>()
            {
                Status = 0,
                Message = "Ok",
                Data = new User()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Param1 = param1,
                    Param2 = param2,
                    Description = Request.Headers["Description"]
                }
            };
            return Json(res);
        }

        [HttpPost("FormMultipartTestUser")]
        public async Task<IActionResult> PostMultipartFormTestUser([FromForm]User user, [FromForm]ICollection<IFormFile> files, string param1, string param2)
        {
            var uploads = @"d:\";
            //var uploads = "/home/test/target";
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, "form_file_" + file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            var res = new ApiResponse<User>()
            {
                Status = 0,
                Message = "Ok",
                Data = new User()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Param1 = param1,
                    Param2 = param2,
                    Description = Request.Headers["Description"]
                }
            };

            return Json(res);
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile()
        {
            var uploads = @"d:\";
            //var uploads = "/home/test/target";

            if (Request.Headers["Description"] == "Michael's file" && Request.Body != null)
            {
                var fileName = ContentDispositionHeaderValue
                    .Parse(Request.Headers["Content-Disposition"])
                    .FileName
                    .Trim('"');
                using (var fs = new FileStream(Path.Combine(uploads, "upload_" + fileName), FileMode.Create))
                {
                    await Request.Body.CopyToAsync(fs);
                }
            }

            var res = new ApiResponse<User>()
            {
                Status = 0,
                Message = "Ok",
                Data = new User()
                {
                    Param1 = Request.Query["param1"],
                    Param2 = Request.Query["param2"],
                    Description = Request.Headers["Description"]
                }
            };

            return Json(res);
        }

        [HttpGet("DownloadFile")]
        public IActionResult DownloadFile()
        {
            var file1 = @"d:\test1.txt";
            var file2 = @"d:\test2.txt";
            //var file1 = "/home/test/source/test1.txt";
            //var file2 = "/home/test/source/test2.txt";

            if (Request.Headers["Description"] == "Michael's file")
            {
                var file = Request.Query["id"] == "1" ? file1 : file2;

                var fs = new FileStream(file, FileMode.Open);

                return File(fs, "application/octet-stream", Path.GetFileName(file));
            }

            return Forbid();
        }
    }
}
