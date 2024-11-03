using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PopulateDB.Interfaces;
using PopulateDB.Models;
using System.Text.Json;

namespace PopulateDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PopulateController : ControllerBase
    {
        private readonly IPopulateService _service;
        private readonly Uri baseUrl = new("https://jsonplaceholder.typicode.com/users");

        public PopulateController(IPopulateService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(baseUrl.ToString());

                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<List<User>>(content)
                            ?? throw new JsonException();

                        foreach (var users in result)
                        {
                            await _service.Populate(users);
                        }

                        return Created(nameof(Post), result);
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch(JsonException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return NoContent();
        }
    }
}
