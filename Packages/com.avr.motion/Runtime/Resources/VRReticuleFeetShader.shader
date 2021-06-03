Shader "AVR/VRReticuleFeetShader"
{
    Properties
    {
        _Color ("Base Color", Color) = (1.0, 1.0, 1.0)
        _Amp ("Amplitude", Float) = 0.81
        _Freq ("Frequency", Float) = 22.0
        _Amount ("Amount", Float) = 80.0
        _Thickness ("Thickness", Range(0.0, 1.0)) = 0.4
        _Thickness_Gradient ("Thickness Gradient", Float) = 3.16
        _Gradient ("Gradient", Range(0.0, 2.0)) = 2.0
        _xSpeed ("xSpeed", Float) = 0.1
        _ySpeed ("ySpeed", Float) = -0.1
        _Backface_multiplier ("Backface Multiplier", Range(-1.0, 2.0)) = 0.8
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            // Draw front faces only
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _Amp;
            float _Freq;
            float _Amount;
            float _Thickness;
            float _Gradient;
            float _Thickness_Gradient;
            float _xSpeed;
            float _ySpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float alpha = 0.0;

                // bottom-to-top gradient
                float g = (1.0 - i.uv.y);

                // Scale thickness and gradient by height
                _Gradient *= g*g;
                _Thickness *= _Thickness_Gradient * g;

                // Move UVs over time
                i.uv = float2(
                    fmod(i.uv.x + (_Time.y)*_xSpeed, 1.0),
                    fmod(i.uv.y + _Time.y*_ySpeed, 1.0)
                );

                // distance between y of our fragment and sin(uv.x)
                float d = abs(
                    (_Amp * sin(i.uv.x*_Freq)) - 
                    fmod(i.uv.y*_Amount, 2.0) - 1.0
                );

                // If distance is smaller than thickness -> 1.0
                float r = _Thickness-d < 0 ? 0.0 : 1.0;

                alpha = _Gradient*_Color.a + g*_Color.a*r;
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }

        Pass
        {
            //Draw Back faces with _Color *= Backface_multiplier
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _Amp;
            float _Freq;
            float _Amount;
            float _Thickness;
            float _Gradient;
            float _Thickness_Gradient;
            float _xSpeed;
            float _ySpeed;
            float _Backface_multiplier;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                _Color *= _Backface_multiplier; //Fragment shader is identical to the one above, except for this line
                float alpha = 0.0;

                float g = (1.0 - i.uv.y);

                _Gradient *= g*g;
                _Thickness *= _Thickness_Gradient * g;

                i.uv = float2(
                    fmod(i.uv.x + (_Time.y)*_xSpeed, 1.0),
                    fmod(i.uv.y + _Time.y*_ySpeed, 1.0)
                );

                float d = abs(
                    (_Amp * sin(i.uv.x*_Freq)) - 
                    fmod(i.uv.y*_Amount, 2.0) - 1.0
                );

                float r = _Thickness-d < 0 ? 0.0 : 1.0;

                alpha = _Gradient*_Color.a + g*_Color.a*r;
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
}
