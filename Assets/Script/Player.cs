using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // const properties that same for all players
    // TODO: to be decided
    public const float RespawnTime = 5.0f; // seconds
    public const float Speed = 1.0f; // per second
    public const float RotationSpeed = 600.0f; // degree per second
    public const int MaxHp = 100;
    public const int MaxAmmo = 30;
    public const int AttackDamage = 10;

    Team team;
    int id;
    // Position and rotation could be replaced by Transform in Unity
    PlayerState state;
    int hp;
    int ammo;
    float respawnTimeLeft;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // TODO: methods to be implemented
}
