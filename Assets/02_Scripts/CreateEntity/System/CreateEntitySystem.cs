using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct EntityManagedSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        state.EntityManager.CreateSingletonBuffer<CreateEntityBuffer>();
        state.EntityManager.CreateSingletonBuffer<DestroyEntityBuffer>();

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        CreateEntity(ref state);
        DestroyEntity(ref state);
        ClearBuffer(ref state);

    }

    [BurstCompile]
    private void DestroyEntity(ref SystemState state)
    {

        using EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        var buf = SystemAPI.GetSingletonBuffer<DestroyEntityBuffer>();

        foreach (var elem in buf)
        {

            ecb.DestroyEntity(elem.entity);

        }

        ecb.Playback(state.EntityManager);

    }

    [BurstCompile]
    private void CreateEntity(ref SystemState state)
    {

        using EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        var buf = SystemAPI.GetSingletonBuffer<CreateEntityBuffer>();

        foreach (var item in buf)
        {

            var entity = ecb.Instantiate(item.prefab);
            var trm = new LocalTransform
            {

                Position = item.position,
                Rotation = item.rotation,
                Scale = 1,

            };
            ecb.SetComponent(entity, trm);

        }

        ecb.Playback(state.EntityManager);


    }

    [BurstCompile]
    private void ClearBuffer(ref SystemState state)
    {

        var cbuf = SystemAPI.GetSingletonBuffer<CreateEntityBuffer>();
        cbuf.Clear();

        var dbuf = SystemAPI.GetSingletonBuffer<DestroyEntityBuffer>();
        dbuf.Clear();

    }

}