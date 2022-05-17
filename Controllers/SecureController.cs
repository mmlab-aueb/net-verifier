using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace net_verifier.Controllers
{
    public class SecureController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Policy = "VCAuthorization")]
        public IActionResult JWT_VC()
        {
            return View();
        }

        [Authorize(Policy = "DPoPAuthorization")]
        public IActionResult JWT_VC_DPoP()
        {
            return View();
        }
    }
}
