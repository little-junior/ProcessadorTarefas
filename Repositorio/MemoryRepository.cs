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
        private readonly List<Tarefa> _tarefas;

        public MemoryRepository(int maxSubTarefas)
        {
            _tarefas = new List<Tarefa>(GenerateTarefas(maxSubTarefas)) {};
        }

        public void Add(Tarefa entity)
        {
            _tarefas.Add(entity);
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

        private IEnumerable<Tarefa> GenerateTarefas(int maxSubTarefas)
        {
            return Enumerable.Range(1, 100).Select(index => new Tarefa(maxSubTarefas));
        }
    }
}
