using System.Collections.Generic;
using UnityEngine;

namespace MeshCreator
{
    [RequireComponent(typeof(MeshFilter))]
    public class Cylinder : MonoBehaviour
    {
        public float mergeThreshold = 0.01f;
        public Vector2 minMaxBones = new Vector2(1, 5);
        public float radius = 1f;
        public float height = 2f;
        public int segments = 16;
        public int rings = 1;
        public List<Bone> bones;

        private MeshFilter _meshFilter;

        public Cylinder()
        {
            bones = new List<Bone>();
        }

        public void Generate()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh cylinderMesh = GenerateMesh();
            meshFilter.mesh = cylinderMesh;
        }

        private Mesh GenerateMesh()
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uv = new List<Vector2>();

            float cylinderHeight = height - (2 * radius); // Height of the central cylindrical part
            float angleStep = 360.0f / segments;

            // Generate bottom hemisphere
            for (int ring = 0; ring <= rings; ring++)
            {
                float theta = Mathf.PI * 0.5f * ring / rings; // Angle for hemisphere (from bottom pole to equator)
                float y = Mathf.Sin(theta) * radius; // Y position of the ring
                float ringRadius = Mathf.Cos(theta) * radius; // Radius of the current ring

                for (int segment = 0; segment <= segments; segment++)
                {
                    float angle = segment * angleStep * Mathf.Deg2Rad;
                    float x = Mathf.Cos(angle) * ringRadius;
                    float z = Mathf.Sin(angle) * ringRadius;

                    // Translate down for the bottom hemisphere (move below the center)
                    vertices.Add(new Vector3(x, y - cylinderHeight * 0.5f - radius, z));
                    uv.Add(new Vector2((float)segment / segments, (float)ring / rings));

                    if (ring < rings && segment < segments)
                    {
                        int a = ring * (segments + 1) + segment;
                        int b = a + segments + 1;

                        // Two triangles per quad
                        triangles.Add(a);
                        triangles.Add(b);
                        triangles.Add(a + 1);

                        triangles.Add(a + 1);
                        triangles.Add(b);
                        triangles.Add(b + 1);
                    }
                }
            }

            // Generate central cylinder
            int vertexOffset = vertices.Count;
            for (int ring = 0; ring <= 1; ring++) // Two rings for the cylinder: bottom and top
            {
                float y = ring * cylinderHeight -
                          cylinderHeight * 0.5f; // Central cylinder starts from -height/2 and ends at height/2

                for (int segment = 0; segment <= segments; segment++)
                {
                    float angle = segment * angleStep * Mathf.Deg2Rad;
                    float x = Mathf.Cos(angle) * radius;
                    float z = Mathf.Sin(angle) * radius;

                    vertices.Add(new Vector3(x, y, z));
                    uv.Add(new Vector2((float)segment / segments, (float)ring));

                    if (ring < 1 && segment < segments)
                    {
                        int a = vertexOffset + ring * (segments + 1) + segment;
                        int b = a + segments + 1;

                        // Two triangles per quad
                        triangles.Add(a);
                        triangles.Add(b);
                        triangles.Add(a + 1);

                        triangles.Add(a + 1);
                        triangles.Add(b);
                        triangles.Add(b + 1);
                    }
                }
            }

            // Generate top hemisphere
            vertexOffset = vertices.Count;
            for (int ring = 0; ring <= rings; ring++)
            {
                float theta = Mathf.PI * 0.5f * ring / rings +
                              Mathf.PI * 0.5f; // Angle for top hemisphere (from equator to top pole)
                float y = Mathf.Sin(theta) * radius; // Y position of the ring
                float ringRadius = Mathf.Cos(theta) * radius; // Radius of the current ring

                for (int segment = 0; segment <= segments; segment++)
                {
                    float angle = segment * angleStep * Mathf.Deg2Rad;
                    float x = Mathf.Cos(angle) * ringRadius;
                    float z = Mathf.Sin(angle) * ringRadius;

                    // Translate up for the top hemisphere (move above the center)
                    vertices.Add(new Vector3(x, y + cylinderHeight * 0.5f + radius, z));
                    uv.Add(new Vector2((float)segment / segments, (float)ring / rings));

                    if (ring < rings && segment < segments)
                    {
                        int a = vertexOffset + ring * (segments + 1) + segment;
                        int b = a + segments + 1;

                        // Two triangles per quad
                        triangles.Add(a);
                        triangles.Add(b);
                        triangles.Add(a + 1);

                        triangles.Add(a + 1);
                        triangles.Add(b);
                        triangles.Add(b + 1);
                    }
                }
            }

            // Assign mesh data
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}