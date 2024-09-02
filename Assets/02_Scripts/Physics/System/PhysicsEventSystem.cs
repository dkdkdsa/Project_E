using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial class PhysicsEventSystem : SystemBase
{

    private EndFixedStepSimulationEntityCommandBufferSystem _ecbSystem;
    private SimulationSingleton _singleton;

    protected override void OnCreate()
    {

        _ecbSystem = World.GetOrCreateSystemManaged<EndFixedStepSimulationEntityCommandBufferSystem>();

    }

    protected override void OnStartRunning()
    {

        _singleton = SystemAPI.GetSingleton<SimulationSingleton>();

    }

    protected override void OnUpdate()
    {

        ClearBuffers();
        CheckCollision();
        CheckTrigger();

    }

    private void CheckCollision()
    {
        var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
        var collisonDataLookup = GetComponentLookup<CollisionData>(true);
        var tagLookup = GetComponentLookup<TagComponent>(true);

        var job = new CollisionCheckJob
        {

            ecb = ecb,
            orderKey = 0,
            collisonDataLookup = collisonDataLookup,
            tagLookup = tagLookup,

        };

        Dependency = job.Schedule(_singleton, Dependency);

        _ecbSystem.AddJobHandleForProducer(Dependency);

    }

    private void ClearBuffers()
    {

        Entities
            .ForEach((ref DynamicBuffer<PhysicsEventBuffer> buffer) =>
            {

                buffer.Clear();

            }).ScheduleParallel();

    }

    private void CheckTrigger()
    {

        var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
        var triggerLookup = GetComponentLookup<TriggerData>(true);
        var tagLookup = GetComponentLookup<TagComponent>(true);

        var job = new TriggerCheckJob
        {

            ecb = ecb,
            orderKey = 0,
            triggerLookup = triggerLookup,
            tagLookup = tagLookup,

        };

        Dependency = job.Schedule(_singleton, Dependency);

        _ecbSystem.AddJobHandleForProducer(Dependency);

    }

}

[BurstCompile]
public struct TriggerCheckJob : ITriggerEventsJob
{

    [ReadOnly] public ComponentLookup<TriggerData> triggerLookup;
    [ReadOnly] public ComponentLookup<TagComponent> tagLookup;
    public EntityCommandBuffer.ParallelWriter ecb;
    public int orderKey;

    public void Execute(TriggerEvent triggerEvent)
    {

        var entityA = triggerEvent.EntityA;
        var entityB = triggerEvent.EntityB;

        if (!triggerLookup.HasComponent(entityA) ||
           !tagLookup.HasComponent(entityA) ||
           !triggerLookup.HasComponent(entityB) ||
           !tagLookup.HasComponent(entityB)) return;

        var triggerA = triggerLookup.GetRefRO(entityA);
        var triggerB = triggerLookup.GetRefRO(entityB);
        var tagA = tagLookup.GetRefRO(entityA);
        var tagB = tagLookup.GetRefRO(entityB);

        CheckCasting(entityA, entityB, triggerA.ValueRO.ignoreTag, tagB.ValueRO.tag);
        CheckCasting(entityB, entityA, triggerB.ValueRO.ignoreTag, tagA.ValueRO.tag);

    }

    private void CheckCasting(Entity target, Entity other, Tag ignore, Tag hit)
    {

        var castingTag = hit & ~ignore;

        if (castingTag == Tag.Null)
            return;

        ecb.AppendToBuffer<PhysicsEventBuffer>(orderKey, target, new PhysicsEventBuffer
        {

            entityTag = castingTag,
            eventType = PhysicsEventType.Trigger,
            otherEntity = other,

        });

    }

}

[BurstCompile]
public struct CollisionCheckJob : ICollisionEventsJob
{
    [ReadOnly] public ComponentLookup<CollisionData> collisonDataLookup;
    [ReadOnly] public ComponentLookup<TagComponent> tagLookup;
    public EntityCommandBuffer.ParallelWriter ecb;
    public int orderKey;

    public void Execute(CollisionEvent collisionEvent)
    {

        var entityA = collisionEvent.EntityA;
        var entityB = collisionEvent.EntityB;

        if (!collisonDataLookup.HasComponent(entityA) ||
           !tagLookup.HasComponent(entityA) ||
           !collisonDataLookup.HasComponent(entityB) ||
           !tagLookup.HasComponent(entityB)) return;

        var triggerA = collisonDataLookup.GetRefRO(entityA);
        var triggerB = collisonDataLookup.GetRefRO(entityB);
        var tagA = tagLookup.GetRefRO(entityA);
        var tagB = tagLookup.GetRefRO(entityB);

        CheckCasting(entityA, entityB, triggerA.ValueRO.ignoreTag, tagB.ValueRO.tag);
        CheckCasting(entityB, entityA, triggerB.ValueRO.ignoreTag, tagA.ValueRO.tag);

    }

    private void CheckCasting(Entity target, Entity other, Tag ignore, Tag hit)
    {

        var castingTag = hit & ~ignore;

        if (castingTag == Tag.Null)
            return;

        ecb.AppendToBuffer<PhysicsEventBuffer>(orderKey, target, new PhysicsEventBuffer
        {

            entityTag = castingTag,
            eventType = PhysicsEventType.Collision,
            otherEntity = other,

        });

    }
}
