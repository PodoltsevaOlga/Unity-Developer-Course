using System;
using Game.Characters;
using Game.Characters.CharacterComponents;
using UnityEngine;

namespace Game.Projectiles
{
    [Serializable]
    public class ProjectileWeapon : MonoBehaviour, IResetComponent
    {
        [SerializeField]
        private float fireCooldown = 1.25f;

        [SerializeField] 
        private ProjectileStatConfig projectileStatConfig;

        private ProjectileSpawner projectileSpawner;
        
        [SerializeField]
        private Transform firePoint;

        private float lastFireTime;
        
        private Faction ownerFaction;
        public event Action OnFire;

        public void Setup(Faction _ownerFaction)
        {
            ownerFaction = _ownerFaction;
        }

        public void SetProjectileSpawner(ProjectileSpawner _projectileSpawner)
        {
            projectileSpawner = _projectileSpawner;
        }

        public bool TryToFire(Vector3? targetPosition)
        {
            if (Time.time - lastFireTime < fireCooldown)
            {
                return false;
            }
            
            if (projectileSpawner == null)
            {
                Debug.LogWarning($"projectile spawner in weapon {this.name} is null");
                return false;
            }

            projectileSpawner.SpawnProjectile(projectileStatConfig,
                ownerFaction, firePoint.position, targetPosition);

            OnFire?.Invoke();
            lastFireTime = Time.time;
            return true;
        }

        public void ResetValues()
        {
            lastFireTime = 0;
        }
    }
}