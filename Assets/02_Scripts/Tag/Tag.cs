using System;

[Flags]
public enum Tag
{

    Null = 0,
    None = 1 << 0,
    Player = 1 << 1,
    Enemy = 1 << 2,
    Bullet = 1 << 3,

}
