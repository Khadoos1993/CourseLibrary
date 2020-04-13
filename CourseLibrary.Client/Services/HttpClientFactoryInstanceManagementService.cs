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
    class HttpClientFactoryInstanceManagementService:IIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthorClient _auhtorClient;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public HttpClientFactoryInstanceManagementService(IHttpClientFactory httpClientFactory, AuthorClient authorClient)
        {
            _httpClientFactory = httpClientFactory;
            _auhtorClient = authorClient;
        }

        public async Task RunAsync()
        {
            await GetAuthorsWithClientFactory(_cancellationTokenSource.Token);
        }

        private async Task GetAuthorsWithClientFactory(CancellationToken token)
        {
            var httpClient = _httpClientFactory.CreateClient("AuthorClient");

            var request = new HttpRequestMessage(HttpMethod.Get, "api/authors");
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            try
            {
                using (var response = await httpClient.SendAsync(request,HttpCompletionOption.ResponseHeadersRead, token))
                {
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStreamAsync();
                    var authors = content.ReadAndDeserializeWithStream<Author>();
                    Console.WriteLine(authors);
                };

            }
            catch (OperationCanceledException ex)
            {
                Console.Write("An operation was cancelled by the user");
            }
        }

        private async Task GetAuthorWithTypeClien(CancellationToken token)
        {
            var authors = await _auhtorClient.GetAuthors(token);
        }
    }
}
