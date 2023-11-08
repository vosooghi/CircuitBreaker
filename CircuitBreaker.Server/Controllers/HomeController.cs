using CircuitBreaker.Model;
using Microsoft.AspNetCore.Mvc;

namespace CircuitBreaker.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        static int counter = 0;
        public IActionResult Index()
        {
            return Ok(new Message
            { Id = 1, Text = "Request is successfully done." });
        }

        [HttpGet]
        public IActionResult Odd()
        {
            counter++;
            if (counter % 2 != 0)
            {
                return Ok(
                new Message { Id = counter, Text = "Request is successfully done in odd numbers." }
                );
            }
            return BadRequest(new Message { Id = counter, Text = "It is even number, request rejected." });
        }

        [HttpGet]
        public IActionResult Ten()
        {
            counter++;
            if(counter % 10 == 0) {
                return Ok(new Message
                {
                    Id = counter,
                    Text = "Request is successfully done in factor 10"
                });
            }
            return BadRequest(new Message { Id = counter, Text = "It's not a factor 10 number. Request rejected." });
        }
    }
}
