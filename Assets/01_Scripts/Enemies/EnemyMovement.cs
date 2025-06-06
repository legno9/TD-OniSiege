using UnityEngine;
using System;

public abstract class EnemyMovement : MonoBehaviour
{
    public event Action<Vector2> OnDirectionChanged;

    protected Vector2[] _pathPoints;
    protected float _distanceAlongPath;
    protected float _totalPathLength;
    protected int _currentPathIndex;
    
    public void SetPathLength( Vector2[] pathPoints )
    {
        _pathPoints = pathPoints;
        _totalPathLength = 0.0f;
        
        if (_pathPoints is not { Length: > 1 }) return;
        
        for (int i = 0; i < _pathPoints.Length - 1; ++i)
        {
            _totalPathLength += Vector2.Distance(_pathPoints[i], _pathPoints[i + 1]);
        }
    }

    public abstract void Move(float speed);
    
    protected void InvokeDirectionChanged(Vector2 newDirection)
    {
        OnDirectionChanged?.Invoke(newDirection);
    }
}
