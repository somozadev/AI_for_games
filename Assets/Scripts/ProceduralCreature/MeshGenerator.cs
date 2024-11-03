using System.Collections.Generic;
using UnityEngine;

namespace ProceduralCreature
{
    public static class MeshGenerator
    {
        private static void CreateSphereMesh(Vector3 position, float radius, List<Vector3> vertices,
            List<int> triangles, int sphereSegments)
        {
            int currentIndex = vertices.Count;

            for (int lat = 0; lat <= sphereSegments; lat++)
            {
                float theta = lat * Mathf.PI / sphereSegments; // vertical angle
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                for (int lon = 0; lon <= sphereSegments; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / sphereSegments; // horizontal angle
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);

                    // Compute the vertex position
                    Vector3 vertex = new Vector3(
                        cosPhi * sinTheta,
                        sinPhi * sinTheta,
                        cosTheta
                    ) * radius + position; // Adjust by the sphere's position

                    vertices.Add(vertex);

                    // Generate the triangles
                    if (lat < sphereSegments && lon < sphereSegments)
                    {
                        int first = currentIndex + lon + (lat * (sphereSegments + 1));
                        int second = currentIndex + lon + ((lat + 1) * (sphereSegments + 1));

                        triangles.Add(first);
                        triangles.Add(second);
                        triangles.Add(first + 1);
                        triangles.Add(second);
                        triangles.Add(second + 1);
                        triangles.Add(first + 1);
                    }
                }
            }
        }

        public static void UpdateMesh(List<BodyPoint> points, int sphereSegments, MeshFilter meshFilter)
        {
            if (points.Count == 0) return;

            // Create a new mesh
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 position = points[i].transform.position;
                float radius = points[i].size;

                // Generate the sphere mesh for each point
                CreateSphereMesh(position, radius, vertices, triangles, sphereSegments);
            }

            // Assign vertices and triangles to the mesh
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            // Update the MeshFilter with the new mesh
            meshFilter.mesh = mesh;

