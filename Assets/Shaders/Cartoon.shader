Shader "Unlit/Cartoon"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _NormalTex("Normal Texture", 2D) = "white" {}
        _RampTex("Ramp Texture", 2D) = "white" {}

        _Warp("Warp factor", Range(0,4.0)) = 1.5

        _SpecularMask("Specular Mask", 2D) = "white" {}
        _Specular("Speculr Exponent", Range(0.1, 128)) = 128
        _SpecularColor("Specular Color", Color) = (1,1,1,1)
        _SpecularPower("Specular Power", Range(0, 30)) = 1
        _SpecularFresnel("Specular Fresnel Value", Range(0, 1)) = 0.28

        _RimMask("Rim Mask", 2D) = "white" {}
        _Rim("Rim Exponent", Range(0.1, 8)) = 1
        _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower("Rim Power", Range(0.5,8.0)) = 3.0

        _DiffuseCubeMap("Diffuse Convolution Cubemap", Cube) = ""{}
    }
    CGINCLUDE
    #include "UnityCG.cginc"
    #include "Lighting.cginc"
    #include "AutoLight.cginc"

    fixed4 _Color;
    sampler2D _MainTex;

    sampler2D _NormalTex;
    sampler2D _RampTex;
    sampler2D _SpecularMask;
    float _Specular;
    sampler2D _RimMask;
    float _Rim;
    float _Warp;

    samplerCUBE _DiffuseCubeMap;
    float _Amount;

    float4 _MainTex_ST;
    float4 _NormalTex_ST;
    float4 _SpecularMask_ST;
    float4 _RimMask_ST;
    float4 _RampTex_ST;

    fixed4 _SpecularColor;
    float _SpecularPower;
    float _SpecularFresnel;
    fixed4 _RimColor;
    float _RimPower;

    struct a2v
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float2 texcoord : TEXCOORD0;
        float4 tangent : TANGENT;
    };

    struct v2f
    {
        float4 pos : SV_POSITION;
        float2 mainTex_uv : TEXCOORD0;
        float3 worldNormal : TEXCOORD1;
        float2 specularMask_uv : TEXCOORD2;
        float2 rimMask_uv : TEXCOORD3;
        //float3 viewDir : TEXCOORD4;
        //float3 lightDir : TEXCOORD5;
        float3 worldPos : TEXCOORD4;
        float2 rampTex_uv : TEXCOORD5;
        float3 up : TEXCOORD6;
        float2 normal_uv : TEXCOORD7;
    };
    ENDCG

    SubShader
    {
        Pass
        {
            Name "FORWARD"
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"


            v2f vert(appdata_full v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.mainTex_uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.rampTex_uv = TRANSFORM_TEX(v.texcoord, _RampTex);
                o.specularMask_uv = TRANSFORM_TEX(v.texcoord, _SpecularMask);
                o.rimMask_uv = TRANSFORM_TEX(v.texcoord, _RimMask);


                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                o.up = mul(unity_ObjectToWorld, half4(0, -1, 0, 0)).xyz;
                //debug
                //o.up.x = dot(v.normal, half3(0, 1, 0));
                // pass lighting information to pixel shader
                o.normal_uv = TRANSFORM_TEX(v.texcoord, _NormalTex);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                worldNormal = UnpackNormal(tex2D(_NormalTex, i.normal_uv));
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 worldUp = normalize(i.up);

                // Compute View Independent Lighting Terms
                fixed4 c = tex2D(_MainTex, i.mainTex_uv) * _Color;
                fixed3 k = tex2D(_MainTex, i.mainTex_uv).rgb;

                half difLight = dot(worldNormal, worldLightDir);
                half halfLambert = pow(0.5 * difLight + 0.5, 1); //hardcoding alpha,beta,r=0.5,0.5,1

                half3 ramp = tex2D(_RampTex, halfLambert.xx).rgb; //warp()
                half3 difWarping = ramp * _Warp; // Or difWarping = ramp * 2;
                //difWarping = half3(1, 1, 1);
                half3 difLightTerm = _LightColor0.rgb * difWarping;

                half3 dirLightTerm = UNITY_LIGHTMODEL_AMBIENT.xyz;
                float3 ambientTerm = texCUBE(_DiffuseCubeMap, worldNormal).rgb * 1; //hardcoding a=1
                //fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

                half3 viewIndependentLightTerms = k * (difLightTerm + ambientTerm);

                // Compute View Dependent Lighting Terms
                /*
                half3 r = reflect(i.lightDir, worldNormal);
                half3 refl = dot(i.viewDir, r);
                half fresnelForSpecular = 1; // Just for example
                half fresnelForRim = pow(1 - dot(worldNormal, worldViewDir）, 4);  //f_r
                half3 kS = tex2D(_SpecularMask, i.uv2).rgb;	//k_s
                half3 multiplePhongTerms = _LightColor0.rgb * kS * max(fresnelForSpecular * pow(refl, _Specular), fresnelForRim * pow(refl, _Rim));
                */
                float3 halfVector = normalize(worldLightDir + worldViewDir);
                float3 refl = dot(worldNormal, halfVector);
                float3 specBase = pow(saturate(refl), _Specular);
                float3 fresnelForSpecular = 1;

                half fresnelForRim = pow(1 - dot(worldNormal, worldViewDir), 4);
                //fresnel += _SpecularFresnel * (1.0 - fresnel);
                half3 multiplePhongTerms = _LightColor0.rgb * max(specBase * fresnelForSpecular,
                                                                  fresnelForRim * pow(refl, _Rim));

                half3 kR = tex2D(_RimMask, i.rimMask_uv).rgb;
                //kR = half3(1, 1, 1);
                half3 aV = float(1);
                half3 dedicatedRimLighting = dot(worldNormal, i.up) * fresnelForRim * kR * aV;
                half rim = saturate(dot(i.up, worldNormal));
                
                //debug
                //rim = i.up.x;
                //half rim = 1.0 - saturate(dot(worldViewDir, worldNormal));
                //dedicatedRimLighting = _RimColor.rgb * pow(rim, _RimPower);
                half3 viewDependentLightTerms = multiplePhongTerms + dedicatedRimLighting;

                // Compute the final color
                float4 col;
                col.rgb = viewIndependentLightTerms + viewDependentLightTerms;
                col.a = 1.0;

                //col = float4(k * (difLightTerm), 1); //debug for figure 6b
                //col = float4(viewIndependentLightTerms, 1);	//debug for figure 6d
                //col = float4(multiplePhongTerms, 1);	//debug for figure 6f
                //col = float4(dedicatedRimLighting, 1);	//debug for figure 6f
                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "FORWARD_ADD"
            Tags
            {
                "LightMode" = "ForwardAdd"
            }


            Blend One One

            CGPROGRAM
            #pragma multi_compile_fwdadd

            #pragma vertex vert
            #pragma fragment frag


            v2f vert(appdata_full v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.mainTex_uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.rampTex_uv = TRANSFORM_TEX(v.texcoord, _RampTex);
                o.specularMask_uv = TRANSFORM_TEX(v.texcoord, _SpecularMask);
                o.rimMask_uv = TRANSFORM_TEX(v.texcoord, _RimMask);

                o.up = mul(unity_ObjectToWorld, half4(0, 1, 0, 0)).xyz;

                // pass lighting information to pixel shader
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //half3 normal = UnpackNormal(tex2D(_NormalTex, i.uv1));
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 worldUp = normalize(i.up);

                // Compute View Independent Lighting Terms
                fixed4 c = tex2D(_MainTex, i.mainTex_uv) * _Color;
                fixed3 k = tex2D(_MainTex, i.mainTex_uv).rgb;

                //half difLight = dot(normal, i.lightDir);
                half difLight = dot(worldNormal, worldLightDir);
                half halfLambert = pow(0.5 * difLight + 0.5, 1); //hardcoding alpha,beta,r=0.5,0.5,1

                //half3 ramp = tex2D(_RampTex, float2(halfLambert)).rgb;  //w()
                half3 ramp = tex2D(_RampTex, halfLambert.xx).rgb;
                half3 difWarping = ramp * 2; // Or difWarping = ramp * 2;
                //difWarping = half3(1, 1, 1);
                half3 difLightTerm = _LightColor0.rgb * difWarping;
                half3 viewIndependentLightTerms = k * (difLightTerm);


                float3 halfVector = normalize(worldLightDir + worldViewDir);
                float3 specBase = pow(saturate(dot(halfVector, worldNormal)), _SpecularPower);
                float fresnel = pow(1.0 - dot(worldViewDir, halfVector), 4);
                //fresnel += _SpecularFresnel * (1.0 - fresnel);
                half3 multiplePhongTerms = _LightColor0.rgb * specBase * fresnel;


                half3 viewDependentLightTerms = multiplePhongTerms;

                // Compute the final color
                float4 col;
                col.rgb = viewIndependentLightTerms + viewDependentLightTerms;
                col.a = 1.0;

                return col;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}