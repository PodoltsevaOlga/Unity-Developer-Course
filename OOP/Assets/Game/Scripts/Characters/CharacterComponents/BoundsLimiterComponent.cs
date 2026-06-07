using Modules.Utils;
using UnityEngine;

namespace Game.Characters.CharacterComponents
{
    public class BoundsLimiterComponent
    {
        private TransformBounds allowedArea;
        private Rigidbody2D rigidbody;

        public BoundsLimiterComponent(TransformBounds bounds, Rigidbody2D rig)
        {
            allowedArea = bounds;
            rigidbody = rig;
        }
        
        public void ApplyLimitToPosition()
        {
            if (allowedArea != null)
            {
                rigidbody.transform.position =
                    allowedArea.ClampInBounds(rigidbody.transform.position);
            }
        }
    }
}