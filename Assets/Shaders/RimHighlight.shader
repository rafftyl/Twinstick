// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//A shader used to highlight characters
Shader "Custom/RimHighlightShader"
{
	Properties
	{
		_Color  ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_HighlightColor("HighlightColor", Color) = (1,0,0,1)
		_HighlightStrength("HighlightStrength",  Range(0,1)) = 1
		[Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.1

	}
	SubShader
	{
		
		Pass
		{
			Tags {
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
				
			#include "UnityPBSLighting.cginc"

			struct appdata
			{
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators
			{			
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;		
			float4 _HighlightColor;
			float _HighlightStrength;
			float _Metallic;
			float _Smoothness;
			
			Interpolators vert (appdata v)
			{
				Interpolators i;
				i.position = UnityObjectToClipPos(v.position);
				i.worldPos = mul(unity_ObjectToWorld, v.position);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);			
				return i;
			}
			
			float4 frag (Interpolators i) : SV_Target
			{
				i.normal = normalize(i.normal);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

				float3 lightColor = _LightColor0.rgb;
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;

				float3 specularTint;
				float oneMinusReflectivity;

				//standard unity function for albedo
				albedo = DiffuseAndSpecularFromMetallic(
					albedo, _Metallic, specularTint, oneMinusReflectivity
				);

				UnityLight light;
				light.color = _LightColor0.rgb;
				light.dir = _WorldSpaceLightPos0.xyz;
				light.ndotl = DotClamped(i.normal, light.dir);
				UnityIndirect indirectLight;
				indirectLight.diffuse = max(0,ShadeSH9(float4(i.normal,1)));
				indirectLight.specular = 0;

				//highlight is stronger on rims
				float highlightWeight = _HighlightStrength *(1 - DotClamped(i.normal, viewDir));

				//standard unity shading
				fixed4 pbsColor = UNITY_BRDF_PBS(
					albedo, specularTint,
					oneMinusReflectivity, _Smoothness,
					i.normal, viewDir,
					light, indirectLight
				);

				return highlightWeight * _HighlightColor + (1 - highlightWeight) * pbsColor; //linear blend
			}
			ENDCG
		}

		//shadow  pass
		Pass
		{
			Tags {
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM
			#pragma target 3.0

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 position : POSITION;
				float3 normal : NORMAL;
			};

			float4 vert (appdata v) : SV_POSITION
			{
				float4 pos = UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
				return UnityApplyLinearShadowBias(pos);
			}	

			half4 frag() : SV_TARGET {
				return 0;
			}
			ENDCG


		}
	}
}
