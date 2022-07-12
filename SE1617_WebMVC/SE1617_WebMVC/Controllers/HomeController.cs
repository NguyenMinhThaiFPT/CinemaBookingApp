using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SE1617_WebMVC.Models;
using System.Diagnostics;
using System.IO;

namespace SE1617_WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(ShowsController.Index), "Shows");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string name, pass;
            var conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            name = conf["User:Name"];
            pass = conf["User:Password"];
            if (name == username && pass == password)
            {
                HttpContext.Session.SetString("username", name);
                HttpContext.Session.SetString("password", pass);
            }
            else return RedirectToAction(nameof(Login));
            /*   // MessageBox.Show($"Name = {name}, Pass = {pass}");
               if (txtName.Text == name && txtPassword.Text == pass)
               {
                   Settings.UserName = name;
                   MessageBox.Show("You are logged in as administrator");
               }
               else
                   MessageBox.Show("Don't have that user");*/
            return RedirectToAction(nameof(ShowsController.Index), "Shows");
        }

    }
}
