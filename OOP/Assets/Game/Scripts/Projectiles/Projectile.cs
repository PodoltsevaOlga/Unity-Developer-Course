using System;
using Game.Characters;
using Game.Characters.CharacterComponents;
using Game.Utils;
using UnityEngine;

namespace Game.Projectiles
{
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour, IPoolableObject
    {
        public event Action<Projectile> OnDealDamage;

        private ProjectileStatConfig statConfig;
        
        public Faction OwnerFaction { get; private set; } = Faction.None;
        
        private Vector2 direction;

        public void Setup(ProjectileStatConfig _statConfig, Faction ownerFaction)
        {
            statConfig = _statConfig;
            OwnerFaction = ownerFaction;
            gameObject.layer = FactionToPhysicsLayerResolver.Resolve(ownerFaction);
        }
        
        private void FixedUpdate()
        {
            Move();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out ICanReceiveDamage receiver)) 
                return;

            if (OwnerFaction != Faction.None &&
                receiver.Faction != Faction.None &&
                OwnerFaction != receiver.Faction)
            {
                receiver.ReceiveDamage(statConfig.Damage);
                OnDealDamage?.Invoke(this);
            }
        }

        public void OnActivate()
        {
        }

        public void DestroyMyself(Projectile projectile)
        {
            if (projectile == this)
            {
                Destroy(this);
            }
        }
        
        public void SetDirection(Vector2 _direction)
        {
            direction = _direction.normalized;
        }

        private void Move()
        {
            Vector3 moveStep = (Vector3)direction * (statConfig.Speed * Time.fixedDeltaTime);
            transform.position += moveStep;
        }
    }
}