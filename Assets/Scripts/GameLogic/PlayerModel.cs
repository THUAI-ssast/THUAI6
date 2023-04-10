using System;
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
    public const int MaxBombCount = 1;

    public const float ShootInterval = 0.1f;
    public const float ChangeBulletTime = 2.5f;
    public const float PlaceBombTime = 2.0f;
    public const float ModifyPortalTime = 0.5f;
    public const float ActivatePortalTime = 1.0f;
    public const float RespawnTime = 8.0f;

    public const int BulletDamage = 10;
    public const float BulletRange = 20.0f;
    // max distance between the player and the cell where the player can place a bomb
    public const float MaxBombDistance = 5.0f;

    public event EventHandler<Vector2> PositionChangedEvent;
    public event EventHandler<Team> DiedEvent;

    public int id
    { get; private set; } // unique for each player
    public Team team { get; private set; }
    public PlayerState state { get; private set; } = new PlayerState();
    public int hp = MaxHp;
    public int ammo = MaxAmmo;
    public int bombCount = MaxBombCount;
    public Vector2 position;
    public float rotation = 0.0f; // degree

    public static Vector2 GetPositionFromCellPosition(Vector2Int cell)
    {
        return new Vector2(cell.x + 0.5f, cell.y + 0.3f);
    }

    public PlayerModel(int id, Team team, Vector2 position)
    {
        this.id = id;
        this.team = team;
        this.position = position;
    }

    public void Respawn()
    {
        hp = MaxHp;
        ammo = MaxAmmo;
        bombCount = MaxBombCount;
        state.isAlive = true;
        state.EnableAll();

        SetPosition(GetPositionFromCellPosition(MapModel.Instance.GetRandomPosition()));
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
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    public void Shoot()
    {
        ammo--;
        state.isShooting = true;
        state.canShoot = false;
        DelayedFunctionCaller.CallAfter(PlayerModel.ShootInterval, () =>
        {
            state.isShooting = false;
            state.canShoot = true;
        });
    }

    public void ChangeBullet()
    {
        state.isChangingBullet = true;
        state.DisableAll();
        state.canMove = true;
        state.canRotate = true;

        DelayedFunctionCaller.CallAfter(PlayerModel.ChangeBulletTime, () =>
        {
            ammo = PlayerModel.MaxAmmo;

            state.isChangingBullet = false;
            state.EnableAll();
        });
    }

    public void PlaceBombBegin()
    {
        state.isPlacingBomb = true;
        state.DisableAll();
    }

    public void PlaceBombEnd()
    {
        bombCount--;
        state.isPlacingBomb = false;
        state.EnableAll();
    }

    public void ModifyPortalBegin()
    {
        state.isModifyingPortal = true;
        state.DisableAll();
    }

    public void ModifyPortalEnd()
    {
        state.isModifyingPortal = false;
        state.EnableAll();
    }

    public void ActivatePortalBegin()
    {
        state.isActivatingPortal = true;
        state.DisableAll();
    }

    public void ActivatePortalEnd()
    {
        state.isActivatingPortal = false;
        state.EnableAll();
    }

    private void Die()
    {
        state.isAlive = false;
        state.DisableAll();

        DiedEvent?.Invoke(this, team);
    }

}
