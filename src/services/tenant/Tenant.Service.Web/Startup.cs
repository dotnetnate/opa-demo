using AutoMapper;
using CitizensFinancialGroup.Threvw.Tenants.Domain;
using CitizensFinancialGroup.Threvw.Tenants.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using CitizensFinancialGroup.Threvw.Tenants.Service.Http.TBD;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.OpenApi;
using System.Text.Json;
using MongoDB.Bson;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using CitizensFinancialGroup.Threvw.Common.Data.MongoDb;
using CitizensFinancialGroup.Threvw.Common.Identity;
using CitizensFinancialGroup.Threvw.Common.Validation;




namespace CitizensFinancialGroup.Threvw.Tenants.Service.Http {
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
            services.AddScoped<ITenantRepository>(sp => {
                return new MongoDbTenantRepository(sp.GetRequiredService<IMongoClient>(), sp.GetRequiredService<IOptions<MongoDbCollectionOptions>>());
            });


            services.AddScoped<IIdentityService<ClaimsIdentity,object>, NullIdentittyService>();


            // Register the service
            services.AddScoped<ITenantService, TenantService<object>>();


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

            services.AddCors(c => {
                c.AddPolicy("AllowAll", builder => {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Register the controller
            services.AddControllers()
                  .AddJsonOptions(options => {
                      options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                      options.JsonSerializerOptions.Converters.Add(new OptionsGroupEntityJsonConverter());
                      options.JsonSerializerOptions.Converters.Add(new OptionsGroupModelJsonConverter());
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
            MongoDbTenantRepositoryConfiguration.Configure();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

     

            app.UseRouting();

            app.UseCors(c => {
                c.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader(); 
            });

            app.UseEndpoints(endpoints => {                
                endpoints.MapControllers().WithOpenApi();
            });
        }
    }
}