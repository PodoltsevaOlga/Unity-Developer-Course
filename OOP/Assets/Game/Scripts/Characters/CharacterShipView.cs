using DG.Tweening;
using UnityEngine;

namespace Game.Characters
{
    [RequireComponent(typeof(CharacterShip))]
    public class CharacterShipView : MonoBehaviour
    {
        private Material material;
        private Tweener damageAnimation;
        
        [SerializeField]
        private CharacterShipViewConfig viewConfiguration;
        [SerializeField]
        private CharacterShip characterShipData;
        [SerializeField]
        private Renderer viewRenderer;
        [SerializeField]
        private AudioClip damageSFX;
        [SerializeField]
        private AudioSource audioSource;
        
        private void Awake()
        {
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

        private void LateUpdate()
        {
            AnimateMovement(Time.deltaTime);
        }

        private void AnimateMovement(float deltaTime)
        {
            Vector2 lastMovement = characterShipData.GetLastMovement();
            Vector3 shipAngles = transform.localEulerAngles;
            shipAngles.x = viewConfiguration.MoveRotationAngle * lastMovement.y;
            shipAngles.y = viewConfiguration.MoveRotationAngle / 2 * lastMovement.x * -1f;
            
            Quaternion shipRotation = Quaternion.Euler(shipAngles);
            float t = viewConfiguration.MoveRotationSpeed * deltaTime;
            transform.localRotation =
                Quaternion.Lerp(transform.localRotation, shipRotation, t);
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