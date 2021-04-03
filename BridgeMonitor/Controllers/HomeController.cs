using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BridgeMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BridgeMonitor.Controllers
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
            {
                List<Bateau> apiInfo = GetFromAPI();
                apiInfo.Sort((s1, s2) => DateTimeOffset.Compare(s1.ClosingDate, s2.ClosingDate));

                foreach (var ferme1 in apiInfo)
                {
                    if (DateTimeOffset.Compare(DateTimeOffset.Now, ferme1.ClosingDate) < 0)
                    {
                        ViewData["Info0"] = ferme1;
                        break;
                    }
                }

                return View();
            }
        }

        public IActionResult Fermeturepont()
        {
            ViewData["Boats"] = GetFromAPI();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static List<Bateau> GetFromAPI()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("https://api.alexandredubois.com/pont-chaban/api.php");
                var stringResult = response.Result.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Bateau>>(stringResult.Result);
                return result;

            }
        }
    }
}