using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System; // Make sure System namespace is included for Environment access
using Microsoft.Extensions.Logging; // Ensure ILogger is accessible

namespace gamespace_server.Controllers;

[ApiController]
[Route("[controller]")]
public class SimilarGamesController : ControllerBase
{
    private readonly ILogger<SimilarGamesController> _logger;

    public SimilarGamesController(ILogger<SimilarGamesController> logger)
    {
        _logger = logger;
    }

    [HttpPost] // Change this to handle POST requests
    public async Task<ActionResult<string>> Post([FromBody] GameSearchRequest request)
    {
        DotNetEnv.Env.Load();

        using (var client = new HttpClient())
        {
            var url = "https://api.igdb.com/v4/games";
            // Construct the fields query using the list of IDs from the request body
            var idList = string.Join(",", request.Ids); // Converts List<int> to a comma-separated string
            var fields = $"fields name,cover.image_id,rating,genres.name; where id = ({idList}); limit 8;";
            var dataToSend = new StringContent(fields, Encoding.UTF8, "text/plain");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            client.DefaultRequestHeaders.Add("Client-ID", Environment.GetEnvironmentVariable("CLIENT_ID"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync(url, dataToSend);
            if (response.IsSuccessStatusCode)
            {
                string jsondata = await response.Content.ReadAsStringAsync();
                return Ok(jsondata);
            }
            else
            {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}
