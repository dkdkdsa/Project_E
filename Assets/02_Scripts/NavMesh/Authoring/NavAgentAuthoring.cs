using Unity.Entities;
using UnityEngine;

public class NavAgentAuthoring : MonoBehaviour
{

    [SerializeField] private float _speed;

    private class NavAgentBaker : Baker<NavAgentAuthoring>
    {

        public override void Bake(NavAgentAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<NavAgent>(entity, new(authoring._speed));
            AddBuffer<NavAgentBuffer>(entity);

        }

    }

}
