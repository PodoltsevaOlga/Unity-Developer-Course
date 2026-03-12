using UnityEngine;

namespace Game.Projectiles
{
    [CreateAssetMenu(
        fileName = "ProjectileViewConfig",
        menuName = "Game/New ProjectileViewConfig"
    )]
    public class ProjectileViewConfig : ScriptableObject
    {
        [SerializeField]
        private GameObject explosionPrefab;
        [SerializeField] 
        private GameObject projectileVFX;

        public GameObject ExplosionPrefab => explosionPrefab;
        public GameObject ProjectileVFX => projectileVFX;
    }
}