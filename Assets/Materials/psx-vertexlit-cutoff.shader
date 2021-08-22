 
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
 
// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color
 
Shader "psx/PSX_CUTOFF_V1" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 100
 
    Lighting On
 
    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog
 
            #include "UnityCG.cginc"
 
            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
				half3 normal : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                half4 color : COLOR0;
            };
 
            struct v2f {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                half4 color : COLOR0;
                half4 colorFog : COLOR1;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
				half3 normal : TEXCOORD1;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

           v2f vert(appdata_full v)
					{
						v2f o;

						//Vertex snapping
						float4 snapToPixel = UnityObjectToClipPos(v.vertex);
						float4 vertex = snapToPixel;
						vertex.xyz = snapToPixel.xyz / snapToPixel.w;
						vertex.x = floor(160 * vertex.x) / 160;
						vertex.y = floor(120 * vertex.y) / 120;
						vertex.xyz *= snapToPixel.w;
						o.pos = vertex;

						//Vertex lighting 
					//	o.color =  float4(ShadeVertexLights(v.vertex, v.normal), 1.0);
						o.color = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);
						o.color *= v.color;

						float distance = length(mul(UNITY_MATRIX_MV,v.vertex));

						//Affine Texture Mapping
						float4 affinePos = vertex; //vertex;				
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						o.texcoord *= distance + (vertex.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;
						o.normal = distance + (vertex.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;

						//Fog
						float4 fogColor = unity_FogColor;

						float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
						o.normal.g = fogDensity;
						o.normal.b = 1;

						o.colorFog = fogColor;
						o.colorFog.a = clamp(fogDensity,0,1);
                        
                        UNITY_TRANSFER_FOG(o,o.pos);

						//Cut out polygons
						if (distance > unity_FogStart.z + unity_FogColor.a * 255)
						{
							o.pos.w = 0;
						}

						return o;
					}
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord / i.normal.r)*i.color;
                clip(col.a - _Cutoff);
                half4 color = col*(i.colorFog.a);
				color.rgb += i.colorFog.rgb*(1 - i.colorFog.a);
                UNITY_APPLY_FOG(i.fogCoord, color);
                return col;
            }
        ENDCG
    }
}
 
}