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

            Action<IGerenciadorTarefas> action = (gerenciador) => { };

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var option = Console.ReadKey(intercept: true).KeyChar;
                    Console.WriteLine();
                    switch (option)
                    {
                        case '1':
                            var novaTarefa = await gerenciador.Criar();
                            Console.WriteLine($"TAREFA {novaTarefa.Id} CRIADA!");
                            await Task.Delay(1000);
                            break;
                        case '2':
                            Console.Write("DIGITE O ID DA TAREFA: ");
                            var idTarefa = int.Parse(Console.ReadLine());
                            await processador.CancelarTarefa(idTarefa);
                            Console.WriteLine("TAREFA CANCELADA");
                            await Task.Delay(1000);
                            break;
                        case '3':
                            action = UserInterface.ImprimirTarefasAtivas;
                            break;
                        case '4':
                            action = UserInterface.ImprimirTarefasInativas;
                            break;
                        case '5':
                            action = (gerenciador) => { };
                            break;
                        case '6':
                            break;
                        case '0':
                            UserInterface.ImprimirTelaSaida();
                            await Task.Delay(2000);
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("OPÇÃO INVÁLIDA!");
                            await Task.Delay(1000);
                            break;
                    }
                }

                Console.Clear();

                UserInterface.ImprimirOpcoes();

                await Task.Run(() =>
                {
                    action(gerenciador);
                });

                UserInterface.ImprimirTarefasEmExecucao(gerenciador!);

                await Task.Delay(200);
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
