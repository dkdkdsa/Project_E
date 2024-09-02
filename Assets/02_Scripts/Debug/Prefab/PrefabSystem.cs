using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PrefabSystem : SystemBase
{

    protected override void OnUpdate()
    {


        if (Input.GetKey(KeyCode.L))
        {
            Entities
                .WithStructuralChanges()
                .ForEach((ref PrefabCreater creater) =>
                {

                    var entity = EntityManager.Instantiate(creater.prefab);
                    var trm = EntityManager.GetComponentData<LocalTransform>(entity);

                    EntityManager.AddBuffer<NavAgentBuffer>(entity);

                    trm.Position = new float3(3, 1f, -3);

                    EntityManager.SetComponentData(entity, trm);

                    if (!EntityManager.HasComponent<NavAgent>(entity)) return;

                    var data = EntityManager.GetComponentData<NavAgent>(entity);
                    data.targetPoint = new float3(20, 0, 20);
                    data.currentBufferIdx = 0;

                    if (data.havePath)
                    {

                        EntityManager.GetBuffer<NavAgentBuffer>(entity).Clear();
                        data.havePath = false;

                    }

                    data.needFind = true;

                    EntityManager.SetComponentData<NavAgent>(entity, data);

                }).Run();

        }

    }

}
