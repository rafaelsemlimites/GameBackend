using System.Collections.Generic;

namespace GameBackend.Models
{
    public class GameState
    {
        public Player? CurrentPlayer { get; set; }  // Jogador que está no turno
        public bool GameStarted { get; set; } = false;  // Indica se o jogo está em andamento
        public Player? Winner { get; set; }  // Armazena o vencedor do jogo
        public List<Player> Players { get; internal set; } = new();  // Lista de jogadores na partida
        public double TotalTime { get; set; } = 0;  // Tempo total acumulado

        public GameState()
        {
            // Inicializa as propriedades, se necessário
            Players = new List<Player>();
            Winner = null;
            CurrentPlayer = null;
            GameStarted = false;
            TotalTime = 0;
        }
    }
}
