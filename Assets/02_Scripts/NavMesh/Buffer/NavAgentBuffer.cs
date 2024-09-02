using Unity.Entities;
using Unity.Mathematics;

public struct NavAgentBuffer : IBufferElementData
{

    public float3 wayPoint;
    public NavAgentBuffer(float3 wayPoint)
    { this.wayPoint = wayPoint; }

}
