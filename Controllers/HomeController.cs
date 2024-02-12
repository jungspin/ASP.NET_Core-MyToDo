using Microsoft.AspNetCore.Mvc;
using MyToDo.Models;
using System.Diagnostics;

namespace MyToDo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public string Sample(string name)
        {
            return $"Hello {name}!";
        }

        public IActionResult Index()
        {
            User sFirstUser = new User();
            sFirstUser.Id = 1;
            sFirstUser.Username = "Tom";

            //User m_sSecondUser = new User()
            //{
            //    No = 2,
            //    Name = "Jane",
            //};

            // 첫번째 방법
            return View(sFirstUser);

            // 두번째 방법
            // ViewBag
            //ViewBag.User = sFirstUser; 

            // 세번째 방법
            //ViewData["user"] = sFirstUser;
            //return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
