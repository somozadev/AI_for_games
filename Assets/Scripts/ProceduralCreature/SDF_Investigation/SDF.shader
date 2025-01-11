Shader "Custom/SDF"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;  // World position of each fragment
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Parameters for the point shape (positions and sizes of the point points)
        float4 _positions[10];      // Array of positions for the point points
        float _radius[10];        // Array of radii for each point segment
        int _pointCount;         // Number of points in the point chain
        float _Smoothness;           // Smooth blending factor for the SDFs

        // SDF for a sphere
        float SphereSDF(float3 p, float3 center, float radius)
        {
            return length(p - center) - radius;
        }

        // Smooth minimum blending function for SDFs
        float SmoothMin(float d1, float d2, float k)
        {
            float res = exp(-k * d1) + exp(-k * d2);
            return -log(res) / k;
        }

        // Compute the SDF for the point shape by blending sphere SDFs
        float PointSDF(float3 p)
        {
            float d = SphereSDF(p, _positions[0], _radius[0]);
            for (int i = 1; i < _pointCount; i++)
            {
                float sphereDist = SphereSDF(p, _positions[i], _radius[i]);
                d = SmoothMin(d, sphereDist, _Smoothness);
            }
            return d;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Evaluate the SDF for the point at the fragment's world position
            float distance = PointSDF(IN.worldPos);

            // Determine if we are inside or outside the point's surface
            float surfaceThreshold = 0.01; // Small threshold for surface thickness
            if (distance < surfaceThreshold)
            {
                // If we're on or near the surface, apply the main color
                o.Albedo = _Color.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
            }
            else
            {
                // Outside the point: apply a background color or make it transparent
                o.Albedo = float3(0, 0, 0); // Example: black background
                o.Alpha = 0; // Set alpha to zero if transparency is desired
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
