using CircuitBreaker.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace CircuitBreaker.Client.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        #region Properties
        private string _serverIndexUrl = "http://localhost:5122";
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly AsyncFallbackPolicy<HttpResponseMessage> _fallbackPolicy;
        private static Polly.CircuitBreaker.AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreaker =
            Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).Or<HttpRequestException>()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(10),
                (d, c) => {
                    string a = "Break";
                },
                () => {
                    string a = "Reset";
                },
                () => {
                    string a = "Half";
                });
        #endregion

        public HomeController()
        {
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .RetryAsync(5, (d, c) => {
                    string a = "Retry";
                });

            _fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .Or<BrokenCircuitException>()
                .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new ObjectContent(typeof(Message), new Message
                    {
                        Id = 100,
                        Text = "FallBack... Default Text..."
                    }, new JsonMediaTypeFormatter())
                });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            var result = await client.GetAsync(_serverIndexUrl + "/Home/Odd");
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ClientMessage>(str);
            return Ok(obj);
        }

        [HttpGet]
        public async Task<IActionResult> Index1()
        {
            HttpClient client = new HttpClient();
            var result = await _fallbackPolicy.ExecuteAsync(() => client.GetAsync(_serverIndexUrl+ "/Home/Odd"));
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ClientMessage>(str);
            return Ok(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Index2()
        {
            HttpClient client = new HttpClient();
            var result = await client.GetAsync(_serverIndexUrl + "/Home/Ten");
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ClientMessage>(str);
            return Ok(obj);
        }

        [HttpGet]
        public async Task<IActionResult> Index3()
        {
            HttpClient client = new HttpClient();
            var result = await _fallbackPolicy.ExecuteAsync(() => client.GetAsync(_serverIndexUrl+"/Home/Ten"));
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ClientMessage>(str);
            return Ok(obj);
        }

        [HttpGet]
        public async Task<IActionResult> Index4()
        {
            HttpClient client = new HttpClient();
            var result = await _fallbackPolicy.ExecuteAsync(() => _retryPolicy.ExecuteAsync(() => client.GetAsync(_serverIndexUrl + "/Home/Ten")));
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ClientMessage>(str);
            return Ok(obj);
        }

        [HttpGet]
        public async Task<IActionResult> Index5()
        {
            HttpClient client = new HttpClient();
            var result = await _fallbackPolicy.ExecuteAsync(() => _retryPolicy.ExecuteAsync(() => _circuitBreaker.ExecuteAsync(() => client.GetAsync(_serverIndexUrl + "/Home/Ten"))));
            var str = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ClientMessage>(str);
            return Ok(obj);
        }
    }
}
