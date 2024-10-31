using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralCreature
{
    public class ProceduralBody : MonoBehaviour
    {
        public List<Point> points;
        [SerializeField] GameObject pointPrefab;
        public MeshFilter _meshFilter;
        public MeshRenderer _meshRenderer;
        // public int verticesPerRing = 32;
        public int segments = 32;

        //SDF rendering vs mesh 
        public Material sdfMaterial;
        public float smoothness = 8.0f;


        private void Awake()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = sdfMaterial; 
            points = new List<Point>();
            Init();
            MeshGenerator.UpdateMesh(points,segments, _meshFilter, sdfMaterial);
        }

        private void Init()
        {
            for (int i = 0; i < 10; i++)
            {
                var p = GameObject.Instantiate(pointPrefab, transform);
                p.name = "point_" + i;
                p.transform.position = new Vector3(p.transform.position.x + i * 4, p.transform.position.y,
                    p.transform.position.z);
                p.GetComponent<Point>().ScaleBy(Random.Range(1.0f, 9.0f));
                points.Add(p.GetComponent<Point>());
                p.GetComponent<Point>().SetIndex(i + 1);
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (i > 0)
                    points[i].parent_point = points[i - 1];
                if (i < points.Count - 1)
                    points[i].children_point = points[i + 1];
            }
        }

        private void Update()
        {
            foreach (var point in points)
            {
                point.Ping();
            }
            MeshGenerator.UpdateMesh(points,segments, _meshFilter, sdfMaterial);
            // GenerateMesh();
        }

        // private void UpdateSDF()
        // {
        //     Vector4[] positions = points.Select(point => (Vector4)point.transform.position).ToArray();
        //     for (int i = 0; i < points.Count; i++)
        //         positions[i].w = 1.0f; // Set the w component to 1 for compatibility
        //
        //
        //     float[] radius = points.Select(point => point.size).ToArray();
        //     sdfMaterial.SetVectorArray("_positions", positions);
        //     sdfMaterial.SetFloatArray("_radius", radius);
        //     sdfMaterial.SetInt("_pointCount", points.Count);
        //     sdfMaterial.SetFloat("_Smoothness", smoothness);
        // }

        
        
        // private void GenerateMesh()
        // {
        //     Mesh mesh = new Mesh();
        //     List<Vector3> vertices = new List<Vector3>();
        //     List<int> triangles = new List<int>();
        //
        //     // Loop through each Point to build circular slices
        //     for (int i = 0; i < points.Count; i++)
        //     {
        //         Point point = points[i];
        //
        //         // Determine direction for each point's circle based on chain orientation
        //         Vector3 direction = (i < points.Count - 1)
        //             ? (points[i + 1].transform.position - point.transform.position).normalized
        //             : (point.transform.position - points[i - 1].transform.position).normalized;
        //
        //         // Get a perpendicular vector in the XZ plane
        //         Vector3 up = Vector3.up;
        //         Vector3 normal = Vector3.Cross(direction, up).normalized;
        //
        //         // Rotate the normal around the direction to form a circle
        //         for (int j = 0; j < verticesPerRing; j++)
        //         {
        //             float angle = (j / (float)verticesPerRing) * Mathf.PI * 2;
        //             Quaternion rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, direction);
        //             Vector3 ringPoint = point.transform.position + rotation * normal * point.size * 4;
        //             vertices.Add(ringPoint);
        //         }
        //     }
        //
        //     // Create triangles between consecutive rings
        //     for (int i = 0; i < points.Count - 1; i++)
        //     {
        //         int startIndexCurrentRing = i * verticesPerRing;
        //         int startIndexNextRing = (i + 1) * verticesPerRing;
        //
        //         for (int j = 0; j < verticesPerRing; j++)
        //         {
        //             int nextJ = (j + 1) % verticesPerRing;
        //
        //             // First triangle of quad (corrected order)
        //             triangles.Add(startIndexCurrentRing + j);
        //             triangles.Add(startIndexNextRing + nextJ);
        //             triangles.Add(startIndexNextRing + j);
        //
        //             // Second triangle of quad (corrected order)
        //             triangles.Add(startIndexCurrentRing + j);
        //             triangles.Add(startIndexCurrentRing + nextJ);
        //             triangles.Add(startIndexNextRing + nextJ);
        //         }
        //     }
        //
        //     // Assign vertices and triangles to mesh
        //     mesh.vertices = vertices.ToArray();
        //     mesh.triangles = triangles.ToArray();
        //     mesh.RecalculateNormals();
        //
        //     // Set the mesh to the MeshFilter
        //     _meshFilter.mesh = mesh;
        // }

        private void OnDrawGizmos()
        {
            foreach (var point in points)
            {
                if (point != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(point.transform.position, point.size);
                }
            }
        }
    }
}