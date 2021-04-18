Shader "Unlit/NormalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor("Main Color",Color)=(1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            // make fog work
           // #pragma multi_compile_fog

            #include "UnityCG.cginc"

           struct VertexData {
                    float4 position:POSITION;//语义为POSITION
                    float3 normal:NORMAL;              
                };
               struct FragmentData {
                 float4 position:SV_POSITION;
                 float3 normal:TEXCOORD0; //NORMAL;//         
               };

            sampler2D _MainTex;
            //float4 _MainTex_ST;
            float4 _MainColor;

               FragmentData MyVertexProgram(VertexData v)
            {
                   FragmentData i;
                   i.position = UnityObjectToClipPos(v.position);              
                   i.normal = UnityObjectToWorldNormal(v.normal);//v.normal
                   //i.normal = (1, 1, 1);
                return i;
            }

               float4 MyFragmentProgram(FragmentData i) : SV_Target
               {
                   return float4(i.normal,1);//_MainColor;//
            }
            ENDCG
        }
    }
}
