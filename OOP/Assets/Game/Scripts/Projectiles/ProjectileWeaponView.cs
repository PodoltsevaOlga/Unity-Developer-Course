using UnityEngine;

namespace Game.Projectiles
{
    [RequireComponent(typeof(ProjectileWeapon))]
    public class ProjectileWeaponView : MonoBehaviour
    {
        private ProjectileWeapon weaponData;
        
        [SerializeField]
        private ParticleSystem fireVFX;
        [SerializeField]
        private AudioClip fireSFX;
        [SerializeField]
        private AudioSource audioSource;
        
        private void Awake()
        {
            weaponData = GetComponent<ProjectileWeapon>();
        }

        private void OnEnable()
        {
            weaponData.OnFire += OnFireFXPlay;
        }

        private void OnDisable()
        {
            weaponData.OnFire -= OnFireFXPlay;
        }

        private void OnFireFXPlay()
        {
            if (fireVFX != null)
            {
                fireVFX.Play();
            }

            if (fireSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(fireSFX);
            }
        }
    }
}