using CircuitBreaker.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CircuitBreaker.Client.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        #region Properties
        private string _serverIndexUrl = "http://localhost:5122/Home/Index";
        #endregion

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            var result = await client.GetAsync(_serverIndexUrl);
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ClientMessage>(str);
            return Ok(obj);
        }
    }
}
