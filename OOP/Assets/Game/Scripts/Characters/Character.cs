using System;
using Game.Projectiles;
using UnityEngine;

namespace Game.Characters
{
    public abstract class Character : MonoBehaviour, ICanReceiveDamage
    {
        [SerializeField] 
        private ProjectileWeapon weapon;

        protected ProjectileWeapon Weapon => weapon;
        public virtual Faction CharacterFaction => Faction.None;

        protected Health health;
        protected MovementAgent movementAgent;
        public event Action<int> OnHealthChanged;
        public event Action<Character> OnDead;

        public bool IsAlive => health.CurrentHealthPoints > 0;

        private void Awake()
        {
            health = GetComponent<Health>();
            movementAgent = GetComponent<MovementAgent>();
            weapon.Setup(this);
        }
        
        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        protected virtual void OnFixedUpdate()
        {
            
        }

        public void ReceiveDamage(int damage)
        {
            if (health.ApplyDamage(damage))
            {
                OnHealthChanged?.Invoke(health.CurrentHealthPoints);
            }

            if (health.CurrentHealthPoints <= 0)
            {
                Die();
            }
        }

        public Faction Faction => CharacterFaction;

        protected virtual void OnDying()
        {
            gameObject.SetActive(false);
        }

        protected virtual void Move()
        {
            
        }

        private void Die()
        {
            OnDying();
            OnDead?.Invoke(this);
        }
    }
}