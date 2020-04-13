using CourseLibrary.API.Models;
using CourseLibrary.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Client.Services
{
    public class StreamService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();
        public StreamService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:61237");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task RunAsync()
        {
            await GetAuthorsUsingStream();
        }

        public async Task GetAuthorsUsingStream()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/authors");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));


            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStreamAsync();
            var authors = content.ReadAndDeserializeWithStream<Author>();
           
            //if(response.Content.Headers.ContentType.MediaType == "application/vnd.khadoos.full+json")

        }

        public async Task GetAuthorsUsingStreamAndCompletionMode()
        {
            var response = await _httpClient.GetAsync("api/authors", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStreamAsync();

            using (var stream = new StreamReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stream))
                {
                    var jsonSerializer = new JsonSerializer();
                    var author = jsonSerializer.Deserialize<IEnumerable<Author>>(jsonTextReader);
                }
            }
            //if(response.Content.Headers.ContentType.MediaType == "application/vnd.khadoos.full+json")

        }

        //public async Task CreateAuthorWithStream()
        //{
        //    var authorDtoForCreation = new AuthorDtoForCreation()
        //    {
        //        FirstName = "Seabury",
        //        LastName = "Toxic Reyson",
        //        DateOfBirth = "1690-11-23",
        //        MainCategory = "Maps"
        //    };

        //    var serializeObject = JsonConvert.SerializeObject(authorDtoForCreation);
        //    var request = new HttpRequestMessage(HttpMethod.Post, "api/authors");
        //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    request.Content = new StringContent(serializeObject);
        //    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        //    var response = await _httpClient.SendAsync(request);
        //    response.EnsureSuccessStatusCode();

        //    var content = await response.Content.ReadAsStringAsync();
        //    var createdAuthor = JsonConvert.DeserializeObject<Author>(content);

        //}

    }
}
