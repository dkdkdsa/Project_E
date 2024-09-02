using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{

    [SerializeField] private float _speed;
    [SerializeField] private GameObject _bulletPrefab;

    private class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {

            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            var blt = GetEntity(authoring._bulletPrefab, TransformUsageFlags.Dynamic); 

            AddComponent(entity, new Player(authoring._speed, blt));

        }

    }

}
