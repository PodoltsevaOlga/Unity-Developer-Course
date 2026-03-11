using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class MovementAgent : MonoBehaviour
    {
        public event Action<Vector3> OnMoved;
        
        [SerializeField]
        private Rigidbody2D rigidbody;

        [SerializeField]
        private float speed;
        
        [SerializeField]
        private float stoppingDistance = 0.25f;

        private Vector2? currentDirection;
        private Vector2? currentDestination;

        public void SetSpeed(float _speed) => speed = _speed;

        public void SetDirection(Vector2 direction) => currentDirection = direction;
        public void SetDestination(Vector2 destination) => currentDestination = destination;
        
        public bool IsReachedDestination(out Vector2 remainingDistance)
        {
            remainingDistance = currentDestination.Value - (Vector2)transform.position;
            return Mathf.Abs(remainingDistance.sqrMagnitude -
                             stoppingDistance * stoppingDistance) < Mathf.Epsilon;
        }

        public void FixedUpdate()
        {
            if (currentDestination.HasValue)
            {
                if (!IsReachedDestination(out var remainingDistance))
                {
                    currentDirection = remainingDistance;
                }
            }
            
            if (!currentDirection.HasValue)
                return;

            Vector2 direction = currentDirection.Value;
            Vector2 newPosition = rigidbody.position + direction * (speed * Time.fixedDeltaTime);
            rigidbody.MovePosition(newPosition);
            currentDirection = null;
            
            this.OnMoved?.Invoke(direction);
        }
    }
}