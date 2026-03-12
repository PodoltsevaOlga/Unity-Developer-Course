using System.Collections.Generic;
using Game.Utils;
using UnityEngine;

namespace Game.Projectiles
{
    public class ProjectileSpawnerFactory : MonoBehaviour
    {
        [SerializeField] 
        private Transform container;
        
        private readonly Dictionary<Projectile, ObjectPool<Projectile>>
            prefabToSpawner = new();

        public Projectile SpawnProjectile(Projectile prefab)
        {
            if (!prefabToSpawner.TryGetValue(prefab, out var spawner))
            {
                spawner = new ObjectPool<Projectile>(prefab, container);
                prefabToSpawner[prefab] = spawner;
            }

            var spawnedProjectile = spawner.GetObject();
            spawnedProjectile.OnDealDamage += DespawnProjectile;
            return spawnedProjectile;
        }

        private void DespawnProjectile(Projectile projectile)
        {
            
        }

        private void Spawn()
        {
            
           // _bullets.Remove(bullet);

            //bullet.gameObject.SetActive(false);
         //   _pool.Push(bullet);
         // projectile.OnDealDamage += Despawn
        }
    }
}