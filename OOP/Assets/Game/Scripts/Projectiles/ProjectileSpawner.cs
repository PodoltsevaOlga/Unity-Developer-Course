using System.Collections.Generic;
using Game.Characters;
using Game.Utils;
using Modules.Utils;
using UnityEngine;

namespace Game.Projectiles
{
    public class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField]
        private Projectile projectilePrefab;
        
        [SerializeField]
        private TransformBounds levelBounds;
        
        private Transform projectilesContainer;
        
        private ObjectPool<Projectile> projectilePool;
        private readonly List<Projectile> activeProjectiles = new();

        private void Awake()
        {
            projectilesContainer = new GameObject().transform;
            projectilesContainer.gameObject.name = $"{name} container";
            projectilesContainer.transform.parent = gameObject.transform;
            projectilePool = new ObjectPool<Projectile>(projectilePrefab, projectilesContainer, 5);
        }
        
        private void FixedUpdate()
        {
            for (int i = activeProjectiles.Count - 1; i >= 0; --i)
            {
                if (!levelBounds.InBounds(activeProjectiles[i].transform.position))
                {
                    DespawnProjectile(activeProjectiles[i]);
                }
            }
        }
        
        private void OnDestroy()
        {
            foreach (var projectile in activeProjectiles)
            {
                projectile.OnDealDamage -= DespawnProjectile;

                projectile.OnDealDamage += projectile.DestroyMyself;
            }
        }

        public Projectile SpawnProjectile(ProjectileStatConfig config,
            Faction ownerFaction, Vector3 spawnPosition, Vector3? targetPosition)
        {
            var projectile = projectilePool.GetObject();
            projectile.transform.SetParent(projectilesContainer);
            projectile.transform.position = spawnPosition;
            
            if (targetPosition == null)
            {
                projectile.SetDirection(Vector2.up);
            }
            else
            {
                projectile.SetDirection(targetPosition.Value - spawnPosition);
            }
            
            projectile.Setup(config, ownerFaction);
            
            projectile.OnDealDamage += DespawnProjectile;
            
            activeProjectiles.Add(projectile);
            projectile.gameObject.SetActive(true);

            return projectile;
        }

        private void DespawnProjectile(Projectile projectile)
        {
            projectile.OnDealDamage -= DespawnProjectile;
            
            projectilePool.ReleaseObject(projectile);
            activeProjectiles.Remove(projectile);
        }
    }
}