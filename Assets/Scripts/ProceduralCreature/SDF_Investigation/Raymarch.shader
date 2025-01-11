Shader "Custom/RaymarchAndBlend"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaxDistance ("Max Distance", Float) = 100.0
        _Sphere1 ("Sphere 1", Vector) = (0,0,0,1)
        _Sphere2 ("Sphere 2", Vector) = (0,0,0,1)
        // Añade más esferas según sea necesario
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Función para calcular la distancia a la esfera
            float sdSphere(float3 pos, float radius)
            {
                return length(pos) - radius;
            }

            // Función para calcular la distancia total
            float distanceField(float3 pos)
            {
                float d = _MaxDistance; // Valor por defecto
                // Distancia a cada esfera
                d = min(d, sdSphere(pos - _Sphere1.xyz, _Sphere1.w));
                d = min(d, sdSphere(pos - _Sphere2.xyz, _Sphere2.w));
                // Añadir más esferas según sea necesario
                return d;
            }

            fixed4 raymarching(float3 ro, float3 rd)
            {
                fixed4 color = fixed4(1, 1, 1, 1); // Color por defecto
                const int max_iterations = 64;
                float t = 0.0; // Distancia recorrida

                for (int i = 0; i < max_iterations; i++)
                {
                    if (t > _MaxDistance)
                    {
                        // Si se superó la distancia máxima, se retorna el color del fondo
                        return fixed4(0.5, 0.5, 0.5, 1); // Color gris
                    }

                    float3 pos = ro + rd * t;
                    float distance = distanceField(pos);
                    if (distance < 0.01) // Si se ha encontrado una esfera
                    {
                        return fixed4(1, 0, 0, 1); // Color rojo
                    }
                    t += distance; // Moverse a lo largo de la dirección del rayo
                }

                return color;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 ray_direction = normalize(float3(i.uv, 1.0));
                float3 ray_origin = _WorldSpaceCameraPos;

                // Raymarching para el color de las esferas
                fixed4 result = raymarching(ray_origin, ray_direction);
                return result;
            }
            ENDCG
        }
    }
}
