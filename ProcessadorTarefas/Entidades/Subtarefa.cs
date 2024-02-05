namespace ProcessadorTarefas.Entidades
{
    public struct Subtarefa
    {
        public TimeSpan Duracao { get; set; }

        public Subtarefa()
        {
            var randomNumber = new Random().Next(3, 61);
            Duracao = new TimeSpan(0, 0, randomNumber);
        }

        public override string ToString()
        {
            return $"Subtarefa com duracao de {Duracao.Seconds} segundos";
        }
    }

}
