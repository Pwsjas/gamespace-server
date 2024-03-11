namespace gamespace_server;

public class FilteredSearchRequest {
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Platforms { get; set; } = new List<string>();
        public string Search { get; set; } = "";
}