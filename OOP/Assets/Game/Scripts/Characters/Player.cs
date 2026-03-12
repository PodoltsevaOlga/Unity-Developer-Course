using UnityEngine;

namespace Game.Characters
{
    public sealed class Player : CharacterShip
    {
        public override Faction CharacterFaction => Faction.Player;
        
        public void RequestFire()
        {
            weapon.TryToFire(transform.forward);
        }

        public void SetDirection(Vector2 direction)
        {
            movementAgent.SetDirection(direction);
        }
    }
}