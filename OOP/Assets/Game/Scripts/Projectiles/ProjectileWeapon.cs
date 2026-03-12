using System;
using System.Collections.Generic;
using Game.Characters;
using Game.Characters.CharacterComponents;
using Game.Utils;
using UnityEngine;

namespace Game.Projectiles
{
    [Serializable]
    public class ProjectileWeapon : MonoBehaviour, IResetComponent
    {
        [SerializeField]
        private float fireCooldown = 1.25f;
        
        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField] 
        private ProjectileStatConfig projectileStatConfig;
        
        [SerializeField]
        private Transform projectilesContainer;
        
        [SerializeField]
        private Transform firePoint;

        private float lastFireTime;
        private CharacterShip owner;
        private ObjectPool<Projectile> projectilePool;
        private readonly HashSet<Projectile> activeProjectiles = new();

        public event Action OnFire;


        private void Awake()
        {
            projectilePool = new ObjectPool<Projectile>(projectilePrefab, projectilesContainer, 5);
        }

        private void OnDestroy()
        {
            foreach (var projectile in activeProjectiles)
            {
                projectile.OnDealDamage -= DespawnProjectile;
                projectile.OnOutOfCameraView -= DespawnProjectile;

                projectile.OnDealDamage += projectile.DestroyMyself;
                projectile.OnOutOfCameraView += projectile.DestroyMyself;
            }
        }

        public void Setup(CharacterShip _owner)
        {
            owner = _owner;
        }

        public bool TryToFire(Vector3 targetPosition)
        {
            if (Time.time - lastFireTime < fireCooldown)
            {
                return false;
            }

            SpawnProjectile(targetPosition);
            OnFire?.Invoke();
            lastFireTime = Time.time;
            return true;
        }

        private void SpawnProjectile(Vector3 targetPosition)
        {
            var projectile = projectilePool.GetObject();
            projectile.transform.SetParent(projectilesContainer);
            projectile.transform.position = firePoint.position;
            projectile.Setup(projectileStatConfig, owner.Faction);
            projectile.SetDirection(targetPosition - firePoint.position);
            projectile.gameObject.SetActive(true);
            switch (owner.Faction)
            {
                case Faction.Enemy:
                    projectile.gameObject.layer = (int)PhysicsLayer.ENEMY_PROJECTILE;
                    break;
                case Faction.Player:
                    projectile.gameObject.layer = (int)PhysicsLayer.PLAYER_PROJECTILE;
                    break;
            }
            
            projectile.OnDealDamage += DespawnProjectile;
            projectile.OnOutOfCameraView += DespawnProjectile;
            
            activeProjectiles.Add(projectile);
        }

        private void DespawnProjectile(Projectile projectile)
        {
            projectilePool.ReleaseObject(projectile);
            activeProjectiles.Remove(projectile);
        }

        public void ResetValues()
        {
            lastFireTime = 0;
        }
    }
}