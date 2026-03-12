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
        [SerializeField] 
        private float outOfCameraViewDistance = 0.5f;
        public event Action<Projectile> OnDealDamage;
        public event Action<Projectile> OnOutOfCameraView;
        
        public Faction OwnerFaction { get; private set; } = Faction.None;

        private int damage;
        private float speed;
        private Vector2 direction;

        public void Setup(ProjectileStatConfig statConfig, Faction ownerFaction)
        {
            damage = statConfig.Damage;
            speed = statConfig.Speed;
            OwnerFaction = ownerFaction;
        }
        
        private void FixedUpdate()
        {
            Move();
            if (CheckIsOutOfCameraView())
            {
                OnOutOfCameraView?.Invoke(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out ICanReceiveDamage receiver)) 
                return;

            if (OwnerFaction != Faction.None &&
                receiver.Faction != Faction.None &&
                OwnerFaction != receiver.Faction)
            {
                receiver.ReceiveDamage(damage);
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
            Vector3 moveStep = (Vector3)direction * (speed * Time.fixedDeltaTime);
            transform.position += moveStep;
        }

        private bool CheckIsOutOfCameraView()
        {
            var viewPos = Camera.main.WorldToViewportPoint(transform.position);
            return (viewPos.x < -outOfCameraViewDistance ||
                    viewPos.x > 1 + outOfCameraViewDistance) ||
                   (viewPos.y < -outOfCameraViewDistance ||
                    viewPos.y > 1 + outOfCameraViewDistance);
        }
    }
}