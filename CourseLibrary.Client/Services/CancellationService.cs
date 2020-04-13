using CourseLibrary.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourseLibrary.Client.Services
{
    class CancellationService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public CancellationService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:61237");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }
        public async Task RunAsync()
        {
            _cancellationTokenSource.CancelAfter(2000);
            await GetAuthorsWithCancel(_cancellationTokenSource.Token);
            
        }

        private async Task GetAuthorsWithCancel(CancellationToken token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/authors");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            try
            {
                using (var response = await _httpClient.SendAsync(request, token))
                {
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStreamAsync();
                    var authors = content.ReadAndDeserializeWithStream<Author>();
                    Console.WriteLine(authors);
                };

            }
            catch(OperationCanceledException ex)
            {
                Console.Write("An operation was cancelled by the user");
            }



        }
    }
}
