using Unity.Entities;

public struct Attack : IComponentData
{

    public float attackCooldown;
    public float currentTime;
    public bool isCooldown;
}
