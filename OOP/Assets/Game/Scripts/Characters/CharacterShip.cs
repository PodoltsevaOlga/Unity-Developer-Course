using System;
using Game.Characters.CharacterComponents;
using Game.Projectiles;
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
            movementAgent = new MovementAgent(shipRigidbody, statConfiguration.MoveSpeed);
            weapon.Setup(CharacterFaction);
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }
        
        public void ReceiveDamage(int damage)
        {
            if (health.ApplyDamage(damage))
            {
                OnHealthChanged?.Invoke(health.CurrentHealthPoints);
                if (health.CurrentHealthPoints <= 0)
                {
                    Die();
                }
            }
        }

        public Vector2 GetLastMovement() => movementAgent.LastMovement;

        public Faction Faction => CharacterFaction;

        protected virtual void OnDying()
        {
        }

        private void Die()
        {
            OnDead?.Invoke(this);
            OnDying();
        }
    }
}