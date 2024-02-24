using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace gamespace_server.Controllers;

[ApiController]
[Route("[controller]")]
public class ComingSoonGamesController : ControllerBase
{
    private readonly ILogger<ComingSoonGamesController> _logger;

    public ComingSoonGamesController(ILogger<ComingSoonGamesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<GamesResponse>> Get()
    {
        DotNetEnv.Env.Load();

        using (var client = new HttpClient())
        {
            //Calculate current unix time to display titles releasing in the near future.
            DateTime currentTime = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

            var url = "https://api.igdb.com/v4/release_dates";
            var fields = $"fields game.name,game.cover.image_id,game.rating,game.genres.name; where platform = (6, 130, 167, 169) & date > {unixTime} & region = 2 & game.themes != (42); sort date asc;";
            var dataToSend = new StringContent(fields, Encoding.UTF8, "text/plain");

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