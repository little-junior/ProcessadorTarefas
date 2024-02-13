using Microsoft.Extensions.Configuration;
using ProcessadorTarefas.Entidades;
using ProcessadorTarefas.Repositorios;
using System;
using System.Collections.Generic;
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

        public Task Cancelar(int idTarefa)
        {
            //var tarefa = _tarefas.GetById(idTarefa);

            //if (tarefa == null)
            //    throw new KeyNotFoundException($"Tarefa com ID {idTarefa} não encontrada.");

            //_tarefas.Delete(tarefa);
            return Task.CompletedTask;
        }

        public async Task<Tarefa> Consultar(int idTarefa)
        {
            var tarefa = _tarefas.GetById(idTarefa);

            return await Task.FromResult(tarefa) ?? throw new KeyNotFoundException($"Tarefa com ID {idTarefa} não encontrada.");
        }

        public async Task<Tarefa> Criar()
        {
            var tarefa = new Tarefa(100);
            _tarefas.Add(tarefa);
            return await Task.FromResult(tarefa);
        }

        public async Task<IEnumerable<Tarefa>> ListarAtivas()
        {
            return await Task.FromResult(_tarefas.GetAll().Where(t => t.Estado == EstadoTarefa.Agendada || t.Estado == EstadoTarefa.EmExecucao));
        }

        public async Task<IEnumerable<Tarefa>> ListarInativas()
        {
            return await Task.FromResult(_tarefas.GetAll().Where(t => t.Estado == EstadoTarefa.Concluida || t.Estado == EstadoTarefa.Cancelada));
        }
    }
}
