Shader "AVR/GradientShader"
{
    Properties
    {
        _Color ("Color", Color) = (1.0, 1.0, 1.0)
        _Falloff_Exp ("Falloff Exponent", Float) = 2.0
        _Glow ("Glow", Range(0.0, 1.0)) = 0.0
        // NOTE: Works along UV y-Axis. Further flexibility will require modiying this shader
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID	// Support for single-pass instanced mode
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO // Support for single-pass instanced mode
            };

            float4 _Color;
            float _Falloff_Exp;
            float _Glow;

            v2f vert (appdata v)
            {
                v2f o;

                // Support for single-pass instanced mode
				UNITY_SETUP_INSTANCE_ID(v);
    			UNITY_INITIALIZE_OUTPUT(v2f, o);
    			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); // Support for single-pass instanced mode

                float d = pow((1.0-i.uv.y)+_Glow, _Falloff_Exp);

                fixed4 col = fixed4(_Color.rgb, _Color.a*d);
                return col;
            }
            ENDCG
        }
    }
}
