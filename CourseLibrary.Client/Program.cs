using CourseLibrary.Client.Services;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CourseLibrary.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            try
            {
                using(var httpClient = new HttpClient())
                {
                    var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync("http://localhost/5001");
                    if (discoveryDocument.IsError)
                        throw new Exception("Error in processing the discovery document", discoveryDocument.Exception);
                    var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        ClientId = "courselibraryclient",
                        ClientSecret = "courselibraryclient",
                        Address = discoveryDocument.TokenEndpoint,
                        Scope = "courselibraryapi",
                        
                    });

                    if (tokenResponse.IsError)
                        throw new Exception("Error while accessing the token endpoint", tokenResponse.Exception);
                    Console.WriteLine(tokenResponse.Json);
                    

                }
                await serviceProvider.GetRequiredService<IIntegrationService>().RunAsync();

            }
            catch(Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An exception occured while running the integration service");
            }
            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            
            services.AddLogging(cfg => cfg.AddConsole().AddConsole());
            services.AddHttpClient("AuthorClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:61237");
                client.Timeout = new TimeSpan(0, 0, 30);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler(handler => new RetryPolicyDelegateHandler(2));
            services.AddSingleton<IIntegrationService, HttpClientFactoryInstanceManagementService>();
            services.AddHttpClient<AuthorClient>();
        }
    }
}
