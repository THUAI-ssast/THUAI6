using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public bool isAlive = true;
    public bool isShooting = false;
    public bool isChangingBullet = false;
    public bool isPlacingBomb = false;
    public bool isModifyingPortal = false;
    public bool isActivatingPortal = false;

    public bool canMove = true;
    public bool canRotate = true;
    public bool canShoot = true;
    public bool canChangeBullet = true;
    public bool canPlaceBomb = true;
    public bool canModifyPortal = true;
    public bool canActivatePortal = true;

    public void DisableAll()
    {
        canMove = false;
        canRotate = false;
        canShoot = false;
        canChangeBullet = false;
        canPlaceBomb = false;
        canModifyPortal = false;
        canActivatePortal = false;
    }

    public void EnableAll()
    {
        canMove = true;
        canRotate = true;
        canShoot = true;
        canChangeBullet = true;
        canPlaceBomb = true;
        canModifyPortal = true;
        canActivatePortal = true;
    }

    public void InterruptAll()
    {
        isShooting = false;
        isChangingBullet = false;
        isPlacingBomb = false;
        isModifyingPortal = false;
        isActivatingPortal = false;
    }
}

public class PlayerModel
{
    // const properties that same for all players
    // TODO: to be decided
    // unit: meter, second, degree
    public const float MaxVelocity = 4.0f;
    public const float Acceleration = 10.0f;
    public const float RotationSpeed = 600.0f;

    public const int MaxHp = 100;
    public const int MaxAmmo = 30;
    public const int GunDamage = 10;
    public const float ShootInterval = 0.1f; // seconds
    // max distance between the player and the cell where the player can place a bomb
    public const float MaxBombDistance = 5.0f;

    public event EventHandler<Vector2> PositionChangedEvent;
    public event EventHandler DiedEvent;

    public int id
    { get; private set; } // unique for each player
    public Team team { get; private set; }
    public PlayerState state { get; private set; } = new PlayerState();
    public int hp = MaxHp;
    public int ammo = MaxAmmo;
    public Vector2 position;
    public float rotation = 0.0f; // degree

    public float respawnTimeLeft = 0.0f; // seconds. 0 means not respawning

    public PlayerModel(int id, Team team, Vector2 position)
    {
        this.id = id;
        this.team = team;
        this.position = position;
    }


    public void SetPosition(Vector2 position)
    {
        this.position = position;
        PositionChangedEvent?.Invoke(this, position);
    }

    public void MoveBy(Vector2 offset)
    {
        position += offset;
        PositionChangedEvent?.Invoke(this, position);
    }

    public void Hurt(int damage)
    {
        // TODO: to be implemented

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        state.isAlive = false;
        state.InterruptAll();
        state.DisableAll();

        // The enemy team gets a point
        GameModel.Instance.AddScore(team.GetOppositeTeam(), 1);

        DiedEvent?.Invoke(this, EventArgs.Empty);
    }

}
