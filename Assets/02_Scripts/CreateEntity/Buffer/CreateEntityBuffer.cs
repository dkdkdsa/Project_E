using Unity.Entities;
using Unity.Mathematics;

public struct CreateEntityBuffer : IBufferElementData
{

    public Entity prefab;
    public float3 position;
    public quaternion rotation;

}
