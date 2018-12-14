using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using XUtility.Http;
using Newtonsoft.Json;
using XUtility.WebDemo.Models;
using System.IO;
using System.Net.Http;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace XUtility.WebDemo.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.ClientIP = HttpContext.Connection.RemoteIpAddress.ToString();
            return View();
        }

        public async Task<IActionResult> ApiGetTest()
        {
            var url = "http://localhost:4093/api/demo/testuser";
            //var url = "http://localhost:5000/api/demo/testuser";

            var paramList = new Dictionary<string, string>();
            paramList.Add("param1", "参数C++");
            paramList.Add("param2", "参数C#");
            var str = await HttpRequestUtils.GetAsync(url, paramList, null, new Action<HttpClient>((client)=> 
            {
                client.DefaultRequestHeaders.Add("Description", "This is a get demo.");
            }));
            var user = JsonConvert.DeserializeObject<User>(str);

            return Content(string.Format("FirstName={0}, LastName={1}, Param1={2}, Param2={3}, Description={4}", user.FirstName, user.LastName, user.Param1, user.Param2, user.Description));
        }

        public async Task<IActionResult> ApiPostTest()
        {
            var url = "http://localhost:4093/api/demo/PostTestUser";
            //var url = "http://localhost:5000/api/demo/PostTestUser";

            var paramList = new Dictionary<string, string>();
            paramList.Add("param1", "参数C++");
            paramList.Add("param2", "参数F#");
            var user = new User()
            {
                FirstName = "Michael",
                LastName = "Sun"
            };
            var body = JsonConvert.SerializeObject(user);
            var str = await HttpRequestUtils.PostAsync(url, body, paramList, null, new Action<HttpClient>((client) =>
            {
                client.DefaultRequestHeaders.Add("Description", "This is a post demo.");
            }));
            var res = JsonConvert.DeserializeObject<ApiResponse<User>>(str);

            return Content(string.Format("Status={0}, Message={1}, FirstName={2}, LastName={3}, Param1={4}, Param2={5}, Description={6}",
                res.Status, res.Message, res.Data.FirstName, res.Data.LastName, res.Data.Param1, res.Data.Param2, res.Data.Description));
        }

        public async Task<IActionResult> ApiPostFormTest()
        {
            var url = "http://localhost:4093/api/demo/FormTestUser";
            //var url = "http://localhost:5000/api/demo/FormTestUser";

            var paramList = new Dictionary<string, string>();
            paramList.Add("param1", "参数C++");
            paramList.Add("param2", "参数F#");

            var form = new Dictionary<string, string>();
            form.Add("firstname", "吉姆++");
            form.Add("lastname", "雷诺#");

            var str = await HttpRequestUtils.PostFormAsync(url, form, paramList, null, new Action<HttpClient>((client) =>
            {
                client.DefaultRequestHeaders.Add("Description", "This is a post form demo.");
            }));
            var res = JsonConvert.DeserializeObject<ApiResponse<User>>(str);

            return Content(string.Format("Status={0}, Message={1}, FirstName={2}, LastName={3}, Param1={4}, Param2={5}, Description={6}",
                res.Status, res.Message, res.Data.FirstName, res.Data.LastName, res.Data.Param1, res.Data.Param2, res.Data.Description));
        }

        public async Task<IActionResult> ApiPostMultipartFormTest()
        {
            var url = "http://localhost:4093/api/demo/FormMultipartTestUser";
            //var url = "http://localhost:5000/api/demo/FormMultipartTestUser";

            var paramList = new Dictionary<string, string>();
            paramList.Add("param1", "参数C++");
            paramList.Add("param2", "参数F#");

            var form = new Dictionary<string, string>();
            form.Add("firstname", "吉姆++");
            form.Add("lastname", "雷诺#");

            var fileList = new List<string>();
            fileList.Add(@"d:\test1.txt");
            fileList.Add(@"d:\test2.txt");
            //fileList.Add(@"/home/test/source/test1.txt");
            //fileList.Add(@"/home/test/source/test2.txt");

            var str = await HttpRequestUtils.PostMultipartFormAsync(url, form, fileList, "files", paramList, null, new Action<HttpClient>((client) =>
            {
                client.DefaultRequestHeaders.Add("Description", "This is a post multipart form demo.");
            }));
            var res = JsonConvert.DeserializeObject<ApiResponse<User>>(str);

            return Content(string.Format("Status={0}, Message={1}, FirstName={2}, LastName={3}, Param1={4}, Param2={5}, Description={6}",
                res.Status, res.Message, res.Data.FirstName, res.Data.LastName, res.Data.Param1, res.Data.Param2, res.Data.Description));
        }

        public async Task<IActionResult> ApiPostUploadFileTest()
        {
            var url = "http://localhost:4093/api/demo/UploadFile";
            //var url = "http://localhost:5000/api/demo/UploadFile";

            var paramList = new Dictionary<string, string>();
            paramList.Add("param1", "参数C++");
            paramList.Add("param2", "参数F#");

            var file = @"d:\test1.txt";
            //var file = "/home/test/source/test1.txt";

            var str = await HttpRequestUtils.PostFileAsync(url, file, paramList, null, new Action<HttpClient>((client) =>
            {
                client.DefaultRequestHeaders.Add("Description", "Michael's file");
            }));
            var res = JsonConvert.DeserializeObject<ApiResponse<User>>(str);

            return Content(string.Format("Status={0}, Message={1}, Param1={2}, Param2={3}, Description={4}",
                res.Status, res.Message, res.Data.Param1, res.Data.Param2, res.Data.Description));
        }

        public async Task<IActionResult> ApiDownloadFileTest()
        {
            var url = "http://localhost:4093/api/demo/DownloadFile";
            //var url = "http://localhost:5000/api/demo/DownloadFile";
            var filePath = @"d:\";
            //var filePath = "/home/test/target";

            var paramList = new Dictionary<string, string>();
            paramList.Add("id", Request.Query["id"]);
            var fileName = await HttpRequestUtils.DownloadAsync(url, filePath, paramList, "abc.tmp", new Action<HttpClient>((client) =>
            {
                client.DefaultRequestHeaders.Add("Description", "Michael's file");
            }));
            return Content(string.Format(fileName));
        }
    }
}
