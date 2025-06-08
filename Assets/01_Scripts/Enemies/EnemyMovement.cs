using UnityEngine;
using System;

public abstract class EnemyMovement : MonoBehaviour
{
    public event Action<Vector2> OnDirectionChanged;
    public event Action OnReachedEnd;

    protected float _pathProgress;
    protected float _totalPathLength;
    protected Vector2[] _pathPoints;
    protected int _currentPathIndex;
    protected Vector2 _lastDirection;
    
    public void SetPath( Vector2[] pathPoints )
    {
        if (pathPoints is not { Length: > 1 }) return;
        
        _pathPoints = pathPoints;

        for (int i = 0; i < _pathPoints.Length - 1; i++)
        {
            _totalPathLength += Vector2.Distance(_pathPoints[i], _pathPoints[i + 1]);
        }
    }

    public float GetPathProgress()
    {
        if (_totalPathLength <= 0.0f) return 0.0f;
        
        return _pathProgress / _totalPathLength;
    }
    
    public abstract void Move(float speed);
    
    protected void InvokeDirectionChanged(Vector2 newDirection)
    {
        OnDirectionChanged?.Invoke(newDirection);
    }
    
    protected void InvokeReachedEnd()
    {
        OnReachedEnd?.Invoke();
    }

    protected void OnDisable()
    {
        _pathPoints = null;
        _currentPathIndex = 0;
        _pathProgress = 0f;
        _totalPathLength = 0f;
        _lastDirection = Vector2.zero;
    }
}
