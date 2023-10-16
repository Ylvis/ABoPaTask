using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using ABoPaTask.API.Classes;
using ABoPaTask.UI.Models;

namespace ABoPaTask.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        private async Task<string> CalculateHashedIPv4()
        {
            var request = _httpClientFactory.CreateClient();
            var requestToIpify = await request.GetAsync("https://api.ipify.org");
            var IPv4 = await requestToIpify.Content.ReadAsStringAsync();

            byte[] inputBytes = Encoding.UTF8.GetBytes(IPv4);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }
        }

        [HttpGet]
        public IActionResult MainMenu() => View();

        [HttpGet]
        public async Task<IActionResult> FirstExperiment()
        {
            int id = 1;
            string IPv4 = await CalculateHashedIPv4();

            var apiClient = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7078/api/Result/PassExperiment1?hash={IPv4}&id={id}&button_color=null";
            var content = await apiClient.GetStringAsync(url);
            ViewBag.Value = content;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FirstExperiment(string button_color)
        {
            int id = 1;
            string IPv4 = await CalculateHashedIPv4();

            var apiClient = _httpClientFactory.CreateClient();
            button_color = new string((char[])button_color.Where(c => c != '#').ToArray());
            var url = $"https://localhost:7078/api/Result/PassExperiment1?hash={IPv4}&id={id}&button_color= + {button_color}";
            var content = await apiClient.GetStringAsync(url);
            ViewBag.Value = content;

            return View("FirstExperiment");
        }

        [HttpGet]
        public async Task<IActionResult> SecondExperiment()
        {
            int id = 2;
            string IPv4 = await CalculateHashedIPv4();

            var apiClient = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7078/api/Result/PassExperiment2?hash={IPv4}&id={id}";
            ViewBag.Value = await apiClient.GetStringAsync(url);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SecondExperiment(string button_price)
        {
            int id = 2;
            string IPv4 = await CalculateHashedIPv4();

            var apiClient = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7078/api/Result/PassExperiment2?hash={IPv4}&id={id}";
            ViewBag.Value = await apiClient.GetStringAsync(url);
            return View();
        }

        [HttpGet]
        public IActionResult StatisticPage() => View();

        [HttpPost]
        public async Task<IActionResult> StatisticPage(string key)
        {
            var apiClient = _httpClientFactory.CreateClient();
            string url = "https://localhost:7078/api/Result/GetByXName/" + key;
            var response = await apiClient.GetAsync(url);
            string experiments = await response.Content.ReadAsStringAsync();

            List<Result> results = new List<Result>();
            results = JsonConvert.DeserializeObject<List<Result>>(experiments);
            ViewBag.Count = results.Count();

            return View(results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}