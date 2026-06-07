using Game.Characters.CharacterComponents;
using UnityEngine;

namespace Game.Characters
{
    public sealed class Player : CharacterShip
    {
        public override Faction CharacterFaction => Faction.Player;
        private BoundsLimiterComponent boundsLimiter;

        protected override void OnAwake()
        {
            boundsLimiter = new BoundsLimiterComponent(allowedArea, shipRigidbody);
        }
        
        public void RequestFire()
        {
            weapon.TryToFire(null);
        }

        public void SetDirection(Vector2 direction)
        {
            movementAgent.SetDirection(direction);
        }

        protected override void OnFixedUpdate()
        {
            movementAgent.MoveOnFixedUpdate();
        }
        
        protected override void OnLateUpdate()
        {
            boundsLimiter.ApplyLimitToPosition();
        }
    }
}