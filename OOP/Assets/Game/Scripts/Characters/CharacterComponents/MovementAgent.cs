using System;
using UnityEngine;

namespace Game.Characters.CharacterComponents
{
    [Serializable]
    public class MovementAgent
    {
        public event Action<Vector3> OnMoved;

        private float moveSpeed;
        
        private Rigidbody2D rigidbody;

        private Vector2 currentDirection;
        private readonly float movementPrecision = 0.00001f;
        
        public Vector2 LastMovement { get; private set; }

        public MovementAgent(Rigidbody2D _rigidbody, float _moveSpeed)
        {
            rigidbody = _rigidbody;
            moveSpeed = _moveSpeed;
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
            
            currentDirection = direction;
            
        }
        
        public void MoveOnFixedUpdate()
        {
            LastMovement = Vector2.zero;
            
            if (currentDirection == Vector2.zero)
                return;

            Vector2 newPosition = rigidbody.position + currentDirection * (moveSpeed * Time.fixedDeltaTime);
            rigidbody.MovePosition(newPosition);
            LastMovement = currentDirection;
            OnMoved?.Invoke(LastMovement);
            currentDirection = Vector2.zero;
        }
    }
}