using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BlinkFeedback : IComponentData
{

    public float4 blinkColor;
    public float4 originColor;
    public float emission;
    public float currentTime;
    public float blinkTime;
    public bool isPlaying;

    public BlinkFeedback(Color blink, Color origin, float blinkTime, float emission)
    {

        blinkColor.x = blink.r;
        blinkColor.y = blink.g;
        blinkColor.z = blink.b;
        blinkColor.w = blink.a;

        originColor.x = origin.r;
        originColor.y = origin.g;
        originColor.z = origin.b;
        originColor.w = origin.a;

        this.blinkTime = blinkTime;
        this.emission = emission;

        isPlaying = false;
        currentTime = 0;

    }

}
