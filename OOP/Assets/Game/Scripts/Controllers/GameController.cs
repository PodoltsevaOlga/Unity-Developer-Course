using System;
using Game.Characters;
using UnityEngine;

namespace Game.Controllers
{
    public class GameController : MonoBehaviour
    {
        public bool IsGameActive { get; private set; }
        public event Action OnGameOver;
        public event Action<int> OnScoreChanged;
        public event Action OnGameStart;

        [SerializeField] 
        private Player player;

        [SerializeField] 
        private EnemySpawner enemySpawner;

        public int Score { get; private set; }

        private void Start()
        {
            if (player != null)
            {
                player.OnDead += OnCharacterDie;
            }

            if (enemySpawner != null)
            {
                enemySpawner.OnEnemySpawned += delegate(Enemy enemy)
                {
                    enemy.OnDead += OnCharacterDie;
                };
            }
            
            StartGame();
        }

        private void StartGame()
        {
            IsGameActive = true;
            Score = 0;
            OnGameStart?.Invoke();
        }

        private void OnCharacterDie(CharacterShip characterShip)
        {
            if (characterShip == player)
            {
                player.OnDead -= OnCharacterDie;
                GameOver();
            }
            else if (characterShip.CharacterFaction == Faction.Enemy)
            {
                Score++;
                OnScoreChanged?.Invoke(Score);
            }
        }

        private void GameOver()
        {
            IsGameActive = false;
            OnGameOver?.Invoke();
        }
        
        
    }
}