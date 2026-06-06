using System;
using System.Collections;
using System.Collections.Generic;
using Game.Characters;
using Game.Utils;
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
        private int maxAliveEnemies = 5;
        
        [SerializeField]
        private EnemyPositionProvider positionProvider;

        [SerializeField] 
        private Transform spawnContainer;
        
        [SerializeField] 
        private Player player;

        [SerializeField] 
        private GameController gameController;

        private float currentSpawnCooldown;
        private float lastSpawnTime;

        private bool canSpawn;

        private ObjectPool<Enemy> enemiesPool = null;
        private List<Enemy> aliveEnemies = new();
        private int currentAliveEnemiesCount => aliveEnemies.Count;

        public event Action<Enemy> OnEnemySpawned;

        private void Awake()
        {
            enemiesPool = new ObjectPool<Enemy>(enemyPrefab, spawnContainer, maxAliveEnemies);
            canSpawn = true;
            aliveEnemies.Clear();
            gameController.OnGameOver += OnGameOver;
        }
        
        private void Start()
        {
            ResetSpawnCooldown();
            positionProvider.Reset();
        }

        private void FixedUpdate()
        {
            if (!canSpawn || currentAliveEnemiesCount == maxAliveEnemies)
            {
                return;
            }
            
            if (Time.fixedTime - lastSpawnTime < currentSpawnCooldown)
                return;

            SpawnEnemy();
            ResetSpawnCooldown();
        }

        private void OnDestroy()
        {
            gameController.OnGameOver -= OnGameOver;
            foreach (var enemy in aliveEnemies)
            {
                enemy.OnDead -= DespawnEnemy;
            }
        }

        private Enemy SpawnEnemy()
        {
            var enemy = enemiesPool.GetObject();
            enemy.transform.position = positionProvider.NextSpawnPosition();
            enemy.gameObject.SetActive(true);
            enemy.Setup(player, positionProvider.NextAttackPosition());
            enemy.OnDead += DespawnEnemy;
            OnEnemySpawned?.Invoke(enemy);
            aliveEnemies.Add(enemy);
            return enemy;
        }

        private void OnGameOver()
        {
            canSpawn = false;
            foreach (var enemy in aliveEnemies)
            {
                enemy.ForbidActions();
            }
            gameController.OnGameOver -= OnGameOver;
        }

        private void DespawnEnemy(Enemy enemy)
        {
            StartCoroutine(DespawnInNextFrame(enemy));
        }
        
        private IEnumerator DespawnInNextFrame(Enemy enemy)
        {
            yield return null;
            enemiesPool.ReleaseObject(enemy);
            enemy.OnDead -= DespawnEnemy;
            aliveEnemies.Remove(enemy);
        }
        
        private void ResetSpawnCooldown()
        {
            currentSpawnCooldown = Random.Range(minSpawnCooldown, maxSpawnCooldown);
            lastSpawnTime = Time.fixedTime;
        }
    }
}