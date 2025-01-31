﻿using Microsoft.Extensions.Configuration;
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
            var tarefaACancelar = await _gerenciador.Consultar(idTarefa);

            var idVerification = cancelationTokens.TryGetValue(idTarefa, out var _);

            if (!idVerification && tarefaACancelar == null)
            {
                Debug.Fail("Tarefa não encontrada");
                return;
            }

            if (!tarefaACancelar.PodeSerCancelada())
            {
                Debug.Fail("A tarefa não pode ser cancelada em seu estado atual.");
                return;
            }

            if (idVerification)
            {
                cancelationTokens[idTarefa].Cancel();
                cancelationTokens.Remove(idTarefa);
            }

            _emExecucao.Remove(tarefaACancelar);
            await _gerenciador.Cancelar(idTarefa);
        }

        public Task Encerrar()
        {
            _isRunning = false;

            _emExecucao.ForEach(tarefa =>
            {
                cancelationTokens[tarefa.Id].Cancel();
                cancelationTokens.Remove(tarefa.Id);
                tarefa.Pausar();
            });

            return Task.CompletedTask;
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

                    if (_emExecucao.Any(tarefa => tarefa.Estado == EstadoTarefa.EmPausa))
                        ConsumirPausadas();
                    
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
            var quantidadeAgendadas = _agendadas.Count;
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
                var maxTarefasEmExecucao = int.Parse(_configurations["maxTarefasEmExecucao"]);

                if (_emExecucao.Count < maxTarefasEmExecucao)
                {
                    var tarefaAExecutar = _agendadas.Dequeue();
                    if (tarefaAExecutar.Estado == EstadoTarefa.Agendada)
                    {

                        var cancelationSource = new CancellationTokenSource();
                        _emExecucao.Add(tarefaAExecutar);

                        cancelationTokens.Add(tarefaAExecutar.Id, cancelationSource);
                        Task.Run(async () =>
                            {
                                await ProcessarTarefa(tarefaAExecutar, cancelationSource.Token);
                                _emExecucao.Remove(tarefaAExecutar);
                            },
                            cancelationSource.Token);
                    }
                }
            }
        }

        private void ConsumirPausadas()
        {
            //Não funcional, por algum motivo as subtarefas ficam vazias
            var tarefaPausada = _emExecucao.First(tarefa => tarefa.Estado == EstadoTarefa.EmPausa);

            var cancelationSource = new CancellationTokenSource();

            cancelationTokens.Add(tarefaPausada.Id, cancelationSource);
            Task.Run(async () =>
            {
                await ProcessarTarefa(tarefaPausada, cancelationSource.Token);
                _emExecucao.Remove(tarefaPausada);
            },
                cancelationSource.Token);
        }

        private async Task ProcessarTarefa(Tarefa tarefa, CancellationToken cancellationToken)
        {
            tarefa.Iniciar();
            var subtarefas = tarefa.SubtarefasPendentes;

            foreach (var subtarefa in subtarefas)
            {
                await Task.Run(async () =>
                {
                    await Task.Delay(subtarefa.Duracao);
                    tarefa.ConcluirSubtarefa(subtarefa);

                }, cancellationToken);
            }

            tarefa.Concluir();
        }
    }
}
