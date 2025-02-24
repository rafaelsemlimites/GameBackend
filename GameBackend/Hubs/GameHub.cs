using GameBackend.Models;
using GameBackend.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameBackend.Hubs
{
    public class GameHub : Hub
    {
        private readonly GameManager _gameManager;

        public GameHub(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        //  Jogador entra no jogo
        public async Task JoinGame(string playerName)
        {
            var playerId = Context.ConnectionId;
            _gameManager.AddPlayer(playerId, playerName);

            Console.WriteLine($"✅ Jogador {playerName} entrou no jogo!");

            // Envia o estado atualizado do jogo para todos os jogadores
            await Clients.All.SendAsync("UpdateGame", _gameManager.GetGameState());
        }

        //  Jogador clica no botão para registrar o tempo
        public async Task ClickButton()
        {
            try
            {
                var playerId = Context.ConnectionId;
                _gameManager.RegisterClick(playerId);

                Console.WriteLine($"🕹️ Jogador {playerId} clicou!");

                // Envia atualização do jogo para todos os jogadores
                await Clients.All.SendAsync("UpdateGame", _gameManager.GetGameState());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no ClickButton: {ex.Message}");
            }
        }

        //  Reiniciar o jogo
        public async Task ResetGame()
        {
            Console.WriteLine("🔄 Reiniciando o jogo...");
            _gameManager.ResetGame();

            // Envia atualização do jogo para todos os jogadores
            await Clients.All.SendAsync("UpdateGame", _gameManager.GetGameState());
        }

        //  Gerencia quando um jogador desconecta
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var playerId = Context.ConnectionId;
            _gameManager.RemovePlayer(playerId);

            Console.WriteLine($"🚪 Jogador {playerId} desconectado.");

            await Clients.All.SendAsync("UpdateGame", _gameManager.GetGameState());

            await base.OnDisconnectedAsync(exception);
        }

        public async Task LeaveGame()
        {
            var playerId = Context.ConnectionId;
            _gameManager.RemovePlayer(playerId);

            // Atualiza todos os clientes com o novo estado do jogo
            await Clients.All.SendAsync("UpdateGame", _gameManager.GetGameState());
        }



    }
}
