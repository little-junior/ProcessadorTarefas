using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using ProcessadorTarefas.Entidades;
using ProcessadorTarefas.Repositorios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessadorTarefas.Servicos;

namespace ConsoleUI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = ConfigureServiceProvider();

            var configuration = serviceProvider.GetService<IConfiguration>();

            ConfigurationsValidation(configuration);

            var processador = serviceProvider.GetService<IProcessadorTarefas>();
            var gerenciador = serviceProvider.GetService<IGerenciadorTarefas>();

            await processador!.Iniciar();

            while (true)
            {
                Menu.ImprimirOpcoes();

                Console.Clear();

                await Task.Delay(100);
            }



        }

        private static IServiceProvider ConfigureServiceProvider()
        {

            IConfiguration configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .Build();


            IServiceCollection services = new ServiceCollection();
            services.AddScoped(_ => configuration);
            services.AddScoped<IRepository<Tarefa>, MemoryRepository>();
            services.AddSingleton<IProcessadorTarefas, Processador>();
            services.AddScoped<IGerenciadorTarefas, Gerenciador>(serviceProvider =>
            {
                var repository = serviceProvider.GetService<IRepository<Tarefa>>();
                return new Gerenciador(repository, serviceProvider, configuration);
            });

            return services.BuildServiceProvider();
        }

        private static void ConfigurationsValidation(IConfiguration configurations)
        {
            try
            {
                var firstValidation = int.TryParse(configurations["maxSubtarefas"], out int maxSubtarefas);
                var secondValidation = int.TryParse(configurations["maxTarefasEmExecucao"], out int maxTarefasEmExecucao);


                if (!firstValidation || !secondValidation)
                {
                    throw new ConfigurationErrorsException("Os campos do arquivo de configuração estão incorretos. Tente novamente");
                }

                if (maxSubtarefas < 10 || maxSubtarefas > 100)
                {
                    throw new ConfigurationErrorsException("O campo 'maxSubtarefas' deve ser um número entre 10 e 100");
                }

                if (maxTarefasEmExecucao <= 0)
                {
                    throw new ConfigurationErrorsException("O campo 'maxTarefasEmExecucao' deve ser um número maior do que 0");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Um erro ocorreu:\n");
                Console.WriteLine($"\"{ex.Message}\"");
                Environment.Exit(0);
            }

        }

    }
}
