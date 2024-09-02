using Unity.Entities;

public enum FeedbackType
{

    Blink,

}

public struct FeedbackBuffer : IBufferElementData
{

    public FeedbackType type;

}
