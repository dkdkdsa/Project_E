using Unity.Entities;

public struct TagComponent : IComponentData
{

    public Tag tag;

    public TagComponent(Tag tag)
    {

        this.tag = tag;

    }

}
