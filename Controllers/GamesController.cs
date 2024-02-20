using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace gamespace_server.Controllers;

[ApiController]
[Route("[controller]")]
public class GamesController : ControllerBase
{
    private readonly ILogger<GamesController> _logger;

    public GamesController(ILogger<GamesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<GamesResponse>> Get()
    {
        DotNetEnv.Env.Load();

        using (var client = new HttpClient())
        {
            var url = "https://api.igdb.com/v4/games";
            var dataToSend = new StringContent("fields name, involved_companies; search \"Halo\";", Encoding.UTF8, "application/json");
            // client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            client.DefaultRequestHeaders.Add("Client-ID", Environment.GetEnvironmentVariable("CLIENT_ID"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync(url, dataToSend);
            if (response.IsSuccessStatusCode)
            {
                string jsondata = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsondata);
                // var responseData = JsonConvert.DeserializeObject<Games>(jsondata);
                return Ok(jsondata);
            } else {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}
