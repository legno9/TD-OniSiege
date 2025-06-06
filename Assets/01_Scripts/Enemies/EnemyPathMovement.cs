using UnityEngine;
using System;

public class EnemyPathMovement : EnemyMovement
{
    public override void Move(float speed)
    {
        if (_pathPoints == null || _pathPoints.Length < 2) return;

        float distanceToMove = speed * Time.deltaTime;

        while (distanceToMove > 0.0f && _currentPathIndex < _pathPoints.Length - 1)
        {
            if (_currentPathIndex >= _pathPoints.Length - 1)
            {
                transform.position = _pathPoints[^1];
                _distanceAlongPath = _totalPathLength;
                // GameObjectManager.Instance.Player.ReceiveDamage(_playerDamage);
                Destroy(gameObject);
                return;
            }

            Vector2 targetPoint = _pathPoints[_currentPathIndex + 1];
            Vector2 vectorToTarget = targetPoint - (Vector2)transform.position;
            float distanceToTarget = vectorToTarget.magnitude;

            if (distanceToTarget < 0.001f)
            {
                _currentPathIndex++;
                transform.position = targetPoint;
                continue;
            }

            Vector2 normalizedDirection = vectorToTarget.normalized;
            InvokeDirectionChanged(normalizedDirection);

            if (distanceToMove >= distanceToTarget)
            {
                transform.position = targetPoint;
                distanceToMove -= distanceToTarget;
                _distanceAlongPath += distanceToTarget;
                _currentPathIndex++;
            }
            else
            {
                transform.position += (Vector3)(normalizedDirection * distanceToMove);
                _distanceAlongPath += distanceToMove;
                distanceToMove = 0.0f;
                break;
            }
        }
    }
}
