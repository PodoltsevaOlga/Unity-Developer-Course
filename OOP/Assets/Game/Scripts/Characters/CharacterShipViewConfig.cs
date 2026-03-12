using UnityEngine;

namespace Game.Characters
{
    [CreateAssetMenu(menuName = "Game/New CharacterShipViewConfig")]
    public sealed class CharacterShipViewConfig : ScriptableObject
    {
        [field: SerializeField]
        public Material MaterialPrefab { get; private set; }

        [Header("Damage")]
        [field: SerializeField]
        public AnimationCurve HitAnimationCurve { get; private set; }

        [field: SerializeField]
        public string HitPropertyName { get; private set; } = "_HitBlend";

        [field: SerializeField]
        public float HitDuration { get; private set; } = 0.2f;

        [Header("Destroy")]
        [field: SerializeField]
        public ParticleSystem DestroyEffectPrefab { get; private set; }
    }
}