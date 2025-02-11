Shader "Custom/GreenCoverageUnlit" {
    Properties {
        _MainColor ("Основной цвет (серый)", Color) = (0.5,0.5,0.5,1)
        _ChargeColor ("Заряженый цвет", Color) = (0,1,0,1)
        _Charge ("Степень закраски", Range(0,1)) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _MainColor;
            fixed4 _ChargeColor;
            float _Charge;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Если нормаль почти вертикальная, это боковая грань торца.
                if (abs(i.normal.y) > 0.5) {
                    return _MainColor;
                }
                else {
                    // Заполнение снизу вверх: если UV по Y меньше или равно _Charge – зелёный.
                    if (i.uv.y <= _Charge)
                        return _ChargeColor;
                    else
                        return _MainColor;
                }
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}