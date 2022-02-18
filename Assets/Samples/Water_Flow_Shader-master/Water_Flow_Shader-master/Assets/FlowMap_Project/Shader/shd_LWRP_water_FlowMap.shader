
//Created and Edited - Emilio Villar 03162020

Shader "FX/LWRP/Water Flow Map" {
    Properties {
        _WaterNormal("Water Normals", 2D) = "bump" {}
        _WaterTex ("Water Texture", 2D) = "white" {}
        _colorWater ("Water Main Color", color) = (1,1,1,1)

        [Header(FLOWMAP)][Space(10)]
        _FlowMap ("Flow Map Texture", 2D) = "grey" {}
        _Speed ("Speed Flow", Range(-0.3,0.3)) = 0.2

        [Header(MATCAP)][Space(10)]
        _MatCap ("MatCap Texture", 2D) = "white" {}
        _RimInt ("MatCap Intensity", float) = 1.0
		_AlphaInt("Alpha intensity", Range(0,1)) = 1

    }
 
    SubShader {


        Tags { "RenderType" = "Transparent" "IgnoreProjector" = "True" "RenderPipeline" = "LightweightPipeline" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            Name "Unlit"
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.lightweight/Shaders/UnlitInput.hlsl"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
				half4 tangent : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half4  vertex : SV_POSITION;
                half2  uv : TEXCOORD0;
                half2  uv_matcap : TEXCOORD1;
                half3 normalDir : TEXCOORD2;
                half3	TtoV0 : TEXCOORD3;
				half3	TtoV1 : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
 
            TEXTURE2D(_WaterTex);
            TEXTURE2D(_FlowMap);
            TEXTURE2D(_MatCap);
            TEXTURE2D(_WaterNormal);
            float _Speed, _AlphaInt, _RimInt;
            half4 _WaterTex_ST,  _colorWater, _MainTex_ST, _MatCap_ST;
            SamplerState sampler_WaterTex, sampler_FlowMap, sampler_MatCap, sampler_WaterNormal;
 
            v2f vert(appdata v) {
                v2f o = (v2f)0;

                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_TRANSFER_INSTANCE_ID(i, o);
                o.uv = TRANSFORM_TEX(v.uv, _WaterTex);
                o.uv_matcap = TRANSFORM_TEX(v.uv, _MatCap);

                //MatCap Normals Section
                half3 worldNormal = mul(UNITY_MATRIX_MV, float4((v.normal.xyz),0));
				half4 worldTangent =  mul(UNITY_MATRIX_MV, float4((v.tangent.xyz),0));
				half3 worldBinormal = cross(worldNormal.xyz, worldTangent.xyz) * v.tangent.y;
				o.TtoV0 = half3(worldTangent.x, worldTangent.x, worldTangent.x);
				o.TtoV1 = half3(worldTangent.y, worldTangent.y, worldTangent.y);

                o.normalDir = mul(UNITY_MATRIX_MV, float4((v.normal.xyz),0));
                VertexPositionInputs vertexInput = GetVertexPositionInputs(float3(v.vertex.x, v.vertex.y, v.vertex.z));
                o.vertex = vertexInput.positionCS;
                return o;
            }
       
            half4 frag(v2f i) : COLOR {
                //Flow Map Texture Section
                half dif1 = frac(_Time.g * 0.015 + 0.5);
                half dif2 = frac(_Time.g * 0.015);
                half lerpVal = abs((0.5 - dif1)/0.5);
                half4 flowVal = (SAMPLE_TEXTURE2D(_FlowMap,sampler_FlowMap, i.uv) * 2 - 1) * _Speed;

                //Matcap + Flow Map 
				half2 vn;					
                half3 normals = UnpackNormal(SAMPLE_TEXTURE2D(_WaterNormal,sampler_WaterNormal, (i.uv + flowVal.xy)*dif1));
                half3 normals2 = UnpackNormal(SAMPLE_TEXTURE2D(_WaterNormal,sampler_WaterNormal, (i.uv + flowVal.xy)*dif2));
                half3 normalsOut = lerp(normals, normals2, lerpVal);
				vn.x = dot(i.TtoV0, normalsOut + flowVal.x);
				vn.y = dot(i.TtoV1, normalsOut + flowVal.y);
				half4 MatcapDir  = SAMPLE_TEXTURE2D(_MatCap, sampler_MatCap, half2((i.uv_matcap.y)+(vn.x*0.5 + 0.5),i.uv_matcap.x+(vn.x*0.5 + 0.5))+ flowVal.xy * 1.5);

                //Color Texture + Flow Map
                half4 col1 = SAMPLE_TEXTURE2D(_WaterTex, sampler_WaterTex, (i.uv + flowVal.xy )* dif1);
                half4 col2 = SAMPLE_TEXTURE2D(_WaterTex, sampler_WaterTex, (i.uv + flowVal.xy) * dif2);
                half4 c = lerp(col1, col2, lerpVal);

                //All together
                half3 RGB = (MatcapDir*_RimInt)*((c.rgb+(flowVal.rgb)));
                return half4(RGB.rgb*_colorWater.rgb, _AlphaInt);
            }
            ENDHLSL
        }
    }
}