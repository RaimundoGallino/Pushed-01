Shader "Custom/Shader2"
{
    Properties { // input data

        _ColorA ("Color A", Color) = (1, 1, 1, 1)
        _ColorB ("Color B", Color) = (1, 1, 1, 1)
        _ColorStart ("Color Start", Range(0, 1)) = 1
        _ColorEnd ("Color End", Range (0, 1)) = 0
        _BrightnessValue ("Brightness", Range (0, 5)) = 2
    }
    SubShader {
        Tags { "RenderType"="Transparent" // tag to inform the render pipeline of what type this is

               "Queue"="Transparent" // changes the render order
        }

        Pass {
            Cull Off
            ZWrite Off
            Blend One One // additive
            // Blend DstColor Zero // multiply 
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            float4 _ColorB;
            float4 _ColorA;
            float _ColorStart;
            float _ColorEnd;
            float _BrightnessValue;



            struct MeshData { // per-vertex mesh data

                float4 vertex : POSITION; // local space vertex position
                float3 normals : NORMAL;  // local space normal direction

                //float4 tangent : TANGENT; // tangent direction (xyz) tangent sign (w)
                //float4 color : COLOR; // vertex colors

                float2 uv0 : TEXCOORD0; // uv0 diffuse/normal map textures
                //float2 uv1 : TEXCOORD0; // uv1 coordinates lightmap coordinates

            };

            // data passed to the vertex shader to the fragment shader
            // this will inerpolate/blend across the triangle

            struct Interpolators {
                float4 vertex : SV_POSITION; // clip space position for each vertex

                float3 normal : TEXCOORD0;
                float2 uv  : TEXCOORD1;
            };



            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normals); // just pass through

                o.uv =  v.uv0; //(v.uv0 + _Offset) * _Scale; // passthrougth

                return o;
            }

            float InverseLerp(float a, float b, float v) {
                return (v - a) / (b - a);
            }

            float4 frag (Interpolators i) : SV_Target {

                // Blend between two colors based on the lerp function
                // saturate clamps the values from the 2 inputs

                // float t = saturate(InverseLerp( _ColorStart, _ColorEnd, i.uv.y));


                float xOffset = cos(i.uv.x * TAU * 8) * 0.015;

                float t = cos( (i.uv.y + xOffset - _Time.y * 0.3) * TAU * 3) * 0.5 + 0.5;
                // adds the inverted uv coordinate
                t *= 1-i.uv.y;

                float topBottomRemover = (abs(i.normal.y) < 0.999);
                float brightness = (i.normal.y < 0.999) * _BrightnessValue;
                float waves = (t  * topBottomRemover) * brightness;

                float4 gradient = lerp(_ColorA, _ColorB, i.uv.y );
                return waves * gradient;

                //return outColor;
            }
            ENDCG
        }
    }
}
