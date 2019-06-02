using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnet.Models;
using ServiceStack.Redis;
using Microsoft.Extensions.Logging;

namespace aspnet.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            try
            {
                var ip = Environment.GetEnvironmentVariable("redisip");
                _logger.LogCritical("ip = " + ip);
                var manager = new RedisManagerPool($"{ip}:6379");
                _logger.LogCritical("After RedisManagerPool");
                
                using (var client = manager.GetClient())
                {
                    _logger.LogCritical("After GetClient");

                    client.Set("foo", "bar");

                    ViewData["cachedvalue"] = client.Get<string>("foo");

                    _logger.LogCritical("foo={0}", client.Get<string>("foo"));
                   
                    _logger.LogCritical("Set/Get Succeeded");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception: {ex.Message + Environment.NewLine + ex.StackTrace}");
            }
            
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
    }
}
