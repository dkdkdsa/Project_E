using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(FeedbackAuthoring))]
public class HitAuthoring : MonoBehaviour
{

    [SerializeField] private float _invincibilityTime;
    [SerializeField] private bool _isCollison;
    [SerializeField] private Tag _hitEventTag;

    private class HitBaker : Baker<HitAuthoring>
    {

        public override void Bake(HitAuthoring authoring)
        {

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, 
                new Hit(authoring._invincibilityTime, 
                authoring._isCollison, authoring._hitEventTag));

        }

    }

}