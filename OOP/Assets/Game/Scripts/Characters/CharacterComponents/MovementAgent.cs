using System;
using Modules.Utils;
using UnityEngine;

namespace Game.Characters.CharacterComponents
{
    [Serializable]
    public class MovementAgent
    {
        public event Action<Vector3> OnMoved;
        
        private Rigidbody2D rigidbody;

        private float moveSpeed;
        private float moveRotationAngle;
        private float stoppingDistance;
        private TransformBounds allowedArea;

        private Vector2? currentDirection;
        private Vector2? currentDestination;
        private readonly float movementPrecision = 0.00001f;
        
        public Vector2 LastRealMovement { get; private set; }

        public MovementAgent(Rigidbody2D _rigidbody, float _moveSpeed,
            float _moveRotation, float _stoppingDistance, TransformBounds _bounds)
        {
            rigidbody = _rigidbody;
            moveSpeed = _moveSpeed;
            moveRotationAngle = _moveRotation;
            stoppingDistance = _stoppingDistance;
            allowedArea = _bounds;
        }

        public void SetSpeed(float speed) => moveSpeed = speed;

        public void SetDirection(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) < movementPrecision)
            {
                direction.x = 0f;
            }
            if (Mathf.Abs(direction.y) < movementPrecision)
            {
                direction.y = 0f;
            }
            
            if (direction == Vector2.zero)
            {
                currentDirection = null;
            }
            else 
            {
                currentDirection = direction;
            }
        }

        public void SetDestination(Vector2 destination) => currentDestination = destination;
        
        public bool IsReachedDestination(out Vector2 remainingDistance)
        {
            remainingDistance = currentDestination.Value - (Vector2)rigidbody.transform.position;
            return Mathf.Abs(remainingDistance.sqrMagnitude -
                             stoppingDistance * stoppingDistance) < movementPrecision;
        }

        public void OnFixedUpdate()
        {
            LastRealMovement = Vector2.zero;
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
            Vector2 newPosition = rigidbody.position + direction * (moveSpeed * Time.fixedDeltaTime);
            rigidbody.MovePosition(newPosition);
            currentDirection = null;
            LastRealMovement = direction;
            
            this.OnMoved?.Invoke(direction);
        }

        public void OnLateUpdate()
        {
            AnimateMovement(Time.deltaTime);
            if (allowedArea != null)
            {
                rigidbody.transform.position =
                    allowedArea.ClampInBounds(rigidbody.transform.position);
            }

        }
        
        private void AnimateMovement(float deltaTime)
        {
            Vector3 shipAngles = rigidbody.transform.localEulerAngles;
            shipAngles.x = moveRotationAngle * LastRealMovement.y;
            shipAngles.y = moveRotationAngle / 2 * LastRealMovement.x * -1f;
            
            Quaternion shipRotation = Quaternion.Euler(shipAngles);
            float t = moveSpeed * deltaTime;
            rigidbody.transform.localRotation =
                Quaternion.Lerp(rigidbody.transform.localRotation, shipRotation, t);
        }
    }
}