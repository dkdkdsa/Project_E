using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Experimental.AI;

public struct NavAgent : IComponentData
{

    public float3 targetPoint;
    public float speed;
    public bool havePath;
    public bool needFind;
    public bool inJob;
    public int currentBufferIdx;

    public NavAgent(float speed)
    {

        this.speed = speed;
        targetPoint = float3.zero;
        currentBufferIdx = 0;
        havePath = false;
        needFind = false;
        inJob = false;

    }

}