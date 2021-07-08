// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/UI/Particle_Additive_Clip" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	[HideInInspector] _ID ("_ID",int) = 1	
    [HideInInspector] _StencilComp ("_StencilComp",Float) = 8
}
 
Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off
	
	SubShader {
		Pass {
		
			Stencil 
            {  
                Ref [_ID]              
                Comp [_StencilComp]    
                Pass keep            
            }  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			#pragma multi_compile_fog
 
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
 
			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
 
			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 worldPosition : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				
				#endif
			};
			
			float4 _MainTex_ST;
 
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				//world space中的xy坐标保存到o.worldPosition.xy中
				//unity_ObjectToWorld 等同于_Object2World 5.3.8中使用：_Object2World
				o.worldPosition.xy = mul(unity_ObjectToWorld, v.vertex).xy;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
 
			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			// C#代码需要传入的裁剪区域变量, 这里我们增加一个变量（_UseClipRect）用来标记是否需要裁剪
			float4 _ClipRect;
			float _UseClipRect;
			
			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				
				float c = UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
				col.a = lerp(col.a, c * col.a, _UseClipRect);
				return col;
			}
			ENDCG 
		}
	}	
}
}