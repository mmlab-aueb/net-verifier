using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net_verifier.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace net_verifier.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

    }
}
