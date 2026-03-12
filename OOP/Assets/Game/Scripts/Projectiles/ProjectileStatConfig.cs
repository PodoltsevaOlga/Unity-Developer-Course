using UnityEngine;

namespace Game.Projectiles
{
    [CreateAssetMenu(
        fileName = "ProjectileStatConfig",
        menuName = "Game/New ProjectileStatConfig"
    )]
    public class ProjectileStatConfig : ScriptableObject
    {
        [SerializeField] 
        private int damage;

        [SerializeField] 
        private float speed;

        public int Damage => damage;
        public float Speed => speed;
    }
}