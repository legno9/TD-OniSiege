using UnityEngine;
using System;

public abstract class EnemyMovement : MonoBehaviour
{
    public event Action<Vector2> OnDirectionChanged;
    public event Action OnReachedEnd;

    protected Vector2[] _pathPoints;
    protected int _currentPathIndex;
    protected Vector2 _lastDirection;
    
    public void SetPath( Vector2[] pathPoints )
    {
        if (pathPoints is not { Length: > 1 }) return;
        
        _pathPoints = pathPoints;
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
}
