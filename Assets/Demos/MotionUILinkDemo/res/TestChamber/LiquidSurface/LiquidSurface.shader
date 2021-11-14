Shader "Helliaca/LiquidSurface"
{
    //Shader adapted from: https://www.shadertoy.com/view/Ms2SD1
    Properties
    {
        [Header(Advanced Properties)]
        _HashSeed ("Hash Function Seed", Float) = 1.0
        _Seed ("Seed", Float) = 1.0
        _Frequency ("Frequency", Range (0.0,0.5)) = 0.16
        _Amplitude ("Amplitude", Range (-2.0, 2.0)) = 0.6
        _Choppiness ("Choppiness", Range (-0.75, 20.0)) = 4.0
        _Wave_iter ("Geometry Iterations", Range(0, 5)) = 3
        _Detail_iter ("Detail Iterations", Range(0, 10)) = 5

        _SkyColor ("SkyColor", Color) = (0.788, 0.871, 1.0, 1.0)

        _SeaAmbientColor ("Sea Ambient", Color) = (0.0, 0.09, 0.18, 1.0)
        _SeaDiffuseColor ("Sea Diffuse", Color) = (0.058, 0.065, 0.043, 1.0)
        _SeaDiffuseExponent ("Sea Diffuse Exponent", Float) = 30
        _SeaSpecularColor ("Sea Specular", Color) = (1.0, 1.0, 1.0, 1.0)
        _SeaSpecularExponent ("Sea Specular Exponent", Float) = 60
        _SeaSurfaceColor ("Sea Surface", Color) = (0.48, 0.54, 0.36, 1.0)
        _SeaSurfaceHeight ("Sea Surface Height", Float) = 0.6
        _SeaSurfaceDepthAttentuationFactor ("Sea Surface Attenuation Factor by Depth", Float) = 0.18
        _SeaSurfaceDistanceAttentuationFactor ("Sea Surface Attenuation Factor by Distance", Float) = 0.001

        [MaterialToggle] _Test("isToggle", Float) = 0 


    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define PI 3.1415927f
            #define f2_00 float2(0.0f, 0.0f)
            #define f2_10 float2(1.0f, 0.0f)
            #define f2_01 float2(0.0f, 1.0f)
            #define f2_11 float2(1.0f, 1.0f)

            uniform float _Seed;
            uniform float _HashSeed;
            uniform float _Frequency;
            uniform float _Amplitude;
            uniform float _Choppiness;
            uniform int _Wave_iter;
            uniform int _Detail_iter;

            uniform float4 _SkyColor;
            uniform float4 _SeaAmbientColor;
            uniform float4 _SeaDiffuseColor;
            uniform float _SeaDiffuseExponent;
            uniform float4 _SeaSpecularColor;
            uniform float _SeaSpecularExponent;
            uniform float4 _SeaSurfaceColor;
            uniform float _SeaSurfaceHeight;
            uniform float _SeaSurfaceDepthAttentuationFactor;
            uniform float _SeaSurfaceDistanceAttentuationFactor;

            uniform float _Test;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldSpacePos : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);
                return o;


            }

            // >>> Helper functions

            // Provides random hash based on floating point seed
            float hash(float f) {
                return frac(sin(f)*_HashSeed);
            }

            // Provides random hash based on 2d vector as seed
            float hash(float2 v) {
                // dot product with some random, constant vector
                return hash( dot(v, float2(_Seed, hash(_Seed))) );
            }

            // Provides noise texture with each integer having a value in [-1,1] and values inbetween being bi-linearly interpolated
            float int_noise(float2 v) {
                // Get integer position of v as well as fractional (v % 1)
                float2 p = floor(v);
                float2 fr = frac(v);

                // Smoothstep the fractional part to avoid jagged edges inbetween values
                fr = smoothstep(f2_00, f2_11, fr);
                // Get inverse vector, we need this in the next step
                float2 fm = f2_11 - fr;

                // Perform a linear square-interpolation between p, p+(1,0), p+(0,1) and p+(1,1). See: https://en.wikipedia.org/wiki/Bilinear_interpolation#Unit_square
                float k = hash(p) * fm.x*fm.y + 
                    hash(p+f2_10) * fr.x*fm.y + 
                    hash(p+f2_01) * fm.x*fr.y + 
                    hash(p+f2_11) * fr.x*fr.y;

                // Transform to values between -1 and 1
                return -1.0 + 2.0 * k;
            }

            // Provides a semi-random random texture with smooth waves. Example: https://i.imgur.com/VFP54aQ.png
            float wave(float2 uv, float choppy) {
                // Distort UV map with noise
                uv.x += int_noise(uv);
                uv.y += int_noise(uv);
                // Sine function in [0,1] with 2x the frequency
                // The lines below provide a smooth, regular grid of waves moving back-and-forth. The distortion step above provides the necessary randomness.
                float2 wv = 0.5 * ( sin(2.0f*uv) + 1.0f ); 
                return pow(1.0-pow(wv.x * wv.y,0.65),choppy);
            }

            // overlay of multiple maps or smth?
            float map2(float3 pos) {
                float freq = _Frequency;
                float amp = _Amplitude;
                float choppy = _Choppiness;
                float2 uv = pos.xz;

                float2x2 octave_m = float2x2(1.6,-1.2,1.2,1.6);

                float d, h = 0.0f;

                for(int i=0; i < _Wave_iter; i++) {
                    d = wave( _Frequency*(_Time[1]+uv), choppy);  // get base wave value
                    d += wave( _Frequency*(_Time[1]-uv), choppy); // For some additional randomness?
                    h += d*amp;                             // Add this value to the height based on amplitude

                    uv = mul(octave_m, uv);                 // Spiral the UV somewhere else for next sample
                    freq*= 1.9f;                           // Increase frequency of wave-map (higher detail)
                    amp *= 0.22f;                           // Decrease amplitude (add detail waves that are smaller)

                    choppy = lerp(choppy, 1.0f, 0.2f);      // Increase choppiness, as bigger waves are smoother and smaller ones are choppier
                }
                return pos.y - h;
            }
            float map2_detailed(float3 pos) {
                float freq = _Frequency;
                float amp = _Amplitude;
                float choppy = _Choppiness;
                float2 uv = pos.xz;

                float2x2 octave_m = float2x2(1.6,-1.2,1.2,1.6);

                float d, h = 0.0f;

                for(int i=0; i < _Detail_iter; i++) {
                    d = wave( freq*(_Time[1]+uv), choppy);
                    d += wave( freq*(_Time[1]-uv), choppy);
                    h += d*amp;

                    uv = mul(octave_m, uv);
                    freq *= 1.9f;
                    amp *= 0.22f;

                    choppy = lerp(choppy, 1.0f, 0.2f);
                }
                return pos.y - h;
            }
            float3 heightMapTracing(float3 ori, float3 dir) {
                float3 p;
                float tm = 0.0;
                float tx = 1000.0;    
                float hx = map2(ori + dir * tx);
                
                if(hx > 0.0) return tx; // We still haven't hit the ocean after 1k units, so return the max. (1k)
                
                float hm = map2(ori + dir * tm);    
                float tmid = 0.0;
                
                // Binary search with 8 iterations to the point where the view vector is just above the wave
                for(int i = 0; i < 8; i++) {
                    tmid = lerp(tm,tx, hm/(hm-hx));                   
                    p = ori + dir * tmid;                   
                    float hmid = map2(p);
                    if(hmid < 0.0) {
                        tx = tmid;
                        hx = hmid;
                    } else {
                        tm = tmid;
                        hm = hmid;
                    }
                }
                return p;
            }
            float3 getNormal(float3 p, float eps) {
                // Derive Surface normal from difference in height based on x+eps and z+eps
                float3 n;
                n.y = map2_detailed(p);    // Get height at current position
                n.x = map2_detailed(float3(p.x+eps,p.y,p.z)) - n.y;
                n.z = map2_detailed(float3(p.x,p.y,p.z+eps)) - n.y;
                n.y = eps;
                return normalize(n);
            }
            float fresnel (float3 normal, float3 viewDir) {
                return pow( clamp(1.0 - dot(normal, -viewDir), 0.0, 1.0), 3.0 ) * 0.5;
            }
            float3 phong(float3 ambient, float3 diffuse, float3 spec, float3 n, float3 l, float3 viewDir) {
                float diff_str = pow(dot(n,l), _SeaDiffuseExponent);
                float spec_str = pow(max(dot(l, reflect(viewDir, n)), 0.0), _SeaSpecularExponent);
                return ambient + diff_str * diffuse + spec_str * spec;
            }
            float3 getSeaColor(float3 p, float3 n, float3 l, float3 eye, float3 dist) {  
                // We perform regular phong shading
                float3 ph = phong(_SeaAmbientColor, _SeaDiffuseColor, _SeaSpecularColor, n, l, eye);
                // Apply a frensel effect with sky color
                float3 color = lerp(ph, _SkyColor, fresnel(n, eye));
                
                // Attenuate towards a differen color on the surface
                float atten = max(1.0 - dot(dist,dist) * _SeaSurfaceDistanceAttentuationFactor, 0.0);
                color += _SeaSurfaceColor * (p.y - _SeaSurfaceHeight) * _SeaSurfaceDepthAttentuationFactor * atten;
                
                return color;
            }
            // ================

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float3 origin = _WorldSpaceCameraPos;
                float3 dir = normalize(i.worldSpacePos.xyz - origin);

                // This line sets the height of the liquid surface to be equal to the plane we draw it on
                origin -= i.worldSpacePos.y;

                // Get position where view ray intersects ocean
                float3 p = heightMapTracing(origin, dir);

                // Distance to p
                float3 dist = p - origin;
                // Surface normal
                float3 n;
                n = getNormal(p, dot(dist,dist) * 0.0001f);

                // Light stuff
                float3 light = normalize(float3(0.0,1.0,0.8)); 
                
                // color
                float3 c = lerp(
                    _SkyColor,
                    getSeaColor(p,n,light,dir,dist),
    	            pow(smoothstep(0.0,-0.02,dir.y),0.2));
                
                
                fixed4 col = fixed4(c.r, c.g, c.b, 1.0f);

                return col;
            }
            ENDCG
        }
    }
}
