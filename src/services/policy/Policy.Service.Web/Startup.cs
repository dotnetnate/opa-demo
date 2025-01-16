using AutoMapper;
using CitizensFinancialGroup.Threvw.Tenants.Domain;
using CitizensFinancialGroup.Threvw.Tenants.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.OpenApi;
using System.Text.Json;
using MongoDB.Bson;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using CitizensFinancialGroup.Elements.Data.MongoDb;
using CitizensFinancialGroup.Threvw.Policies.Domain;
using CitizensFinancialGroup.Elements.Security.Identity;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.TBD;
using CitizensFinancialGroup.Elements.Validation;
using System.Text.Json.Serialization;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Models;
using System.Diagnostics.CodeAnalysis;
using CitizensFinancialGroup.Elements.Data.MongoDb.Configuration;
using CitizensFinancialGroup.Elements.Validation.FluentValidation;




namespace CitizensFinancialGroup.Threvw.Policies.Service.Http {
    [ExcludeFromCodeCoverage]
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;            
        }

        public IConfiguration Configuration { get; }

        private void ConfigureOptions(IServiceCollection services) {
            services.AddOptions<MongoDbCollectionOptions>().Bind(Configuration.GetSection("cfg:threvw:database:mongo:collections:tenantManagement"));            
            services.AddOptions<MongoDbConnectionOptions>().Bind(Configuration.GetSection("cfg:threvw:database:mongo:connection"));
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {



            // Configure the JSON serializer options globally for configuration binding
            services.Configure<JsonSerializerOptions>(options => {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            ConfigureOptions(services);
            ConfigureMongo();

            // Register MongoDB client
            services.AddSingleton<IMongoClient>(sp => {

                var opts = sp.GetRequiredService<IOptions<MongoDbConnectionOptions>>().Value;

                if (!string.IsNullOrEmpty(opts.ConnectionString)) {
                    var settings = MongoClientSettings.FromConnectionString(opts.ConnectionString);                    
                    

                    return new MongoClient(settings);
                }
                else {
                    return new MongoClient(new MongoClientSettings {
                        Credential = MongoCredential.CreatePlainCredential(opts.Server, opts.UserName, opts.Password)
                    });
                }
            });

            // Register the repository
            services.AddScoped<IPolicyRepository>(sp => {
                return new MongoDbPolicyRepository(sp.GetRequiredService<IMongoClient>(), sp.GetRequiredService<IOptions<MongoDbCollectionOptions>>());
            });


            services.AddScoped<IIdentityService<ClaimsIdentity,object>, NullIdentittyService>();


            // Register the service
            services.AddScoped<IPolicyService, PolicyService<object>>();


            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Register logging
            services.AddLogging(config => {
                config.AddConsole();
                config.AddDebug();
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => {
                c.DescribeAllParametersInCamelCase();                
            });



            // Register the controller
            services.AddControllers()
                  .AddJsonOptions(options => {
                      options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                      options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;                      
                      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                      options.JsonSerializerOptions.Converters.Add(new ConditionModelJsonConverter());
                  });

            services.AddSingleton<IValidationService, FluentValidationValidationService>();


            services.AddOpenTelemetry();
            services.ConfigureOpenTelemetryTracerProvider(config => {
                config.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("TenantService"));
                config.AddAspNetCoreInstrumentation();
                config.AddHttpClientInstrumentation();
                config.AddConsoleExporter();
            });
        }

        public void ConfigureMongo() {
            MongoDbPolicyRepositoryConfiguration.Configure();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

     

            app.UseRouting();

            app.UseEndpoints(endpoints => {                
                endpoints.MapControllers().WithOpenApi();
            });
        }
    }
}