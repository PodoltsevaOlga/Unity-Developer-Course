using System;
using Game.Characters;
using UnityEngine;

namespace Game.Projectiles
{
    [Serializable]
    public class ProjectileWeapon
    {
        [SerializeField]
        private float fireCooldown = 1.25f;
        
        [SerializeField]
        private Projectile projectilePrefab;

        private float lastFireTime;
        private ProjectileSpawner projectileSpawner;
        private Character owner;

        public void Setup(Character _owner)
        {
            owner = _owner;
           
        }

        public void TryToFire()
        {
            
        }

        public void ResetValues()
        {
            lastFireTime = 0;
        }
    }
}