using System.Collections;
using System.Collections.Generic;
using NetStack.Quantization;
using UnityEngine;

public static class Constants
{
    public static readonly ushort GAME_PORT = 9001;
    public static BoundedRange[] WORLD_BOUNDS = new BoundedRange[]
    {
        new BoundedRange(-156f, 116f, 0.05f),
        new BoundedRange(-109f, 4f, 0.05f),
        new BoundedRange(-119f, 160f, 0.05f),
    };
}
