using ProcessadorTarefas.Entidades;
using ProcessadorTarefas.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI
{
    public static class UserInterface
    {
        public static void ImprimirOpcoes()
        {
            Console.WriteLine("SELECIONE UMA DAS OPÇÕES ABAIXO");
            Console.WriteLine($"\n1 -> CRIAR TAREFA\n2 -> CANCELAR TAREFA\n3 -> LISTAR TAREFAS ATIVAS\n4 -> LISTAR TAREFAS INATIVAS\n5 -> LIMPAR VISUALIZAÇÃO\n6 -> PAUSAR PROCESSAMENTO\n0 -> SAIR\n");
        }

        public static void ImprimirTarefasEmExecucao(IGerenciadorTarefas gerenciadorTarefas)
        {
            var tarefasEmExecucao = gerenciadorTarefas.ListarAtivas().GetAwaiter().GetResult().Where(t => t.Estado == EstadoTarefa.EmExecucao).OrderBy(t => t.Id);

            Console.WriteLine("\nEM EXECUÇÃO\n");
            ImprimirColunasEmExecucao();
            foreach (var tarefa in tarefasEmExecucao)
            {
                double porcentagemConcluida = (tarefa.DuracaoPercorrida / (tarefa.DuracaoTotal)) * 100;

                var sb = new StringBuilder();

                sb.AppendLine(
                string.Join('|',
                    $"{tarefa.Id}".PadRight(5, ' '),
                    $"{porcentagemConcluida:0.0}%".PadRight(15, ' '),
                    $"{tarefa.DuracaoTotal}".PadRight(30, ' '),
                    $"{tarefa.IniciadaEm}".PadRight(30, ' ')
                    )
                );

                Console.Write(sb);
            }
        }

        public static void ImprimirTarefasAtivas(IGerenciadorTarefas gerenciadorTarefas)
        {
            var tarefasAtivas = gerenciadorTarefas.ListarAtivas().GetAwaiter().GetResult().Where(t => t.Estado != EstadoTarefa.Criada).OrderBy(t => t.Id);

            Console.WriteLine("ATIVAS\n");
            ImprimirColunasCompleta();

            foreach (var tarefa in tarefasAtivas)
            {
                Console.Write(tarefa);
            }
        }

        public static void ImprimirTarefasInativas(IGerenciadorTarefas gerenciadorTarefas)
        {
            var tarefasInativas = gerenciadorTarefas.ListarInativas().GetAwaiter().GetResult().Where(t => t.Estado == EstadoTarefa.Concluida || t.Estado == EstadoTarefa.Cancelada).OrderBy(t => t.Id);

            Console.WriteLine("INATIVAS\n");
            ImprimirColunasCompleta();
            foreach (var tarefa in tarefasInativas)
            {
                Console.Write(tarefa);
            }
        }

        private static void ImprimirColunasCompleta()
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                string.Join('|',
                    "ID".PadRight(5, ' '),
                    "ESTADO".PadRight(15, ' '),
                    "INÍCIO".PadRight(30, ' '),
                    "TÉRMINO".PadRight(30, ' '),
                    "DURACAO TOTAL (segundos)".PadRight(10, ' ')
                    )
            );

            Console.WriteLine(sb);
        }

        private static void ImprimirColunasEmExecucao()
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                string.Join('|',
                    "ID".PadRight(5, ' '),
                    "% DE CONCLUSÃO".PadRight(15, ' '),
                    "DURACAO TOTAL (segundos)".PadRight(30, ' '),
                    "INICIO".PadRight(30, ' ')
                    )
            );

            Console.WriteLine(sb);
        }

        public static void ImprimirTelaSaida()
        {
            Console.Clear();
            Console.WriteLine("FECHANDO O SISTEMA...");
        }

        public static void ImprimirTelaEmPausa(IGerenciadorTarefas gerenciadorTarefas)
        {
                Console.Clear();

                Console.WriteLine("PROCESSADOR EM PAUSA\nDIGITE QUALQUER TECLA PARA VOLTAR O PROCESSAMENTO DE TAREFAS...");
                Console.WriteLine("\nTAREFAS PAUSADAS:\n");

                var tarefasEmExecucao = gerenciadorTarefas.ListarAtivas().GetAwaiter().GetResult().Where(t => t.Estado == EstadoTarefa.EmPausa).OrderBy(t => t.Id);

                ImprimirColunasEmExecucao();
                foreach (var tarefa in tarefasEmExecucao)
                {
                    double porcentagemConcluida = (tarefa.DuracaoPercorrida / (tarefa.DuracaoTotal)) * 100;

                    var sb = new StringBuilder();

                    sb.AppendLine(
                    string.Join('|',
                        $"{tarefa.Id}".PadRight(5, ' '),
                        $"{porcentagemConcluida:0.0}%".PadRight(15, ' '),
                        $"{tarefa.DuracaoTotal}".PadRight(30, ' '),
                        $"{tarefa.IniciadaEm}".PadRight(30, ' ')
                        )
                    );

                    Console.Write(sb);
                }
        }
    }
}
