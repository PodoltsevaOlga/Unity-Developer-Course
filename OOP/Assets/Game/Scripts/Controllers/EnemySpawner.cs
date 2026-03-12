using System;
using System.Collections;
using Game.Characters;
using Game.Utils;
using Modules.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Controllers
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] 
        private Enemy enemyPrefab;
        
        [SerializeField]
        private float minSpawnCooldown = 2;

        [SerializeField]
        private float maxSpawnCooldown = 3;
        
        [SerializeField]
        private Transform[] spawnPositions;
        
        [SerializeField]
        private Transform[] attackPositions;

        [SerializeField] 
        private Transform spawnContainer;
        
        [SerializeField] 
        private Player player;

        [SerializeField] 
        private GameController gameController;

        private float currentSpawnCooldown;
        private float lastSpawnTime;
        private int currentSpawnPositionIndex;
        private int currentAttackPositionIndex;

        private bool canSpawn;

        private ObjectPool<Enemy> enemiesPool = null;

        public event Action<Enemy> OnEnemySpawned;

        private void Awake()
        {
            enemiesPool = new ObjectPool<Enemy>(enemyPrefab, spawnContainer);
            
            ShuffleSpawnPositions();
            ShuffleAttackPositions();
            
            canSpawn = true;
            gameController.OnGameOver += () => canSpawn = false;
        }
        
        private void Start()
        {
            ResetSpawnCooldown();
        }

        private void FixedUpdate()
        {
            if (!canSpawn)
            {
                return;
            }
            
            float time = Time.fixedTime;
            if (time - lastSpawnTime < currentSpawnCooldown)
                return;

            SpawnEnemy();
            ResetSpawnCooldown();
        }

        private Enemy SpawnEnemy()
        {
            var enemy = enemiesPool.GetObject();
            enemy.transform.position = NextSpawnPosition();
            enemy.gameObject.SetActive(true);
            enemy.Setup(player, NextAttackPosition());
            enemy.OnDead += DespawnEnemy;
            gameController.OnGameOver += enemy.ForbidActions;
            OnEnemySpawned?.Invoke(enemy);
            return enemy;
        }

        private void DespawnEnemy(Enemy enemy)
        {
            StartCoroutine(DespawnInNextFrame(enemy));
        }
        
        private IEnumerator DespawnInNextFrame(Enemy enemy)
        {
            yield return null;
            enemiesPool.ReleaseObject(enemy);
            gameController.OnGameOver += enemy.ForbidActions;
        }
        
        private void ResetSpawnCooldown()
        {
            currentSpawnCooldown = Random.Range(minSpawnCooldown, maxSpawnCooldown);
            lastSpawnTime = Time.fixedTime;
        }

        private void ShuffleSpawnPositions()
        {
            spawnPositions.Shuffle();
            currentSpawnPositionIndex = 0;
        }
        
        private void ShuffleAttackPositions()
        {
            attackPositions.Shuffle();
            currentAttackPositionIndex = 0;
        }
        
        private Vector3 NextSpawnPosition()
        {
            if (currentSpawnPositionIndex >= spawnPositions.Length)
            {
                ShuffleSpawnPositions();
            }

            return spawnPositions[currentSpawnPositionIndex++].position;
        }
        
        private Vector3 NextAttackPosition()
        {
            if (currentAttackPositionIndex >= attackPositions.Length)
            {
                ShuffleAttackPositions();
            }

            return attackPositions[currentAttackPositionIndex++].position;
        }
    }
}