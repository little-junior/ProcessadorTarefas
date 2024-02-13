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
            SubtarefasExecutadas = new List<Subtarefa>();
        }

        private List<Subtarefa> GenerateSubTarefas(int maxSubTarefas)
        {
            var randomNumber = new Random().Next(10, maxSubTarefas + 1);

            return Enumerable.Range(10, randomNumber).Select(index => new Subtarefa()).ToList();
        }

        
        public override string ToString()
        {
            return $"Tarefa {Id} | Estado: {Estado} | Iniciada em: {IniciadaEm} | Encerrada Em: {EncerradaEm} |  Pendentes: {SubtarefasPendentes.Count()} | Executadas: {SubtarefasExecutadas?.Count()}";
        }



        public void Agendar()
        {
            Estado = EstadoTarefa.Agendada;
        }

        public void Iniciar()
        {
            Estado = EstadoTarefa.EmExecucao;
            IniciadaEm = DateTime.Now;
        }
        public void Cancelar()
        {
            Estado = EstadoTarefa.Cancelada;
            EncerradaEm = DateTime.Now;
        }

        public void Pausar()
        {
            Estado = EstadoTarefa.EmPausa;
        }

        public void Concluir()
        {
            Estado = EstadoTarefa.Concluida;
            EncerradaEm = DateTime.Now;
        }

        public void ConcluirSubtarefa(Subtarefa subtarefa)
        {
            SubtarefasPendentes = SubtarefasPendentes.Except(new[] { subtarefa });
            SubtarefasExecutadas = SubtarefasExecutadas.Append(subtarefa);
        }
    }
}
