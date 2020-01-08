using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.FileWeb.External
{
    public class TestController :Controller
    {
        public IActionResult Test()
        {
            return new JsonResult("Hello World");
        }
    }
}
