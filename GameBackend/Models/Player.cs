namespace GameBackend.Models
{
    public class Player
    {
        public string Id { get; set; } = string.Empty; // ID único do jogador
        public string Name { get; set; } = string.Empty; // Nome do jogador
        public double TotalTime { get; set; } = 0; // Tempo acumulado do jogador
        public bool IsEliminated { get; set; } = false; // Jogador eliminado?
        public bool IsConnected { get; set; } = true;    //  Indica se o jogador está conectado
    }
}

