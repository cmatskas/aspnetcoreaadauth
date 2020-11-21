using System.Text.Json.Serialization;

namespace TodoListWebApp.Models
{
    public class TodoItem
    {
        [JsonPropertyName("owner")]
        public string Owner { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
