using Microsoft.Extensions.Configuration;
using ProcessadorTarefas.Entidades;
using ProcessadorTarefas.Repositorios;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessadorTarefas.Servicos
{
    public class Processador : IProcessadorTarefas
    {

        private readonly IGerenciadorTarefas _gerenciador;
        private readonly IConfiguration _configurations;
        private readonly Queue<Tarefa> _agendadas = new();
        private readonly List<Tarefa> _emExecucao = new();
        private bool _isRunning;
        private Dictionary<int, CancellationTokenSource> cancelationTokens = new Dictionary<int, CancellationTokenSource>();

        public Processador(IGerenciadorTarefas gerenciador, IConfiguration configurations)
        {
            _gerenciador = gerenciador;
            _configurations = configurations;
        }

        public async Task CancelarTarefa(int idTarefa)
        {
            await _gerenciador.Cancelar(idTarefa);
        }

        public Task Encerrar()
        {
            throw new NotImplementedException();
        }

        public Task Iniciar()
        {
            var tarefasAtivas = _gerenciador.ListarAtivas().GetAwaiter().GetResult();
            
            _isRunning = true;


            GerenciarProcessamento();

            return Task.CompletedTask;
        }

        private async Task GerenciarProcessamento()
        {
            while (_isRunning)
            {
                try
                {
                    PopularAgendadas();
                    ConsumirAgendadas();
                }
                catch (Exception ex)
                {
                    Debug.Fail($"Error: {ex.Message}");
                }

                await Task.Delay(250);
            }
        }

        private async Task PopularAgendadas()
        {
            var num = 5;
            var tarefaAtivas = await _gerenciador.ListarAtivas();
            var quantidadeAgendadas = tarefaAtivas.Count(t => t.Estado == EstadoTarefa.Agendada);
            if (quantidadeAgendadas < num)
            {
                var tarefaAEspera = _gerenciador.ListarAtivas().Result.First(t => t.Estado == EstadoTarefa.Criada);
                tarefaAEspera.Agendar();
                _agendadas.Enqueue(tarefaAEspera);
            }
        }

        private void ConsumirAgendadas()
        {
            if (_agendadas.Count != 0)
            {
                var num = 5;

                if (_emExecucao.Count < num)
                {
                    var tarefaAExecutar = _agendadas.Dequeue();

                    var cancelationSource = new CancellationTokenSource();
                    _emExecucao.Add(tarefaAExecutar);

                    cancelationTokens.Add(tarefaAExecutar.Id, cancelationSource);
                    Task.Run(async () =>
                    {
                        await ProcessarTarefa(tarefaAExecutar);
                        _emExecucao.Remove(tarefaAExecutar);
                    },
                        cancelationSource.Token);

                }
            }
        }

        private async Task ProcessarTarefa(Tarefa tarefa)
        {
            tarefa.Iniciar();
            var subtarefas = tarefa.SubtarefasPendentes;

            foreach (var subtarefa in subtarefas)
            {
                await Task.Delay(subtarefa.Duracao);
                tarefa.ConcluirSubtarefa(subtarefa);
            }
            tarefa.Concluir();
        }



    }
}
