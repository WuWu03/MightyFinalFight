Shader "Custom/BeAttackFlashColor"
{
    Properties {
        _MainTex ("MainTex (RGB)", 2D) = "white" {}
    _FlashColor("FlashColor" , Color) = (1,1,1,1)
        _ColorRange("ColorRange" , Range(0.0, 0.7)) = 0
    }

        SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // 定义自己的光照模型  
#pragma surface surf CustomDiffuse  
        /* 
        参数：SurfaceOutput 经过表面计算后的输入；lightDir 光线方向；atten 光衰减系数 
        */  
        half4 LightingCustomDiffuse(SurfaceOutput s, half3 lightDir, half atten) {  
        half difLight = max(0, dot(s.Normal, lightDir));    //点积调整当前点的亮度  
        difLight = difLight * 0.5 + 0.5;    //低光下增亮效果，范围从 0-1 到 0.5-1  
        half4 col;  
        col.rgb = s.Albedo * _LightColor0.rgb * (difLight * atten * 1.5); //根据物体的颜色、光颜色、光亮度、衰减度计算当前点的颜色
        col.a = s.Alpha;  
        return col;  
    }  

    sampler2D _MainTex;
    half4 _FlashColor;
    float _ColorRange;

    struct Input {
        float2 uv_MainTex;
        float3 viewDir;
    };

    void surf (Input IN, inout SurfaceOutput o) 
    {
        half4 c = tex2D (_MainTex, IN.uv_MainTex);
        o.Albedo = c.rgb;
        o.Alpha = c.a;
        IN.viewDir = normalize(IN.viewDir);
        float NdotV = dot(o.Normal, IN.viewDir);
        if(NdotV < _ColorRange)
            o.Emission = _FlashColor.rgb * lerp(0, 1, (_ColorRange - NdotV)/(1 - _ColorRange));
    }

    ENDCG
    } 
        FallBack "Diffuse"
}