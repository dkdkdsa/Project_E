using Unity.Entities;
using UnityEngine;

public class AttackAuthoring : MonoBehaviour
{

    [SerializeField] private float _attackCooldown;

    private class AttackBaker : Baker<AttackAuthoring>
    {
        public override void Bake(AttackAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Attack { attackCooldown = authoring._attackCooldown });

        }

    }

}
