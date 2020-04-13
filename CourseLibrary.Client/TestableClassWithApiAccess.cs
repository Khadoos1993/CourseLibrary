using CourseLibrary.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourseLibrary.Client
{
    public class TestableClassWithApiAccess
    {
        private readonly HttpClient _httpClient;

        public TestableClassWithApiAccess(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Author>> GetAuthors(CancellationToken token)
        {
            var response = await _httpClient.GetAsync("http://localhost:61237/api/authors", HttpCompletionOption.ResponseHeadersRead,token);
            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("The requested movie cannot be found.");
                    return null;
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Access Denied");
                }

                response.EnsureSuccessStatusCode();
            }
            

            var content = await response.Content.ReadAsStreamAsync();
            return content.ReadAndDeserializeWithStream<Author>();

        }
    }
}
