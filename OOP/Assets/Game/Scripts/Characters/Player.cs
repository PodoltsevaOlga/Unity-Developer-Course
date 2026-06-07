using System;
using Game.Characters.CharacterComponents;
using Game.Projectiles;
using Modules.Utils;
using UnityEngine;

namespace Game.Characters
{
    public sealed class Player : CharacterShip
    {
        [SerializeField]
        private TransformBounds allowedArea;

        public override Faction CharacterFaction => Faction.Player;
        private BoundsLimiterComponent boundsLimiter;

        [SerializeField]
        private ProjectileSpawner projectileSpawner;

        public event Action OnPlayerDead;

        protected override void OnAwake()
        {
            boundsLimiter = new BoundsLimiterComponent(allowedArea, shipRigidbody);
            weapon.SetProjectileSpawner(projectileSpawner);
        }

        public void RequestFire()
        {
            weapon.TryToFire(null);
        }

        public void SetDirection(Vector2 direction)
        {
            movementAgent.SetDirection(direction);
        }

        private void FixedUpdate()
        {
            movementAgent.MoveOnFixedUpdate();
        }

        private void LateUpdate()
        {
            boundsLimiter.ApplyLimitToPosition();
        }

        protected override void OnDying()
        {
            base.OnDying();
            OnPlayerDead?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
