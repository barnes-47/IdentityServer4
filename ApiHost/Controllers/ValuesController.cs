using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiHost.Controllers
{
    [Route("api/values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [Authorize(Policy.WriteAccess)]
        [HttpPost]
        public IActionResult Create(string value)
        {
            var message = $"Resource successfully created.";

            return StatusCode((int)HttpStatusCode.Created, new { message });
        }

        [Authorize(Policy.ReadAccess)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var values = new[] { "value_1", "value_2", "value_3" };

            return Ok(new { values });
        }

        [Authorize(Policy.ReadAccess)]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var value = $"value_{id}";

            return Ok(new { value });
        }

        [Authorize(Policy.AdminAccess)]
        [HttpPut]
        public IActionResult Update(string value)
        {
            var message = $"Resource successfully updated.";

            return Ok(new { message });
        }

        [Authorize(Policy.DeleteAccess)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var message = $"Resource successfully deleted.";

            return Ok(new { message });
        }
    }
}
