using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Bson;

namespace data_generator
{

    class Program
    {
        class Options
        {
            [Option('r', "resourceCount", Required = false, Default = 100, HelpText = "Number of resources.")]
            public int NumberOfResources { get; set; }

            [Option('s', "subjectCount", Required = false, Default = 5, HelpText = "Number of subjects per resource.")]
            public int NumberOfSubjectsPerResource { get; set; }

            [Option('p', "permissionCount", Required = false, Default = 5, HelpText = "Number of permissions per subject.")]
            public int NumberOfPermissionsPerSubject { get; set; }

            [Option('o', "outputFolder", Default = "", HelpText = "Output folder name.")]
            public string OutputFolder { get; set; } = "";
            [Option('t', "strategy", Default = "cfg.authz.rich.v4", HelpText = "Data generation strategy.")]
            public string Strategy { get; set; } = "cfg.authz.rich.v4";

            [Option('z', "storage", Default = "file", HelpText = "Storage method.")]
            public string Storage { get; set; } = "file";
        }

        static void Main(string[] args)
        {

            MongoDbPolicyRepositoryConfiguration.Configure();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                {
                    var serviceProvider = ConfigureServices(opts);
                    RunOptionsAndReturnExitCode(opts, serviceProvider);
                })
                .WithNotParsed<Options>(HandleParseError);
        }

        static ServiceProvider ConfigureServices(Options options)
        {
            var services = new ServiceCollection();


            services.AddKeyedSingleton<IPersistence, MongoPersistence>("mongo", (sp, _) =>
            {
                var client = new MongoClient("mongodb://root:example@localhost:27017");
                var database = client.GetDatabase("opa-demo");
                var collection = database.GetCollection<PolicyWrapper>("policies_current");
                return new MongoPersistence(collection);
            });

            services.AddKeyedSingleton<IPersistence, StreamPersistence>("file", (sp, _) =>
            {
                var streamWriter = new StreamWriter("output.json");
                return new StreamPersistence(streamWriter);
            });
            services.AddSingleton<IPersistence, StreamPersistence>((sp) =>
            {
                var streamWriter = new StreamWriter("output.json");
                return new StreamPersistence(streamWriter);
            });


            services.AddKeyedSingleton<IDataGenerationStrategy, RichDataGenerationStrategyV2>("cfg.authz.rich.v2", (sp, _) =>
            {
                return new RichDataGenerationStrategyV2(sp.GetKeyedService<IPersistence>(options.Storage));
            });
            services.AddKeyedSingleton<IDataGenerationStrategy, RichDataGenerationStrategyV3>("cfg.authz.rich.v3", (sp, _) =>
            {
                return new RichDataGenerationStrategyV3(sp.GetKeyedService<IPersistence>(options.Storage));
            });
                        services.AddKeyedSingleton<IDataGenerationStrategy, RichDataGenerationStrategyV4>("cfg.authz.rich.v4", (sp, _) =>
            {
                return new RichDataGenerationStrategyV4(sp.GetKeyedService<IPersistence>(options.Storage));
            });
            services.AddKeyedSingleton<IDataGenerationStrategy, FlattenedDataGenerationStrategy>("cfg.authz.flattened", (sp, _) =>
            {
                return new FlattenedDataGenerationStrategy(sp.GetKeyedService<IPersistence>(options.Storage));
            });

            return services.BuildServiceProvider();
        }

        static void RunOptionsAndReturnExitCode(Options opts, ServiceProvider serviceProvider)
        {
            var strategy = serviceProvider.GetKeyedService<IDataGenerationStrategy>(opts.Strategy);

            Console.WriteLine($"Generating data using {opts.Strategy} strategy");

            strategy.GenerateSampleData(opts.NumberOfResources, opts.NumberOfSubjectsPerResource, opts.NumberOfPermissionsPerSubject, opts.OutputFolder);

        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Error parsing arguments");
        }
    }

}