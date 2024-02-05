using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using ProcessadorTarefas.Entidades;
using ProcessadorTarefas.Repositorios;

namespace ConsoleUI
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var settings = ConfigurationManager.AppSettings;
            var maxSubtarefasCheck = int.TryParse(settings["MaxSubtarefas"], out int maxSubtarefas);
            var maxTarefasEmExecCheck = int.TryParse(settings["MaxTarefasEmExec"], out int maxTarefasEmExec);

            var repositorio = new MemoryRepository(maxSubtarefas);
            while (true)
            {
                Console.Clear();

                Menu();

                var escolha = Console.ReadLine();

                Console.Clear();

                switch (escolha)
                {
                    case "3":
                        ListarTarefas(repositorio);
                        Console.ReadKey();
                        continue;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        continue;
                }
            }


        }

        static void Menu()
        {
            Console.WriteLine($"1 -> Criar Tarefa\n2 -> Cancelar Tarefa\n3 -> Listar Tarefas\n0 -> Sair\n");
        }

        static void ListarTarefas(IRepository<Tarefa> repositorio)
        {
            foreach (var item in repositorio.GetAll())
            {
                Console.WriteLine(item);
            }
        }

    }
}
