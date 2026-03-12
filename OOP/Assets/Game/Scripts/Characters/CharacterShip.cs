using System;
using Game.Characters.CharacterComponents;
using Game.Projectiles;
using Modules.Utils;
using UnityEngine;

namespace Game.Characters
{
    public abstract class CharacterShip : MonoBehaviour, ICanReceiveDamage
    {
        [SerializeField] 
        protected ProjectileWeapon weapon;
        [SerializeField] 
        protected CharacterShipStatConfig statConfiguration;
        [SerializeField] 
        protected Rigidbody2D shipRigidbody;
        [SerializeField] 
        protected TransformBounds allowedArea;
        
        public virtual Faction CharacterFaction => Faction.None;

        protected Health health;
        protected MovementAgent movementAgent;
        public event Action<int> OnHealthChanged;
        public event Action<CharacterShip> OnDead;

        public bool IsAlive => health.CurrentHealthPoints > 0;
        public int CurrentHealth => health.CurrentHealthPoints;
        public int MaxHealth => health.MaxHealthPoints;

        private void Awake()
        {
            health = new Health(statConfiguration.MaxHealth);
            movementAgent = new MovementAgent(shipRigidbody, statConfiguration.MoveSpeed,
                statConfiguration.MoveRotationAngle, statConfiguration.StoppingDistance,
                allowedArea);
            weapon.Setup(this);
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }
        
        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        protected virtual void OnFixedUpdate()
        {
            movementAgent.OnFixedUpdate();
        }

        private void LateUpdate()
        {
            OnLateUpdate();
        }
        
        protected virtual void OnLateUpdate()
        {
            movementAgent.OnLateUpdate();
        }

        public void ReceiveDamage(int damage)
        {
            if (health.ApplyDamage(damage))
            {
                if (health.CurrentHealthPoints <= 0)
                {
                    Die();
                }
                else
                {
                    OnHealthChanged?.Invoke(health.CurrentHealthPoints);
                }
            }
        }

        public Faction Faction => CharacterFaction;

        protected virtual void OnDying()
        {
            gameObject.SetActive(false);
        }

        private void Die()
        {
            OnDying();
            OnDead?.Invoke(this);
        }
    }
}