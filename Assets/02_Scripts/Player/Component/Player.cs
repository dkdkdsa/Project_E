using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Player : IComponentData
{

    public Entity bulletPrefab;
    public float speed;

    public Player(float speed, Entity blt)
    {

        this.speed = speed;
        bulletPrefab = blt;

    }

}