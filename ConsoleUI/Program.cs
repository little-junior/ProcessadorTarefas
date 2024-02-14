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

            var processador = serviceProvider.GetService<IProcessadorTarefas>();
            var gerenciador = serviceProvider.GetService<IGerenciadorTarefas>();

            await processador!.Iniciar();

            while (true)
            {
                Menu();

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

        static void Menu()
        {
            Console.WriteLine("SELECIONE UMA DAS OPÇÕES ABAIXO");
            Console.WriteLine($"\n1 -> CRIAR TAREFA\n2 -> CANCELAR TAREFA\n3 -> LISTAR TAREFAS ATIVAS\n4 -> LISTAR TAREFAS INATIVAS\n5 -> PAUSAR PROCESSAMENTO\n0 -> SAIR\n");
        }

        

    }
}
