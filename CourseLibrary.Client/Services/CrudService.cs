using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using CourseLibrary.Client.Models;
using System.Net.Http.Headers;
using CourseLibrary.API.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace CourseLibrary.Client.Services
{
    public class CRUDService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();
        public CRUDService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:61237");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.khadoos.full+json"));
        }
        public async Task RunAsync()
        {
             //await GetAuthors();
             //await CreateAuthor();
        }

        public async Task GetAuthors()
        {
            var response = await _httpClient.GetAsync("api/authors");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            //if(response.Content.Headers.ContentType.MediaType == "application/vnd.khadoos.full+json")
            var authors = JsonConvert.DeserializeObject<IEnumerable<AuthorFull>>(content);
        }

        public async Task CreateAuthor()
        {
            var authorDtoForCreation = new AuthorDtoForCreation()
            {
                FirstName = "Seabury",
                LastName = "Toxic Reyson",
                DateOfBirth = "1690-11-23",
                MainCategory = "Maps"
            };

            var serializeObject = JsonConvert.SerializeObject(authorDtoForCreation);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/authors");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializeObject);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var createdAuthor = JsonConvert.DeserializeObject<Author>(content);

        }

        public async Task DeleteAuthor()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/authors/5e872646bf1bb2040c523eab");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
        }

        public async Task PatchAuthor()
        {
            var patchDocument = new JsonPatchDocument<AuthorDtoForCreation>();
            patchDocument.Replace(a => a.LastName, "");

            var response = await _httpClient.PatchAsync("api/authors/5e872646bf1bb2040c523eab",
                                            new StringContent(JsonConvert.SerializeObject(patchDocument), 
                                            Encoding.UTF8,
                                            "application/json-patch+json"));

            response.EnsureSuccessStatusCode();

            //parse content as string which create in memory string which as large as complete response body
            var content = await response.Content.ReadAsStringAsync();

            //stream avoid creating this in memory string and deserialize directly from content stream
            var partiallyUpdatedAuthor = JsonConvert.DeserializeObject<Author>(content);
        }
    }
}
