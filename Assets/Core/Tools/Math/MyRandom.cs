using Unity.Mathematics;
using System;

public static class MyRandom
{
    public static Unity.Mathematics.Random Random = new Unity.Mathematics.Random((uint)DateTime.UtcNow.Ticks);
}