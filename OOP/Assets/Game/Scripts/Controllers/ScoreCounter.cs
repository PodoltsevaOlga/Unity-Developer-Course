using System;
using Game.Characters;
using UnityEngine;

namespace Game.Controllers
{
    public class ScoreCounter : MonoBehaviour
    {
        [SerializeField] 
        private EnemySpawner enemySpawner;
        
        public int Score { get; private set; }
        
        public event Action<int> OnScoreChanged;

        private void Awake()
        {
            if (enemySpawner != null)
            {
                enemySpawner.OnEnemyKilled += OnEnemyKilled;
            }
        }

        private void OnDestroy()
        {
            if (enemySpawner != null)
            {
                enemySpawner.OnEnemyKilled -= OnEnemyKilled;
            }
        }

        private void OnEnemyKilled(Enemy _)
        {
            Score++;
            OnScoreChanged?.Invoke(Score);
        }

        public void Reset()
        {
            Score = 0;
            OnScoreChanged?.Invoke(Score);
        }
    }
}