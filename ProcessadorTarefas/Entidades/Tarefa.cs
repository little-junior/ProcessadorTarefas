using System.Text;

namespace ProcessadorTarefas.Entidades
{
    public class Tarefa : ITarefa
    {
        private static int _id = 0;
        public int Id { get; set; }
        public EstadoTarefa Estado { get; set; }
        public DateTime IniciadaEm { get; set; }
        public DateTime EncerradaEm { get; set; }
        public IEnumerable<Subtarefa> SubtarefasPendentes { get; set; }
        public IEnumerable<Subtarefa> SubtarefasExecutadas { get; set; }
        public double DuracaoTotal { get; set; }
        public double DuracaoPercorrida
        {
            get
            {
                return GetDuracao(SubtarefasExecutadas);
            }
        }
        public Tarefa(int maxSubTarefas)
        {
            _id++;
            Id = _id;
            Estado = EstadoTarefa.Criada;
            SubtarefasPendentes = GenerateSubTarefas(maxSubTarefas);
            SubtarefasExecutadas = new List<Subtarefa>();
            DuracaoTotal = GetDuracao(SubtarefasPendentes);
        }

        private List<Subtarefa> GenerateSubTarefas(int maxSubTarefas)
        {
            var randomNumber = new Random().Next(10, maxSubTarefas + 1);

            return Enumerable.Range(10, randomNumber).Select(index => new Subtarefa()).ToList();
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                string.Join('|',
                    $"{Id}".PadRight(5, ' '),
                    $"{Estado}".PadRight(15, ' '),
                    $"{(IniciadaEm == DateTime.MinValue ? "n/a" : IniciadaEm)}".PadRight(30, ' '),
                    $"{(EncerradaEm == DateTime.MinValue ? "n/a" : EncerradaEm)}".PadRight(30, ' '),
                    $"{DuracaoTotal}".PadRight(10, ' ')
                    )
            );

            return sb.ToString();
        }

        public void Agendar()
        {
            Estado = EstadoTarefa.Agendada;
        }

        public void Iniciar()
        {
            Estado = EstadoTarefa.EmExecucao;

            if(IniciadaEm == DateTime.MinValue)
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

        private double GetDuracao(IEnumerable<Subtarefa> subtarefas)
        {
            int duracaoTotal = 0;

            foreach (var subTarefa in subtarefas)
            {
                duracaoTotal += (int)subTarefa.Duracao.TotalSeconds;
            }

            return duracaoTotal;
        }

        public bool PodeSerCancelada()
        {
            if (Estado == EstadoTarefa.Criada)
                return true;

            if (Estado == EstadoTarefa.Agendada)
                return true;

            if (Estado == EstadoTarefa.EmExecucao) 
                return true;

            return false;
        }
    }
}
