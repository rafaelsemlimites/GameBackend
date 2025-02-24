using GameBackend.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameBackend.Services
{
    public class GameManager
    {
        private List<Player> _players;
        private GameState _gameState;
        private int _currentPlayerIndex = 0;

        public GameManager()
        {
            _players = new List<Player>();
            _gameState = new GameState();
        }

        //  Adiciona um novo jogador e inicia o jogo se necessário
        public void AddPlayer(string playerId, string playerName)
        {
            //  Evita adicionar o mesmo jogador novamente
            if (_players.Any(p => p.Id == playerId)) return;

            //  Adiciona o novo jogador à lista
            _players.Add(new Player
            {
                Id = playerId,
                Name = playerName,
                TotalTime = 0,
                IsEliminated = false,
                IsConnected = true
            });

            //  Atualiza o estado do jogo com os tempos corretos antes de enviar ao frontend
            _gameState.Players = new List<Player>(_players);

            Console.WriteLine($"👤 Novo jogador entrou: {playerName}");

            //  Atualiza a lista de jogadores no estado do jogo
            _gameState.Players = _players;

            if (!_gameState.GameStarted)
            {
                //  Se o jogo ainda não começou, zeramos o tempo de TODOS os jogadores
                foreach (var player in _players)
                {
                    player.TotalTime = 0;
                }
            }

            //  Se agora temos exatamente 2 jogadores, inicia automaticamente
            if (_players.Count == 2 && !_gameState.GameStarted)
            {
                Console.WriteLine("🎮 Dois jogadores detectados. Iniciando o jogo...");
                StartGame();
            }
        }

        // Método para iniciar o jogo sem resetar tudo
        private void StartGame()
        {
            if (_players.Count < 2)
            {
                Console.WriteLine("⚠️ Jogadores insuficientes para iniciar o jogo.");
                return;
            }

            // Define o primeiro jogador a jogar
            _currentPlayerIndex = 0;
            _gameState = new GameState
            {
                Players = _players,
                CurrentPlayer = _players[_currentPlayerIndex],
                GameStarted = true,
                Winner = null
            };

            Console.WriteLine("✅ O jogo começou!");
        }


        //  Registra um clique e atualiza o tempo do jogador
        public void RegisterClick(string playerId)
        {
            var player = _players.FirstOrDefault(p => p.Id == playerId);

            if (player == null || !_gameState.GameStarted || player.IsEliminated)
                return;

            // Simula tempo entre 0 e 3 segundos
            double timeTaken = Math.Round(new Random().NextDouble() * 3, 2);
            player.TotalTime += timeTaken;

            Console.WriteLine($"🕒 {player.Name} clicou! Tempo acumulado: {player.TotalTime}s");

            // Verifica eliminação
            if (player.TotalTime >= 30)
            {
                player.IsEliminated = true;
                Console.WriteLine($"❌ {player.Name} foi eliminado!");
            }

            // Atualiza o estado do jogo removendo eliminados
            _gameState.Players = _players.Where(p => !p.IsEliminated).ToList();

            // Verifica se há um vencedor
            var activePlayers = _players.Where(p => !p.IsEliminated).ToList();
            if (activePlayers.Count == 1)
            {
                DeclareWinner(activePlayers.First());
            }
            else
            {
                // Passa o turno para o próximo jogador não eliminado
                PassTurn();
            }
        }

        // Passa o turno para o próximo jogador ativo
        private void PassTurn()
        {
            if (_players.Count(p => !p.IsEliminated) <= 1) return; // Se só tem um jogador ativo, não passa a vez

            do
            {
                _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            } while (_players[_currentPlayerIndex].IsEliminated);

            _gameState.CurrentPlayer = _players[_currentPlayerIndex];
        }

        // Retorna o estado do jogo
        public GameState GetGameState() => _gameState;

        // Reinicia o jogo mantendo apenas jogadores conectados
        public void ResetGame()
        {
            Console.WriteLine("🔄 Reiniciando o jogo...");

            //  Remove jogadores desconectados antes de reiniciar
            _players = _players.Where(p => p.IsConnected).ToList();

            //  Se restar apenas 1 jogador, ele vence automaticamente
            if (_players.Count == 1)
            {
                _gameState = new GameState
                {
                    Players = _players.Select(p => new Player
                    {
                        Id = p.Id,
                        Name = p.Name,
                        TotalTime = 0,  
                        IsEliminated = false,
                        IsConnected = true
                    }).ToList(),
                    
                    CurrentPlayer = null,
                    GameStarted = false,
                    
                    Winner = _players.Any()
                        ? new Player
                        {
                            Id = _players.First().Id,
                            Name = _players.First().Name,
                            TotalTime = 0,  //  Zera o tempo do vencedor antes de atribuí-lo
                            IsEliminated = false,
                            IsConnected = true
                        }
                        : null

                };

                Console.WriteLine($"🏆 {_gameState.Winner.Name} venceu automaticamente porque todos os outros saíram!");
                return;
            }

            //  Se não houver jogadores suficientes, reseta completamente o jogo
            if (_players.Count < 2)
            {
                Console.WriteLine("⚠️ Não há jogadores suficientes para reiniciar o jogo. Resetando estado...");
                _gameState = new GameState
                {
                    Players = new List<Player>(),
                    CurrentPlayer = null,
                    GameStarted = false,
                    Winner = null
                };
                return;
            }

            //  Resetando corretamente os jogadores (tempo, eliminações e status de conexão)
            foreach (var player in _players)
            {
                player.TotalTime = 0;  // ✅ Agora garantimos que o tempo de TODOS os jogadores seja resetado
                player.IsEliminated = false;
            }

            //  Limpa o vencedor da partida anterior antes de iniciar uma nova
            _gameState.Winner = null;

            //  Atualiza a lista de jogadores e limpa o vencedor da partida anterior
                _gameState = new GameState
                {
                    Players = _players.Select(p => new Player
                    {
                        Id = p.Id,
                        Name = p.Name,
                        TotalTime = 0,  //  Resetando o tempo para todos os jogadores na cópia
                        IsEliminated = false,
                        IsConnected = true
                    }).ToList(),
                    CurrentPlayer = _players[0],
                    GameStarted = true,
                    Winner = null
                };

            //  Define o primeiro jogador de forma alternada para mais equilíbrio
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            _gameState = new GameState
            {
                Players = _players.Select(p => new Player
                {
                    Id = p.Id,
                    Name = p.Name,
                    TotalTime = 0,  //  Resetando o tempo para todos os jogadores na cópia
                    IsEliminated = false,
                    IsConnected = true
                }).ToList(),
                CurrentPlayer = _players[_currentPlayerIndex], //  Alternando quem começa a nova rodada
                GameStarted = true,
                Winner = null
            };

            Console.WriteLine("✅ Novo jogo iniciado! Todos os tempos foram resetados e um novo jogador começa.");
        }


        // Remove jogador e ajusta o jogo
        public void RemovePlayer(string playerId)
        {
            var player = _players.FirstOrDefault(p => p.Id == playerId);

            if (player == null)
                return; //  Se o jogador não existir, não faz nada

            _players.Remove(player); //  Remove o jogador da lista
            Console.WriteLine($"🚪 Jogador {player.Name} saiu da partida.");

            //  Atualiza a lista de jogadores no estado do jogo
            _gameState.Players = new List<Player>(_players);

            //  Verifica se restou apenas um jogador na partida
            if (_players.Count == 1)
            {
                _gameState.Winner = _players.First();
                _gameState.GameStarted = false;
                Console.WriteLine($"🎉 {_gameState.Winner.Name} venceu automaticamente!");
            }
            else if (_players.Count == 0)
            {
                //  Se todos os jogadores saíram, reseta o jogo completamente
                Console.WriteLine("⚠️ Todos os jogadores saíram. O jogo foi resetado.");
                ResetGame();
            }
            else
            {
                // Se o jogo ainda estiver em andamento, passa o turno para o próximo jogador válido
                PassTurn();
            }
        }



        //  Declara o vencedor e encerra o jogo
        private void DeclareWinner(Player winner)
        {
            _gameState.Winner = winner;
            _gameState.GameStarted = false;
            Console.WriteLine($"🎉 {winner.Name} venceu!");
        }
    }
}
