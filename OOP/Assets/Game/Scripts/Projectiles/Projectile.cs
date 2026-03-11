using System;
using Game.Characters;
using UnityEngine;

namespace Game.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public event Action<Projectile> OnDealDamage;
        public Faction OwnerFaction { get; private set; } = Faction.None;
        
        [SerializeField] 
        private ProjectileConfig configuration;
        public ProjectileConfig Configuration => configuration;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out ICanReceiveDamage receiver)) 
                return;

            if (OwnerFaction != Faction.None &&
                receiver.Faction != Faction.None &&
                OwnerFaction != receiver.Faction &&
                Configuration.Damage > 0)
            {
                receiver.ReceiveDamage(Configuration.Damage);
            }

            OnDealDamage?.Invoke(this);


                // Explosion Vfx
              //  GameObject prefab = _configView.ExplosionVFX;
               // Instantiate(prefab, bullet.transform.position, prefab.transform.rotation);
        }
    }
}