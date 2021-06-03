Shader "AVR/CameraFade"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Always
        Cull Off
        ZWrite Off
        Fog { Mode Off }

        Pass
        {

        }
    }
}
