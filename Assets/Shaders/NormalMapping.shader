Shader "Unlit/NormalMapping"
{
    Properties
    {
        _Color("Color Tint",Color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap("Normal Map",2D)="bump"{}//法线纹理
        _BumpScale("Bump Scale",Float) = 1.0//凹凸度
       
        _DiffuseColor("Diffuse Color",color) = (1,1,1,1)
        _SpecularColor("Specular Color",color) = (1,1,1,1)
        _Shininess("Shininess",float) = 10
        _Gloss("Gloss",Range(8.0,256)) = 20
    }
        SubShader
        {
            Tags { "LightMode" = "ForwardBase" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog

                #include "UnityCG.cginc"
                #include "Lighting.cginc"

                fixed4 _Color;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                sampler2D _BumpMap;
                float4 _BumpMap_ST;
                float _BumpScale;
                float4 _DiffuseColor;
                float4 _SpecularColor;
                float _Shininess;
                float _Gloss;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float3 normal:NORMAL;
                float4 tangent:TANGENT;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float3 lightDir:TEXCOORD1;
                float3 viewDir:TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f i;
                i.vertex = UnityObjectToClipPos(v.vertex);
                i.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
                i.uv.zw = TRANSFORM_TEX(v.uv.xy, _BumpMap);
                TANGENT_SPACE_ROTATION;            
                i.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
                i.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;
                               
                return i;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 tangentLightDir = normalize(i.lightDir);
                float3 tangentViewDir = normalize(i.viewDir);

                float4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                float3 tangentNormal;
                tangentNormal = UnpackNormal(packedNormal);
                tangentNormal.xy *= _BumpScale;
                tangentNormal.z = sqrt(1.0 - dot(tangentNormal.xy, tangentNormal.xy));

                float3 lightColor = _LightColor0.rgb;
                float3 diffuse = tex2D(_MainTex,i.uv).rgb * lightColor * DotClamped(tangentLightDir, tangentNormal) * _DiffuseColor;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * tex2D(_MainTex, i.uv).rgb;
               
                float3 halfVector = normalize(tangentLightDir + tangentViewDir);
                float3 specular = lightColor * pow(DotClamped(tangentNormal, halfVector),_Shininess) * _SpecularColor;
             
                return float4(specular + ambient + diffuse,1);
            }
            ENDCG
        }
    }
}
