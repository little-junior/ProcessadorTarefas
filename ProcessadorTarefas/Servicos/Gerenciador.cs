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
    public class Gerenciador : IGerenciadorTarefas
    {
        private readonly IRepository<Tarefa> _tarefas;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configurations;

        public Gerenciador(IRepository<Tarefa> tarefas, IServiceProvider serviceProvider, IConfiguration configurations)
        {
            _tarefas = tarefas;
            _serviceProvider = serviceProvider;
            _configurations = configurations;
        }

        public async Task Cancelar(int idTarefa)
        {
            var tarefa = await Consultar(idTarefa);

            tarefa.Cancelar();

            await Task.CompletedTask;
        }

        public async Task<Tarefa> Consultar(int idTarefa)
        {
            var tarefa = _tarefas.GetById(idTarefa);

            if (tarefa == null)
            {
                Debug.Fail($"Tarefa com ID {idTarefa} não encontrada.");
            }

            return await Task.FromResult(tarefa);
        }

        public async Task<Tarefa> Criar()
        {
            var maxSubtarefas = int.Parse(_configurations["maxSubtarefas"]!);
            var tarefa = new Tarefa(maxSubtarefas);
            _tarefas.Add(tarefa);
            return await Task.FromResult(tarefa);
        }

        public async Task<IEnumerable<Tarefa>> ListarAtivas()
        {
            var tarefasAtivas = _tarefas.GetAll().Where(t => t.Estado == EstadoTarefa.Agendada || t.Estado == EstadoTarefa.EmExecucao || t.Estado == EstadoTarefa.Criada || t.Estado == EstadoTarefa.EmPausa);
            return await Task.FromResult(tarefasAtivas);
        }

        public async Task<IEnumerable<Tarefa>> ListarInativas()
        {
            return await Task.FromResult(_tarefas.GetAll().Where(t => t.Estado == EstadoTarefa.Concluida || t.Estado == EstadoTarefa.Cancelada));
        }
    }
}
