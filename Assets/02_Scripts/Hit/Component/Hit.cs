using Unity.Entities;

public struct Hit : IComponentData
{

    public float invincibilityTime;
    public float currentTime;
    public bool isCollison;
    public bool isInvincibility;
    public Tag hitEventTag;

    public Hit(float invincibilityTime, bool isCollison, Tag hitEventTag)
    {

        this.invincibilityTime = invincibilityTime;
        this.isCollison = isCollison;
        this.hitEventTag = hitEventTag;
        isInvincibility = false;
        currentTime = 0;

    }

}
