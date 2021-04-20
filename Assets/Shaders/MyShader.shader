Shader "Unlit/MyShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _MainColor("Main Color",color) = (1,1,1,1)
        _DiffuseColor("Diffuse Color",color) = (1,1,1,1)
        _SpecularColor("Specular Color",color) = (1,1,1,1)
        _Shininess("Shininess",float) = 10
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque"}
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex MyVertexProgram
                #pragma fragment MyFragmentProgram
                
                #pragma shader_feature USE_SPECULAR
                #pragma shader_feature NORMAL_ONLY   
                #pragma enable_d3d11_debug_symbols

                #include "UnityCG.cginc"
                #include "UnityStandardBRDF.cginc"


                float4 _MainColor;
                float4 _DiffuseColor;
                float4 _SpecularColor;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Shininess;

                struct VertexData {
                    float4 position:POSITION;//语义为POSITION
                    float3 normal:NORMAL;
                    float2 uv:TEXCOORD0;
                };
               struct FragmentData {
                 float4 position:SV_POSITION;
                 float2 uv:TEXCOORD0;
                 float3 normal:TEXCOORD1;
                 float3 worldPos:TEXCOORD2;
               };


               FragmentData MyVertexProgram(VertexData v) {
                    FragmentData i;
                    i.worldPos = mul(unity_ObjectToWorld, v.position);
                    i.position = UnityObjectToClipPos(v.position);
                    i.normal = UnityObjectToWorldNormal(v.normal);
                    i.uv = TRANSFORM_TEX(v.uv, _MainTex);//v.uv*_MainTex_ST.xy+_MainTex_ST.zw;
                    return i;
               }

               float3 specular = float3(0, 0, 0);
               float4 MyFragmentProgram(FragmentData i) :SV_TARGET{
                #if NORMAL_ONLY
                   return float4(i.normal,1);
                #endif
                    float3 lightDir = _WorldSpaceLightPos0.xyz;
                    float3 lightColor = _LightColor0.rgb;
                    float3 diffuse = tex2D(_MainTex,i.uv).rgb * lightColor * DotClamped(lightDir, i.normal) * _DiffuseColor;
                    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * tex2D(_MainTex, i.uv).rgb;
                #if USE_SPECULAR
                    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);//视线方向
                    float3 halfVector = normalize(lightDir + viewDir);
                    specular = lightColor * pow(DotClamped(i.normal, halfVector),_Shininess) * _SpecularColor;
                #endif
                    return float4(specular + ambient + diffuse,1);
                    // return tex2D(_MainTex,i.uv);
                }


             ENDCG
         }
        }
            CustomEditor "CustomShaderGUI"
}
