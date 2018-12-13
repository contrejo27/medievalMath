Shader "Pat/MS_MagicShield" {
	Properties{
		_MainTex("ForceField Distortion" , 2D) = "white" {}
		_RimColor("Rim Color", Color) = (0,0.5,0.5,0.0)
		_RimPower("Rim Power", Range(0.4,8.0)) = 3.0
		_PanX ("Pan X", Range(-5,5)) = 1
		_PanY("Pan Y", Range(-5,5)) = 1

	}
		SubShader
		{
			Tags { "Queue" = "Transparent" }

			CGPROGRAM
			#pragma surface surf Lambert alpha:fade

			struct Input {
				float3 viewDir;
				float2 uv_MainTex;
			};

			float4 _RimColor;
			float _RimPower;

			struct appdata {
				float4 vertex: POSITION;
				float3 normal: NORMAL;
				float4 texcoord: TEXCOORD0;
			};

			void vert(inout appdata v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
			}

			float _PanX;
			float _PanY;
			sampler2D _MainTex;

			void surf(Input IN, inout SurfaceOutput o) {
				half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
				_PanX *= _Time;
				_PanY *= _Time;
				float2 newuv = IN.uv_MainTex + float2(_PanX, _PanY);

				o.Emission = tex2D(_MainTex, newuv) * (_RimColor.rgb * pow(rim, _RimPower) * 10);
				o.Alpha = pow(rim, _RimPower);
			}
			ENDCG
		}
			Fallback "Diffuse"
	}
