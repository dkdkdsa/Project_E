using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[RequireComponent(typeof(URPMaterialPropertyEmissionColorAuthoring))]
[RequireComponent(typeof(URPMaterialPropertyBaseColorAuthoring))]
public class FeedbackAuthoring : MonoBehaviour
{

    [Header("Blink")]
    [SerializeField] private Color _originColor;
    [SerializeField] private Color _blinkColor;
    [SerializeField] private float _blinkTime;
    [SerializeField] private float _emission;

    private class FeedbackBaker : Baker<FeedbackAuthoring>
    {
        public override void Bake(FeedbackAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BlinkFeedback(
                authoring._blinkColor, 
                authoring._originColor, 
                authoring._blinkTime,
                authoring._emission));

            AddBuffer<FeedbackBuffer>(entity);

        }

    }

}
