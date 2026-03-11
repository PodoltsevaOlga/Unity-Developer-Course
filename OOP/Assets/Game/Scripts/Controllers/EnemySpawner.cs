using System;
using System.Collections;
using Game.Characters;
using Game.Utils;
using Modules.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.CharacterControllers
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
        
        private float currentSpawnCooldown;
        private float lastSpawnTime;
        private int currentSpawnPositionIndex;
        private int currentAttackPositionIndex;

        private ObjectPool<Enemy> enemiesPool = null;

        public event Action<Enemy> OnEnemySpawned;

        private void Awake()
        {
            enemiesPool = new ObjectPool<Enemy>(enemyPrefab, spawnContainer);
            ShuffleSpawnPositions();
            
        }
        
        private void Start()
        {
            ResetSpawnCooldown();
        }
        
        private void FixedUpdate()
        {
            float time = Time.fixedTime;
            //if (GameController.isGameActive)
            if (time - lastSpawnTime < currentSpawnCooldown)
                return;

            SpawnEnemy();
            ResetSpawnCooldown();
        }

        private Enemy SpawnEnemy()
        {
            var enemy = enemiesPool.GetObject();
            enemy.transform.position = NextSpawnPosition();
            enemy.OnSpawn();
            enemy.Setup(player, NextAttackPosition());
            enemy.OnDead += DespawnEnemy;
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