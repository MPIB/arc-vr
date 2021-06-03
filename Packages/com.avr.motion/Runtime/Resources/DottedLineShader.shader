Shader "AVR/DottedLineShader"
{
    Properties
    {
	    _DotLen ("Dot Length", Float) = 0.2
        _SpaLen ("Space Length", Float) = 0.1
        _ScrollSpeed ("Scroll Speec", Float) = 0.5
        _DotColor ("Dot Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _SpaColor ("Space Color", Color) = (1.0, 1.0, 1.0, 0.0)
    }
    SubShader {

    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
	Cull Off
    Lighting Off
    ZWrite Off
    Fog { Mode Off }

		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = v.uv;
				return o;
			}

            float _DotLen;
            float _SpaLen;
            float _ScrollSpeed;
            float4 _DotColor;
            float4 _SpaColor;
			
			fixed4 frag (v2f i) : SV_Target
			{
                float d = i.uv.x;
                d += _Time.y * _ScrollSpeed;
                d = abs(fmod(d, _DotLen+_SpaLen));

                if(d>_DotLen) {
                    i.color *= _SpaColor;
                }
                else {
                    i.color *= _DotColor;
                }

				return i.color;// * i.color.a;
			}
			ENDCG 
		}
	}
}
