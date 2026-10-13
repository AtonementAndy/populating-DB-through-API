using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PopulateDB.Interfaces;
using PopulateDB.Models;
using System.Text.Json;

namespace PopulateDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PopulateController(
        IPopulateService service, 
        IHttpClientFactory client,
        ILogger<PopulateController> logger) : ControllerBase
    {  
        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken ct)
        {
            var http = client.CreateClient("populate");

                try
                {
                    var response = await http.GetAsync("users", ct);
                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadFromJsonAsync<List<User>>(cancellationToken: ct)
                        ?? throw new JsonException("Resposta vazia da API externa");

                    foreach (var user in result)
                    {
                        logger.LogInformation("Populando usuário {Username}", user.Username);  
                        await service.Populate(user);
                    }

                    return Created(nameof(Post), result);
                }
                catch (HttpRequestException ex)
                {
                    logger.LogError(ex, "Erro ao chamar API externa");
                    return Problem($"Erro ao chamar API externa: {ex.Message}", statusCode: 502);
                }
                catch(JsonException ex)
                {
                    logger.LogError(ex, "Erro ao chamar API externa");
                    return Problem($"Erro ao chamar API externa: {ex.Message}", statusCode: 502);
                }

        }
    }
}