            // Pass the data to the shader
            Vector4[] positions = new Vector4[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                positions[i] = (Vector4)points[i].transform.position;
                positions[i].w = points[i].size; // Store radius in the w component
            }
        }

        public static Mesh GenerateTriangularMesh(List<BodyPoint> bodyPoints)
        {
            Mesh mesh = new Mesh();
            if (bodyPoints.Count < 2) return mesh;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i < bodyPoints.Count; i++)
            {
                BodyPoint bodyPoint = bodyPoints[i];
                Vector3 center = bodyPoint.transform.localPosition;
                float radius = bodyPoint.size / 2;
                var trf = bodyPoint.transform;

                Vector3 top = center + (trf.up * radius);
                Vector3 bottomLeft = (center - (trf.up * radius)) - (trf.right * radius);
                Vector3 bottomRight = (center - (trf.up * radius)) + (trf.right * radius);

                vertices.Add(top);
                vertices.Add(bottomLeft);
                vertices.Add(bottomRight);
            }

            for (int i = 0; i < bodyPoints.Count - 1; i++)
            {
                int currentTop = i * 3;
                int currentBottomLeft = i * 3 + 1;
                int currentBottomRight = i * 3 + 2;

                int nextTop = (i + 1) * 3;
                int nextBottomLeft = (i + 1) * 3 + 1;
                int nextBottomRight = (i + 1) * 3 + 2;
                if (i == 0)
                {
                    triangles.Add(currentBottomLeft);
                    triangles.Add(currentBottomRight);
                    triangles.Add(currentTop);
                    
                    
                    triangles.Add(currentTop);
                    triangles.Add(currentBottomRight);
                    triangles.Add(nextTop);

                    triangles.Add(currentBottomRight);
                    triangles.Add(nextBottomLeft);
                    triangles.Add(nextTop);

                    triangles.Add(nextTop);
                    triangles.Add(currentBottomLeft);
                    triangles.Add(currentTop);

                    triangles.Add(nextTop);
                    triangles.Add(nextBottomRight);
                    triangles.Add(currentBottomLeft);
                    
                    triangles.Add(nextBottomLeft);
                    triangles.Add(currentBottomLeft);
                    triangles.Add(nextBottomRight);
                   
                    triangles.Add(nextBottomLeft);
                    triangles.Add(currentBottomRight);
                    triangles.Add(currentBottomLeft);
                }
                else
                {
                    triangles.Add(currentTop);
                    triangles.Add(nextTop);
                    triangles.Add(currentBottomRight);

                    triangles.Add(nextTop);
                    triangles.Add(nextBottomRight);
                    triangles.Add(currentBottomRight);

                    triangles.Add(nextTop);
                    triangles.Add(currentTop);
                    triangles.Add(currentBottomLeft);

                    triangles.Add(nextTop);
                    triangles.Add(currentBottomLeft);
                    triangles.Add(nextBottomLeft);
                    
                    triangles.Add(currentBottomLeft);
                    triangles.Add(currentBottomRight);
                    triangles.Add(nextBottomLeft);

                    triangles.Add(currentBottomRight);
                    triangles.Add(nextBottomRight);
                    triangles.Add(nextBottomLeft);
                }
                if (i == bodyPoints.Count - 1)
                {
                    triangles.Add(currentTop);
                    triangles.Add(currentBottomRight);
                    triangles.Add(currentBottomLeft);
                }

            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();

            return mesh;
        } public static Mesh GenerateTriangularMesh(List<Point> bodyPoints)
        {
            Mesh mesh = new Mesh();
            if (bodyPoints.Count < 2) return mesh;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i < bodyPoints.Count; i++)
            {
                Point bodyPoint = bodyPoints[i];
                Vector3 center = bodyPoint.transform.localPosition;
                float radius = bodyPoint.size / 2;
                var trf = bodyPoint.transform;

                Vector3 top = center + (trf.up * radius);
                Vector3 bottomLeft = (center - (trf.up * radius)) - (trf.right * radius);
                Vector3 bottomRight = (center - (trf.up * radius)) + (trf.right * radius);

                vertices.Add(top);
                vertices.Add(bottomLeft);
                vertices.Add(bottomRight);
            }

            for (int i = 0; i < bodyPoints.Count - 1; i++)
            {
                int currentTop = i * 3;
                int currentBottomLeft = i * 3 + 1;
                int currentBottomRight = i * 3 + 2;

                int nextTop = (i + 1) * 3;
                int nextBottomLeft = (i + 1) * 3 + 1;
                int nextBottomRight = (i + 1) * 3 + 2;
                if (i == 0)
                {
                    triangles.Add(currentBottomLeft);
                    triangles.Add(currentBottomRight);
                    triangles.Add(currentTop);
                    
                    
                    triangles.Add(currentTop);
                    triangles.Add(currentBottomRight);
                    triangles.Add(nextTop);

                    triangles.Add(currentBottomRight);
                    triangles.Add(nextBottomLeft);
                    triangles.Add(nextTop);

                    triangles.Add(nextTop);
                    triangles.Add(currentBottomLeft);
                    triangles.Add(currentTop);

                    triangles.Add(nextTop);
                    triangles.Add(nextBottomRight);
                    triangles.Add(currentBottomLeft);
                    
                    triangles.Add(nextBottomLeft);
                    triangles.Add(currentBottomLeft);
                    triangles.Add(nextBottomRight);
                   
                    triangles.Add(nextBottomLeft);
                    triangles.Add(currentBottomRight);
                    triangles.Add(currentBottomLeft);
                }
                else
                {
                    triangles.Add(currentTop);
                    triangles.Add(nextTop);
                    triangles.Add(currentBottomRight);

                    triangles.Add(nextTop);
                    triangles.Add(nextBottomRight);
                    triangles.Add(currentBottomRight);

                    triangles.Add(nextTop);
                    triangles.Add(currentTop);
                    triangles.Add(currentBottomLeft);

                    triangles.Add(nextTop);
                    triangles.Add(currentBottomLeft);
                    triangles.Add(nextBottomLeft);
                    
                    triangles.Add(currentBottomLeft);
                    triangles.Add(currentBottomRight);
                    triangles.Add(nextBottomLeft);

                    triangles.Add(currentBottomRight);
                    triangles.Add(nextBottomRight);
                    triangles.Add(nextBottomLeft);
                }
                if (i == bodyPoints.Count - 2)
                {
                    triangles.Add(nextBottomLeft);
                    triangles.Add(nextBottomRight);
                    triangles.Add(nextTop);
                }

            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}