using System;
using System.Collections;
using System.Collections.Generic;
using Game.Characters;
using Game.Projectiles;
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

        [SerializeField]
        private ProjectileSpawner projectileSpawner;

        private float currentSpawnCooldown;
        private float lastSpawnTime;

        private bool canSpawn;

        private ObjectPool<Enemy> enemiesPool = null;
        private List<Enemy> aliveEnemies = new();
        private int currentAliveEnemiesCount => aliveEnemies.Count;

        public event Action<Enemy> OnEnemySpawned;
        public event Action<Enemy> OnEnemyKilled;

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
                enemy.OnEnemyDead -= DespawnEnemyByDeath;
            }
        }

        private Enemy SpawnEnemy()
        {
            var enemy = enemiesPool.GetObject();
            enemy.transform.position = positionProvider.NextSpawnPosition();
            enemy.gameObject.SetActive(true);
            enemy.Setup(player, positionProvider.NextAttackPosition(), projectileSpawner);
            enemy.OnEnemyDead += DespawnEnemyByDeath;
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

        private void DespawnEnemyByDeath(Enemy enemy)
        {
            StartCoroutine(DespawnInNextFrame(enemy));
        }

        private IEnumerator DespawnInNextFrame(Enemy enemy)
        {
            yield return null;
            enemy.OnEnemyDead -= DespawnEnemyByDeath;
            OnEnemyKilled?.Invoke(enemy);
            aliveEnemies.Remove(enemy);
            enemiesPool.ReleaseObject(enemy);
        }

        private void ResetSpawnCooldown()
        {
            currentSpawnCooldown = Random.Range(minSpawnCooldown, maxSpawnCooldown);
            lastSpawnTime = Time.fixedTime;
        }
    }
}
