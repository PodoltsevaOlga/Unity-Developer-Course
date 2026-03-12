using UnityEngine;

namespace Game.Characters
{
    public sealed class Player : CharacterShip
    {
        public override Faction CharacterFaction => Faction.Player;
        
        public void RequestFire()
        {
            weapon.TryToFire(null);
        }

        public void SetDirection(Vector2 direction)
        {
            movementAgent.SetDirection(direction);
        }

        protected override void OnLateUpdate()
        {
            movementAgent.OnLateUpdate();
        }
        
        protected override void OnFixedUpdate()
        {
            movementAgent.OnFixedUpdate();
        }
    }
}