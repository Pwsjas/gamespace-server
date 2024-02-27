using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace gamespace_server.Controllers;

[ApiController]
[Route("[controller]")]
public class PopularGamesController : ControllerBase
{
    private readonly ILogger<PopularGamesController> _logger;

    public PopularGamesController(ILogger<PopularGamesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<GamesResponse>> Get()
    {
        DotNetEnv.Env.Load();

        using (var client = new HttpClient())
        {
            //Calculate unix time from 4 months ago to display recently popular titles.
            DateTime currentTime = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
            unixTime = unixTime - 10520000; //4 Months Ago

            var url = "https://api.igdb.com/v4/games";
            var fields = $"fields name,cover.image_id,rating,genres.name; where first_release_date >= {unixTime} & rating >= 80 & platforms = (6, 130, 167, 169) & themes != (42); sort rating desc; limit 8;";
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
