using Unity.Entities;
using UnityEngine;

public class PhysicsDataAuthoring : MonoBehaviour
{

    [SerializeField] private bool _enableTrigger;
    [SerializeField] private bool _enableCollision;
    [SerializeField] private Tag _ignoreTag;

    private class TriggerDataBaker : Baker<PhysicsDataAuthoring>
    {

        public override void Bake(PhysicsDataAuthoring authoring)
        {

            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddBuffer<PhysicsEventBuffer>(entity);

            if(authoring._enableTrigger)
                AddComponent(entity, new TriggerData(authoring._ignoreTag));

            if(authoring._enableCollision)
                AddComponent(entity, new CollisionData(authoring._ignoreTag));

        }

    }
}
