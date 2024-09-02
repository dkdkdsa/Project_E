using Unity.Entities;

public enum PhysicsEventType
{

    Trigger,
    Collision

}

public struct PhysicsEventBuffer : IBufferElementData
{

    public Tag entityTag;
    public PhysicsEventType eventType;
    public Entity otherEntity;

}
