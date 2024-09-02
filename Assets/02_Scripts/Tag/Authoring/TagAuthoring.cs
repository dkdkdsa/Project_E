using Unity.Entities;
using UnityEngine;

public class TagAuthoring : MonoBehaviour
{

    [SerializeField] private Tag _tag;

    private class TagBaker : Baker<TagAuthoring>
    {

        public override void Bake(TagAuthoring authoring)
        {

            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new TagComponent(authoring._tag));

        }

    }

}
