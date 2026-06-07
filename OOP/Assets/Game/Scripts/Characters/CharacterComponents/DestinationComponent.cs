using UnityEngine;

namespace Game.Characters.CharacterComponents
{
    public class DestinationComponent : IResetComponent
    {
        private Vector2 currentDestination;
        private float stoppingDistance;

        public DestinationComponent(float _stoppingDistance)
        {
            stoppingDistance = _stoppingDistance;
        }

        public void SetDestination(Vector3 destination)
        {
            currentDestination = (Vector2)destination;
        }

        public Vector2 CalculateMovementVector(Vector3 currPosition)
        {
            var currentPosition = (Vector2)currPosition;
            if (!IsReachedDestination(currentPosition))
            {
                return currentDestination - currentPosition;
            }

            return Vector2.zero;
        }

        public bool IsReachedDestination(Vector3 currPosition)
        {
            var remainingDistance = currentDestination - (Vector2)currPosition;
            return remainingDistance.sqrMagnitude < stoppingDistance * stoppingDistance;
        }

        public void ResetValues()
        {
            currentDestination = Vector2.zero;
        }
    }
}