using Unity.Entities;
using UnityEngine;

public class HpAuthoring : MonoBehaviour
{

    [SerializeField] private float _hp;

    private class HpBaker : Baker<HpAuthoring>
    {
        public override void Bake(HpAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Hp { hp = authoring._hp });

        }

    }

}
