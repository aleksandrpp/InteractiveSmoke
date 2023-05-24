Shader "AK.InteractiveSmoke/Point"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _RotationSpeed ("Rotation Speed", Range(0,2)) = 1
        _Scale ("Scale", Range(0,3)) = 1.5
    }
    SubShader
    {
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		LOD 100
		Cull off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert alpha vertex:vert
        #pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
        #pragma target 3.0

        #include "UnityCG.cginc"
        #include "/AngleAxis3x3.cginc"
        #include "/Hash.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        half _RotationSpeed;
        half _Scale;

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<float3> _Points;
        StructuredBuffer<float3> _Colors;
#endif

		void setup() {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			float3 point_ = _Points[unity_InstanceID];
            unity_ObjectToWorld._m03_m13_m23_m33 = float4(point_, 1);
#endif
		}

		void vert(inout appdata_full v) {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            float r_ = rand(_Points[unity_InstanceID]);
            float scale_ = 1 + r_ * _Scale;
			float4x4 scaleMatrix_ = float4x4(scale_, 0, 0, 0,
				                             0, scale_, 0, 0,
				                             0, 0, scale_, 0,
				                             0, 0, 0, 1);

            v.vertex.xyz = mul(v.vertex.xyz, scaleMatrix_);
            v.vertex.xyz = mul(AngleAxis3x3(r_ * UNITY_TWO_PI * _Time.x * _RotationSpeed, float3(0, 1, 0)), v.vertex.xyz);
#endif
		}

        void surf(Input IN, inout SurfaceOutput o) {
			fixed4 color_ = tex2D(_MainTex, IN.uv_MainTex) * _Color;
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            color_ *= fixed4(_Colors[unity_InstanceID], 1);
#endif
			o.Albedo = color_.rgb;
			o.Alpha = color_.a * 0.2;
		}
        ENDCG
    }
    FallBack "Diffuse"
}
