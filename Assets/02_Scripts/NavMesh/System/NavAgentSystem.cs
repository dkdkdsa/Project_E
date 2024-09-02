using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct NavAgentSystem : ISystem
{

    private NavMeshWorld _navWord;
    private NavMeshQuery _query;

    public void OnCreate(ref SystemState state)
    {

        _navWord = NavMeshWorld.GetDefaultWorld();
        _query = new NavMeshQuery(_navWord, Allocator.Persistent, short.MaxValue);

    }

    public void OnUpdate(ref SystemState state)
    {

        PathFind(ref state);
        MoveAgent(ref state);
        CheckWayPoint(ref state);
        CheckOutBounce(ref state);

    }

    private void CheckOutBounce(ref SystemState state)
    {

        var job = new CheckOutBounceJob();

        state.Dependency = job.ScheduleParallel(state.Dependency);

    }

    private void CheckWayPoint(ref SystemState state)
    {

        var job = new CheckWayPointJob();

        state.Dependency = job.ScheduleParallel(state.Dependency);

    }

    private void MoveAgent(ref SystemState state)
    {

        var job = new MoveAgentJob
        {

            dt = SystemAPI.Time.DeltaTime

        };

        state.Dependency = job.ScheduleParallel(state.Dependency);

    }

    private void PathFind(ref SystemState state)
    {

        var job = new PathFindingJob
        {

            query = _query

        };

        state.Dependency = job.Schedule(state.Dependency);

    }

    public void OnDestroy(ref SystemState state)
    {

        _query.Dispose();

    }

    [BurstCompile]
    public partial struct PathFindingJob : IJobEntity
    {

        public NavMeshQuery query;

        public void Execute(
            ref NavAgent agent,
            ref DynamicBuffer<NavAgentBuffer> buffer,
            in LocalToWorld trm)
        {

            if (!agent.needFind)
                return;

            var start = query.MapLocation(trm.Position, Vector3.one * 2, 0);
            var end = query.MapLocation(agent.targetPoint, Vector3.one * 2, 0);
            var status = PathQueryStatus.Failure;

            if (query.IsValid(start) && query.IsValid(end))
            {

                status = query.BeginFindPath(start, end);

                if (status == PathQueryStatus.InProgress)
                    query.UpdateFindPath(100, out _);

                status = query.EndFindPath(out int size);

                if (status == PathQueryStatus.Success)
                {

                    buffer.Clear();

                    var polys = new NativeArray<PolygonId>(size, Allocator.Temp);
                    var paths = new NativeArray<NavMeshLocation>(100, Allocator.Temp);
                    var flags = new NativeArray<StraightPathFlags>(100, Allocator.Temp);
                    var verts = new NativeArray<float>(100, Allocator.Temp);
                    int pathLen = 0;

                    query.GetPathResult(polys);

                    PathUtils.FindStraightPath(query, trm.Position,
                        agent.targetPoint, polys, size, ref paths, ref flags, ref verts, ref pathLen, 100);
                    for (int i = 0; i < pathLen; i++)
                    {

                        buffer.Add(new NavAgentBuffer(paths[i].position));

                    }

                    agent.havePath = true;
                    agent.needFind = false;

                    polys.Dispose();
                    paths.Dispose();
                    verts.Dispose();
                    flags.Dispose();

                    return;

                }

            }

            agent.needFind = false;

        }

    }

    [BurstCompile]
    public partial struct MoveAgentJob : IJobEntity
    {

        [ReadOnly] public float dt;

        public void Execute(ref LocalTransform trm,
            in NavAgent agent,
            in DynamicBuffer<NavAgentBuffer> buffer)
        {

            if (!agent.havePath || agent.needFind) return;
            if (buffer.Length <= agent.currentBufferIdx) return;

            var point = buffer[agent.currentBufferIdx].wayPoint;
            var dir = math.normalize(point - trm.Position);
            dir.y = 0;

            trm.Rotation = Quaternion.LookRotation(dir);
            trm.Position += dir * agent.speed * dt;

        }

    }

    [BurstCompile]
    public partial struct CheckWayPointJob : IJobEntity
    {

        public void Execute(
            ref LocalTransform trm,
            ref NavAgent agent,
            in DynamicBuffer<NavAgentBuffer> buffer)
        {

            if (agent.inJob || !agent.havePath || agent.needFind) return;

            if(buffer.Length <= agent.currentBufferIdx) return;

            var way = buffer[agent.currentBufferIdx].wayPoint;
            var current = trm.Position;
            way.y = 0;
            current.y = 0;

            var dist = math.distance(way, current);

            if (dist <= 0.35f)
            {

                if (agent.currentBufferIdx + 1 != buffer.Length)
                {

                    agent.currentBufferIdx++;

                }

            }

        }

    }

    [BurstCompile]
    public partial struct CheckOutBounceJob : IJobEntity
    {

        public void Execute(ref LocalTransform trm,
             ref NavAgent agent,
             in DynamicBuffer<NavAgentBuffer> buffer)
        {


            if (agent.inJob || !agent.havePath || agent.needFind) return;

            if (buffer.Length <= agent.currentBufferIdx) return;


            if (agent.currentBufferIdx == 0)
            {

                var st = math.distance(trm.Position, buffer[agent.currentBufferIdx].wayPoint);
                if (st > 3)
                {

                    agent.currentBufferIdx = 0;
                    agent.needFind = true;
                    agent.havePath = false;

                }

                return;

            }


            var pt = buffer[agent.currentBufferIdx].wayPoint;
            var opt = buffer[agent.currentBufferIdx - 1].wayPoint;
            var dir = math.normalize(pt - opt);
            var myDir = math.normalize(pt - trm.Position);

            float dist = math.dot(dir, myDir);
            float angle = math.acos(dist);

            if (angle > 4)
            {

                agent.currentBufferIdx = 0;
                agent.needFind = true;
                agent.havePath = false;

            }

        }

    }

}
