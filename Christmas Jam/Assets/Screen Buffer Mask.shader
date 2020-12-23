Shader "Screen Buffer Mask" {
    Properties {
        _Color ("Main Color, Alpha", Color) = (1,1,1,1)
    }

    Category {
        Tags {"RenderType" = "Opaque" "Queue"="Transparent"}

        ZWrite On   // for more on Z-buffer stuff and Offset, see http://docs.unity3d.com/Documentation/Components/SL-CullAndDepth.html
        ZTest GEqual   // GEqual = mask stuff in front of the mask geo
        Lighting Off
        Color [_Color]   // change alpha in material to tweak mask strength

        SubShader {
            Pass {
                Colormask A   // render only to alpha
                Offset -1, -1
                Cull Back
            }
        }
    } 
}
