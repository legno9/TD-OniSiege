using UnityEngine;

public class EnemyPathMovement : EnemyMovement
{
    public override void Move(float speed)
    {
        if (_pathPoints == null || _pathPoints.Length < 2) return;

        float distanceToMove = speed * Time.deltaTime;

        while (distanceToMove > 0.0f && _currentPathIndex <= _pathPoints.Length)
        {
            if (_currentPathIndex >= _pathPoints.Length)
            {
                InvokeReachedEnd();
                return;
            }
            
            Vector2 targetPoint = _pathPoints[_currentPathIndex];
            Vector2 vectorToTarget = targetPoint - (Vector2)transform.position;
            float distanceToTarget = vectorToTarget.magnitude;

            if (distanceToTarget < 0.001f)
            {
                _currentPathIndex++;
                transform.position = targetPoint;
                continue;
            }

            Vector2 normalizedDirection = vectorToTarget.normalized;
            
            if (_lastDirection != normalizedDirection)
            {
                _lastDirection = normalizedDirection;
                InvokeDirectionChanged(normalizedDirection);
            }

            if (distanceToMove >= distanceToTarget)
            {
                transform.position = targetPoint;
                distanceToMove -= distanceToTarget;
                _currentPathIndex++;
            }
            else
            {
                transform.position += (Vector3)(normalizedDirection * distanceToMove);
                distanceToMove = 0.0f;
                break;
            }
        }
    }
}
