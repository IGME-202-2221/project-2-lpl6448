Shader "Custom/Tree_shader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _SecondaryColor ("Secondary Color", Color) = (1,1,1,1)
		_Transition ("Transition", Range(0,1)) = 0.5
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 normal;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _BaseColor;
        fixed4 _SecondaryColor;
		float _Transition;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.normal = v.normal;
		}
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			fixed4 color;
			if (IN.normal.z > -0.5)
				IN.uv_MainTex.y = 1 - IN.uv_MainTex.y;
			if (_Transition <= 0)
				color = _BaseColor;
			else if (_Transition >= 1)
				color = _SecondaryColor;
			else if (IN.normal.y < -0.5)
				color = _BaseColor;
			else if (IN.normal.y > 0.5)
				color = _SecondaryColor;
			else if (IN.uv_MainTex.y > _Transition)
				color = _BaseColor;
			else
				color = _SecondaryColor;
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
