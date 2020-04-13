using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.DbContext;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using AutoMapper;
using CourseLibrary.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Serialization;
using IdentityServer4.AccessTokenValidation;

namespace CourseLibrary.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        IConfiguration Configuration;
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpCacheHeaders((expirationModelOptions) => 
            {
                expirationModelOptions.MaxAge = 60;
                expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Public;
            },
            (validationModelOptions) =>
            {
                validationModelOptions.MustRevalidate = true;
            }
            );
            services.AddResponseCaching();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(opt =>
                {
                    opt.Authority = "https://localhost:5001/";
                    opt.ApiName = "courselibraryapi";    //is the given api is valid in the aud of the access token
                });
            services.AddMvc(option =>
            {
                option.ReturnHttpNotAcceptable = true;
                option.ModelBinderProviders.Insert(0, new ArrayModelBinderProvider());
                option.CacheProfiles.Add("240SecondsCacheProfile", 
                                                    new CacheProfile()
                                                    {
                                                        Duration = 240
                                                    });
            })
            .AddXmlDataContractSerializerFormatters()
            //.AddJsonOptions(setupAction =>
            //{
            //    setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //})
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    //create a problem details object
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Detail = "See the error for details",
                        Type = context.HttpContext.TraceIdentifier,
                        
                    };
                    var actionExecutingContext = context as ActionExecutingContext;
                    if (context.ModelState.ErrorCount > 0 &&
                    (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "One or more validation errors";
                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    }

                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more input errors occured";
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            services.AddCors(setupAction =>
            {
                setupAction.AddPolicy("reactcors", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            services.Configure<MongoDBSetting>(Configuration.GetSection("MongoDB"));
            services.TryAddScoped<ICourseLibraryContext, CourseLibraryContext>();
            services.TryAddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
            //services.TryAddSingleton<IMongoDBSetting>(sp => sp.GetRequiredService<IOptions<MongoDBSetting>>().Value);

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected error occured while processing your request.\n Please try again");
                    });
                });
            }
            app.UseCors("reactcors");

            //app.UseResponseCaching();
            //app.UseHttpCacheHeaders();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
