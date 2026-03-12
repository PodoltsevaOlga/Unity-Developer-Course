using UnityEngine;

namespace Game.Projectiles
{
    [RequireComponent(typeof(Projectile))]
    public class ProjectileView : MonoBehaviour
    {
        private Projectile projectileData;
        private GameObject explosionPrefab;
        [SerializeField] 
        private ProjectileViewConfig viewConfiguration;

        [SerializeField] private Transform visualContainer;

        private void Awake()
        {
            projectileData = GetComponent<Projectile>();
            if (viewConfiguration != null)
            {
                if (viewConfiguration.ProjectileVFX != null)
                {
                    Instantiate(viewConfiguration.ProjectileVFX, visualContainer);
                }

                explosionPrefab = viewConfiguration.ExplosionPrefab;
            }
        }
        

        private void OnEnable()
        {
            if (projectileData != null)
            {
                projectileData.OnDealDamage += Explode;
            }
        }

        private void OnDisable()
        {
            if (projectileData != null)
            {
                projectileData.OnDealDamage -= Explode;
            }
        }

        private void Explode(Projectile _)
        {
            Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);
        }
    }
}