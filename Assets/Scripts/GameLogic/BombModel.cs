using System;
using UnityEngine;

public class BombModel
{
    // TODO: to be decided
    public const float ExplosionTime = 5.0f;
    public const float ExplosionRadius = 2.0f;
    public const int ExplosionDamage = 70;

    public event EventHandler<Vector2Int> PositionChangedEvent;

    public Vector2Int position { get; private set; }

    public void SetPosition(Vector2Int position)
    {
        this.position = position;
        PositionChangedEvent?.Invoke(this, position);
    }

    public BombModel(Vector2Int position)
    {
        this.position = position;
    }
}
