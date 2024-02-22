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
            var fields = "fields name,cover.image_id,rating; where first_release_date >= 1693540800 & rating >= 80 & platforms = (6, 130, 167, 169) & themes != (42); sort rating desc; limit 8;";
            var dataToSend = new StringContent(fields, Encoding.UTF8, "application/json");

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
