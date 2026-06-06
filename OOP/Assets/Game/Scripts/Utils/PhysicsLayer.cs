using Game.Characters;

namespace Game.Utils
{
    public enum PhysicsLayer
    {
        DEFAULT = 0,
        PLAYER = 10,
        ENEMY = 11,
        ENEMY_PROJECTILE = 13,
        PLAYER_PROJECTILE = 14
    }

    public class FactionToPhysicsLayerResolver
    {
        public static int Resolve(Faction faction)
        {
            switch (faction)
            {
                case Faction.Enemy:
                    return (int)PhysicsLayer.ENEMY_PROJECTILE;
                case Faction.Player:
                    return (int)PhysicsLayer.PLAYER_PROJECTILE;
                default:
                    return (int)PhysicsLayer.DEFAULT;
            }
        }
    }
}