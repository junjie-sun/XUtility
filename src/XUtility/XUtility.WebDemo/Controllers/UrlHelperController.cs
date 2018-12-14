using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XUtility.Web.Extensions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace XUtility.WebDemo.Controllers
{
    public class UrlHelperController : Controller
    {
        public IActionResult AbsoluteAction()
        {
            return new ContentResult() { Content = Url.AbsoluteAction("AbsoluteAction", "UrlHelper") };
        }

        public IActionResult AbsoluteContent()
        {
            return new ContentResult() { Content = Url.AbsoluteContent("UrlHelper/AbsoluteContent") };
        }
    }
}
