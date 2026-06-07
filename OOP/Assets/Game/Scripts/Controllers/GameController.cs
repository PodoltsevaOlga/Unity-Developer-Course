using System;
using Game.Characters;
using UnityEngine;

namespace Game.Controllers
{
    public class GameController : MonoBehaviour
    {
        public bool IsGameActive { get; private set; }
        public event Action OnGameOver;
        
        public event Action OnGameStart;

        [SerializeField] 
        private Player player;

        [SerializeField] 
        private ScoreCounter scoreCounter;

        private void Start()
        {
            if (player != null)
            {
                player.OnDead += OnPlayerDied;
            }

            StartGame();
        }

        private void StartGame()
        {
            IsGameActive = true;
            scoreCounter.Reset();
            OnGameStart?.Invoke();
        }

        private void OnPlayerDied(CharacterShip _)
        {
            player.OnDead -= OnPlayerDied;
            GameOver();
        }

        private void GameOver()
        {
            IsGameActive = false;
            OnGameOver?.Invoke();
        }
    }
}