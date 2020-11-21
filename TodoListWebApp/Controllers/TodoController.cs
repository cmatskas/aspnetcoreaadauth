using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using TodoListWebApp.Models;
using System.Text;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoListWebApp.Controllers
{
    [AuthorizeForScopes(Scopes = new []{"api://9a080a62-e244-4adc-a7d9-dc98835c8815/access_as_user"})]
    public class TodoController : Controller
    {
        private static ITokenAcquisition tokenGetter { get; set; }
        private static HttpClient httpClient;
        public TodoController(ITokenAcquisition tokenAcquisition, IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient();
            tokenGetter = tokenAcquisition;
        }

        public async Task<IActionResult> Index()
        {
            var token = await tokenGetter.GetAccessTokenForUserAsync(new []{
                "api://9a080a62-e244-4adc-a7d9-dc98835c8815/access_as_user"
            });
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var result = await httpClient.GetAsync("https://localhost:44351/todolist");
            var content = await result.Content.ReadAsStringAsync();
            var todoItems = JsonSerializer.Deserialize<List<TodoItem>>(content);
            return View(todoItems);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string item)
        {
            if (ModelState.IsValid)
            {
                var token = await tokenGetter.GetAccessTokenForUserAsync(new []{
                "api://9a080a62-e244-4adc-a7d9-dc98835c8815/access_as_user"
                });
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var todoItem = new TodoItem { Title=item, Owner = User.GetObjectId()};
                var content = new StringContent(JsonSerializer.Serialize(todoItem), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await httpClient.PostAsync("https://localhost:44351/todolist",content);

                if(result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error");
                }
            }
            
            return View("Error");
        }
    }
}
