using CourseLibrary.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourseLibrary.Client
{
    public class AuthorClient
    {
        private HttpClient Client {get;}
        public AuthorClient(HttpClient client)
        {
            Client = client;
            client.BaseAddress = new Uri("http://localhost:61237");
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<Author>> GetAuthors(CancellationToken token)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, "api/authors");
           
            try
            {
                IEnumerable<Author> authors;
                using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token))
                {
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStreamAsync();
                    authors = content.ReadAndDeserializeWithStream<Author>();
                }
                return authors; 
            }
            catch (OperationCanceledException ex)
            {
                Console.Write("An operation was cancelled by the user");
                throw;
            }
        }
    }
}
