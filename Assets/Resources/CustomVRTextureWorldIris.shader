Shader "Hidden/CustomVRTextureWorldIris" {
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        _Strength("Strength", Float) = 0.0
        _EdgeWidth("EdgeWidth", Float) = 0.0
        _WorldSpaceOrigin("WorldSpaceOrigin", Vector) = (0.0, 0.0, 0.0, 0.0)

		_SrcBlend("SrcBlend", Int) = 5.0 // SrcAlpha
		_DstBlend("DstBlend", Int) = 10.0 // OneMinusSrcAlpha
		_ZWrite("ZWrite", Int) = 1.0 // On
		_ZTest("ZTest", Int) = 4.0 // LEqual
		_Cull("Cull", Int) = 0.0 // Off
		_ZBias("ZBias", Float) = 0.0
    }
    SubShader{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Pass
		{
			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]
			ZTest[_ZTest]
			Cull[_Cull]
			Offset[_ZBias],[_ZBias]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _BaseMap;
            uniform float4 _BaseMap_ST;
            uniform float4 _BaseMap_TexelSize;
            uniform float _Strength;
            uniform float _EdgeWidth;
            uniform float3 _WorldSpaceOrigin;
            sampler2D_float _CameraDepthTexture;

            struct VertIn
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 ray : TEXCOORD1;
            };

            struct VertOut
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv_depth : TEXCOORD1;
                float4 interpolatedRay : TEXCOORD2;
            };

            VertOut vert(VertIn v)
            {
                VertOut o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xy;
                o.uv_depth = v.uv.xy;

#if UNITY_UV_STARTS_AT_TOP
                if (_BaseMap_TexelSize.y < 0)
                    o.uv.y = 1 - o.uv.y;
#endif				

                o.interpolatedRay = v.ray;

                return o;
            }

            fixed4 frag(VertOut i) : SV_Target
            {
                /* - World Space Ray -*/
                float rawDepth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
                float linearDepth = Linear01Depth(rawDepth);
                float4 wsDir = linearDepth * i.interpolatedRay;
                float3 wsPos = _WorldSpaceCameraPos + wsDir;
                
                /* ------------------------- */

                /* --- Ray Closest Point --- */
                float3 ray = -wsDir;
                float len = length(ray);
                float3 normDir = normalize(ray);

                float3 v = _WorldSpaceOrigin - wsPos;
                float d = dot(v, normDir);
                if (d > len) d = len;
                if (d < 0) d = 0;

                float3 closest = wsPos + normDir * d;
                /* ------------------------- */

                /*  Alpha BLend  */
                float dist = distance(closest, _WorldSpaceOrigin);
                
                half4 irisColor = half4(0, 0, 0, 0);
                float max = 20 * _Strength;
                if (dist < max)
                {
                    if (dist > max - _EdgeWidth)
                    {
                        irisColor = half4(1, 1, 1, (max - dist) / _EdgeWidth);
                    }
                    else
                        irisColor = half4(1, 1, 1, 1);
                }
                /* ------------------------- */

                return tex2D(_BaseMap, UnityStereoScreenSpaceUVAdjust(i.uv, _BaseMap_ST)) * irisColor;
            }
            ENDCG

        }
    }
    Fallback Off
}