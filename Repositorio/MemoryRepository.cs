﻿using Microsoft.Extensions.Configuration;
using ProcessadorTarefas.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessadorTarefas.Repositorios
{
    public class MemoryRepository : IRepository<Tarefa>
    {
        private static List<Tarefa> _tarefas;
        private readonly IConfiguration _configurations;

        
        public MemoryRepository(IConfiguration configurations)
        {
            _configurations = configurations;
            _ = int.TryParse(_configurations["maxSubtarefas"], out int num);
            _tarefas = new List<Tarefa>(GenerateTarefas(num));
        }

        public void Add(Tarefa entity)
        {
            _tarefas = _tarefas.Prepend(entity).ToList();
        }

        public void Delete(Tarefa entity)
        {
            _tarefas.Remove(entity);
        }

        public IEnumerable<Tarefa> GetAll()
        {
            return _tarefas;
        }

        public Tarefa? GetById(int id)
        {
            return _tarefas.FirstOrDefault(t => t.Id == id);
        }

        public void Update(Tarefa entity)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Tarefa> GenerateTarefas(int maxSubTarefas)
        {
            return Enumerable.Range(1, 100).Select(index => new Tarefa(maxSubTarefas));
        }
    }
}
