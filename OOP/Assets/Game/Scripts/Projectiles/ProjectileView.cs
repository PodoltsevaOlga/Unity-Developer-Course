using System;
using UnityEngine;

namespace Game.Projectiles
{
    public class ProjectileView : MonoBehaviour
    {
        private Projectile projectileData;
        [SerializeField]
        private GameObject explosionPrefab;

        private void Awake()
        {
            projectileData = GetComponent<Projectile>();
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