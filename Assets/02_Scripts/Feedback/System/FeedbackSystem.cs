using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct FeedbackSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        PlayBlinkFeedback(ref state);
        ClearBuffer(ref state);

    }

    [BurstCompile]
    private void PlayBlinkFeedback(ref SystemState state)
    {

        var job = new BlinkFeedbackJob
        {

            dt = SystemAPI.Time.DeltaTime,

        };

        state.Dependency = job.Schedule(state.Dependency);

    }

    [BurstCompile]
    private void ClearBuffer(ref SystemState state)
    {

        var job = new BufferClearJob();

        state.Dependency = job.Schedule(state.Dependency);

    }


    [BurstCompile]
    public partial struct BufferClearJob : IJobEntity
    {

        public void Execute(ref DynamicBuffer<FeedbackBuffer> buffer)
        {

            buffer.Clear();

        }

    }

    [BurstCompile]
    public partial struct BlinkFeedbackJob : IJobEntity
    {

        public float dt;

        public void Execute(
            ref BlinkFeedback feedback,
            ref URPMaterialPropertyBaseColor color,
            ref URPMaterialPropertyEmissionColor emission,
            in DynamicBuffer<FeedbackBuffer> buffer)
        {

            if (feedback.isPlaying)
            {

                feedback.currentTime -= dt;

                if (feedback.currentTime <= 0)
                {

                    feedback.isPlaying = false;
                    color.Value = feedback.originColor;
                    emission.Value.x = 0;

                }

                return;

            }

            if (!CheckFeedbackInBuffer(in buffer))
                return;

            color.Value = feedback.blinkColor;
            feedback.currentTime = feedback.blinkTime;
            emission.Value.x = feedback.emission;
            feedback.isPlaying = true;

        }

        public readonly bool CheckFeedbackInBuffer(in DynamicBuffer<FeedbackBuffer> buffer)
        {

            foreach (var item in buffer)
            {

                if (item.type == FeedbackType.Blink)
                    return true;

            }

            return false;

        }

    }

}