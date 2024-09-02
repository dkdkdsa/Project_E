using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{

    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private float _lifeTime;

    private class BulletBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var bullet = new Bullet 
            { 
                damage = authoring._damage, 
                speed = authoring._speed,
                lifeTime = authoring._lifeTime,
            };

            AddComponent(entity, bullet);

        }

    }

}
