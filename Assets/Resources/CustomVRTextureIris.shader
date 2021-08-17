// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/CustomVRTextureIris" {
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        _Strength("Strength", Float) = 0.0
        _EdgeWidth("EdgeWidth", Float) = 0.0

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

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //float2x2 rotationMatrix = float2x2(_SpacePoint.x, _SpacePoint.y, _SpacePoint.z, _SpacePoint.w);
                //v.texcoord.xy = mul(v.texcoord.xy, rotationMatrix);
                o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _BaseMap);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dis = sqrt((i.texcoord.y - (0.5)) *
                    (i.texcoord.y - (0.5))
                    + (i.texcoord.x - (0.5)) *
                    (i.texcoord.x - (0.5))
                );
                
                float4 irisColor;
                float max = _Strength * (1 + _EdgeWidth);
                if (dis < max)
                {
                    if (dis > max - _EdgeWidth)
                        irisColor = float4(1.0, 1.0, 1.0, (max - dis) / (_EdgeWidth));
                    else
                        irisColor = float4(1.0, 1.0, 1.0, 1.0);
                }
                else
                    irisColor = float4(1.0, 1.0, 1.0, 0.0);

                return tex2D(_BaseMap, UnityStereoScreenSpaceUVAdjust(i.texcoord, _BaseMap_ST)) * irisColor;
            }
            ENDCG

        }
    }
    Fallback Off
}