Shader "Unlit/DiffuseShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MainColor("Main Color",color) = (1,1,1,1)
            
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
                // make fog work
              //  #pragma multi_compile_fog

                #include "UnityCG.cginc"
                #include "UnityStandardBRDF.cginc"
                //#include "Lighting.cginc"

                float4 _MainColor;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                struct VertexData {
                    float4 position:POSITION;//语义为POSITION
                    float3 normal:NORMAL;
                    float2 uv:TEXCOORD0;
                };
               struct FragmentData {
                 float4 position:SV_POSITION;
                 float2 uv:TEXCOORD0;
                 float3 normal:NORMAL;
               };
               FragmentData MyVertexProgram(VertexData v){
                    FragmentData i;
                    i.position = UnityObjectToClipPos(v.position);
                    i.normal = UnityObjectToWorldNormal(v.normal);
                    i.uv = TRANSFORM_TEX(v.uv, _MainTex);//v.uv*_MainTex_ST.xy+_MainTex_ST.zw;
                    return i;
               }
               float4 MyFragmentProgram(FragmentData i) :SV_TARGET{
                    float3 lightDir = _WorldSpaceLightPos0.xyz;
                    float3 lightColor = _LightColor0.rgb;
                    float3 diffuse =tex2D(_MainTex,i.uv).rgb *  lightColor * DotClamped(lightDir, i.normal);//
                    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * tex2D(_MainTex, i.uv).rgb;
                    return float4(ambient+diffuse,1);
                   // return tex2D(_MainTex,i.uv);
               }

            
            ENDCG
        }
    }
}
