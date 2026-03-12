using UnityEngine;

namespace Game.Projectiles
{
    [CreateAssetMenu(
        fileName = "ProjectileConfig",
        menuName = "Game/New ProjectileConfig"
    )]
    public class ProjectileStatConfig : ScriptableObject
    {
        [SerializeField] 
        private int damage;

        [SerializeField] 
        private float speed;

        public int Damage => damage;
        public float Speed => speed;
        
        
        //
        
        //public event Action<BulletData, Collider2D> OnTriggerEntered;
        //public Vector2 direction;

        //public GameObject blueVFX;
        //public GameObject redVFX;

        
    }
}