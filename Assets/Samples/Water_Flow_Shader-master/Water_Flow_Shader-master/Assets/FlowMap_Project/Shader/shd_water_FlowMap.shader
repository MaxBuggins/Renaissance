
//Created and Edited - Emilio Villar 03162020

Shader "FX/Water Flow Map" {
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


        Pass {
            Tags { "RenderType"="Transparent" "Queue"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
       
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
				fixed3 normal : NORMAL;
				fixed4 tangent : TANGENT;
            };

            struct v2f {
                fixed4  pos : SV_POSITION;
                fixed2  uv : TEXCOORD0;
                fixed2	uv_matcap : TEXCOORD1;
                half3	TtoV0 : TEXCOORD2;
				half3	TtoV1 : TEXCOORD3;
				half3	n : TEXCOORD4;
            };
 
            sampler2D  _WaterTex, _FlowMap, _MatCap, _WaterNormal;
            float _Speed,  _AlphaInt, _RimInt;
            fixed4 _WaterTex_ST,  _colorWater, _MainTex_ST, _MatCap_ST;
 
            v2f vert(appdata v) {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _WaterTex);
                o.uv_matcap = TRANSFORM_TEX(v.uv, _MatCap);

                //MatCap Normals Section
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent);
				fixed3 worldBinormal = cross(worldNormal.xyz, worldTangent.xyz) * v.tangent.y;
				//o.TtoV0 = fixed3(worldTangent.x, worldBinormal.x, worldNormal.x);
				//o.TtoV1 = fixed3(worldTangent.y, worldBinormal.y, worldNormal.y);
					
                    TANGENT_SPACE_ROTATION;
					o.n = mul(rotation, v.normal);
					o.TtoV0 = mul(rotation, UNITY_MATRIX_IT_MV[0].xyz);
					o.TtoV1 = mul(rotation, UNITY_MATRIX_IT_MV[1].xyz);

                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
       
            fixed4 frag(v2f i) : COLOR {
                //Flow Map Texture Section
                fixed dif1 = frac(_Time.g * 0.015 + 0.5);
                fixed dif2 = frac(_Time.g * 0.015);
                half lerpVal = abs((0.5 - dif1)/0.5);
                half4 flowVal = (tex2D(_FlowMap, i.uv) * 2 - 1) * _Speed;

                //Matcap + Flow Map 
				half2 vn;					
                half3 normals = UnpackNormal(tex2D(_WaterNormal, (i.uv + flowVal.xy)*dif1));
                half3 normals2 = UnpackNormal(tex2D(_WaterNormal, (i.uv + flowVal.xy)*dif2));
                half3 normalsOut = lerp(normals, normals2, lerpVal);
				vn.x = dot(i.TtoV0, normalsOut + flowVal.x);
				vn.y = dot(i.TtoV1, normalsOut + flowVal.y);
				fixed4 MatcapDir  = tex2D(_MatCap, fixed2((i.uv_matcap.y)+(vn.x*0.5 + 0.5),i.uv_matcap.x+(vn.x*0.5 + 0.5))+ flowVal.xy * 1.5);

                //Color Texture + Flow Map
                fixed4 col1 = tex2D(_WaterTex, (i.uv + flowVal.xy )* dif1);
                fixed4 col2 = tex2D(_WaterTex, (i.uv + flowVal.xy) * dif2);
                fixed4 c = lerp(col1, col2, lerpVal);

                //All together
                fixed3 RGB = (MatcapDir*_RimInt)*((c.rgb+(flowVal.rgb)));
                return fixed4(RGB.rgb*_colorWater.rgb, _AlphaInt);
            }
            ENDCG
        }
    }
}