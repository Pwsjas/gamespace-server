using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Logging;

namespace gamespace_server.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;

    public SearchController(ILogger<SearchController> logger)
    {
        _logger = logger;
    }

    [HttpPost] // Change this to handle POST requests
    public async Task<ActionResult<string>> Post([FromBody] FilteredSearchRequest request)
    {
        DotNetEnv.Env.Load();

        using (var client = new HttpClient())
        {
            var url = "https://api.igdb.com/v4/games";
            // Construct the fields query using the list of IDs from the request body
            var filters = "";

            if(request.Genres?.Any() == true) {
                var genreList = string.Join("\",\"", request.Genres);
                genreList = $"\"{genreList}\"";

                filters = $" & genres.name = ({genreList})";
            }

            if(request.Platforms?.Any() == true) {
                var platformList = string.Join("\",\"", request.Platforms);
                platformList = $"\"{platformList}\"";

                filters = $"{filters} & platforms.name = ({platformList})";
            }

            if(request.Search != "") {
                var searchString = $"*\"{request.Search}\"*";

                filters = $"{filters} & name ~ {searchString}";
            }

            var fields = $"fields name,cover.image_id,rating,genres.name; where themes != (42){filters}; limit 20;";
            Console.WriteLine(fields);
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