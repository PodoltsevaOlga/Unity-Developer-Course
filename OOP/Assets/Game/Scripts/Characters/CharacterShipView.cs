using DG.Tweening;
using UnityEngine;

namespace Game.Characters
{
    [RequireComponent(typeof(CharacterShip))]
    public class CharacterShipView : MonoBehaviour
    {
        private CharacterShip characterShipData;
        private Material material;
        private Tweener damageAnimation;
        
        [SerializeField]
        private CharacterShipViewConfig viewConfiguration;
        [SerializeField]
        private Renderer viewRenderer;
        [SerializeField]
        private AudioClip damageSFX;
        [SerializeField]
        private AudioSource audioSource;
        
        private void Awake()
        {
            characterShipData = GetComponent<CharacterShip>();
            material = new Material(viewConfiguration.MaterialPrefab);
            viewRenderer.material = material;
        }

        private void OnEnable()
        {
            characterShipData.OnHealthChanged += AnimateDamage;
            characterShipData.OnDead += AnimateDeath;
        }

        private void OnDisable()
        {
            characterShipData.OnHealthChanged -= AnimateDamage;
            characterShipData.OnDead -= AnimateDeath;
        }

        private void AnimateDamage(int _)
        {
            if (damageAnimation.IsActive())
                damageAnimation.Kill();

            damageAnimation = DOVirtual.Float(
                0f,
                1f,
                viewConfiguration.HitDuration,
                progress => material.SetFloat(viewConfiguration.HitPropertyName,
                    viewConfiguration.HitAnimationCurve.Evaluate(progress))
            ).SetLink(viewRenderer.gameObject);

            if (damageSFX != null && audioSource != null)
                audioSource.PlayOneShot(damageSFX);
        }

        private void AnimateDeath(CharacterShip _)
        {
            ParticleSystem prefab = viewConfiguration.DestroyEffectPrefab;
            Instantiate(prefab, this.transform.position, prefab.transform.rotation);
        }
    }
}