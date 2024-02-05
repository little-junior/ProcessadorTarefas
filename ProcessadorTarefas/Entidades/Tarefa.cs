namespace ProcessadorTarefas.Entidades
{
    public class Tarefa : ITarefa
    {
        public int Id { get; set; }
        public EstadoTarefa Estado { get; set; }
        public DateTime IniciadaEm { get; set; }
        public DateTime EncerradaEm { get; set; }
        public IEnumerable<Subtarefa> SubtarefasPendentes { get; set; }
        public IEnumerable<Subtarefa> SubtarefasExecutadas { get; set; }

        public Tarefa(int maxSubTarefas)
        {
            Id = new Random().Next();
            Estado = EstadoTarefa.Criada;
            SubtarefasPendentes = GenerateSubTarefas(maxSubTarefas);
            SubtarefasExecutadas = Array.Empty<Subtarefa>();
        }

        private IEnumerable<Subtarefa> GenerateSubTarefas(int maxSubTarefas)
        {
            var randomNumber = new Random().Next(10, maxSubTarefas + 1);

            return Enumerable.Range(10, randomNumber).Select(index => new Subtarefa()).ToArray();
        }

        public override string ToString()
        {
            return $"Tarefa {Id} | Estado: {Estado} | Iniciada em: {IniciadaEm} | Encerrada Em: {EncerradaEm} |  Pendentes: {SubtarefasPendentes.Count()} | Executadas: {SubtarefasExecutadas?.Count()}";
        }
    }
}
