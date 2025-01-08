using System;
using System.Collections.Generic;
using System.Linq;
using Genetics;
using UnityEngine;
// using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace ProceduralCreature
{
    public class Body : MonoBehaviour
    {
        [SerializeField] private CreatureInfoDisplay _display;
        
        public CreatureConfiguration configuration;
        // private SplineContainer _spline;

        public List<BodyPoint> points;
        public int nOfPoints = 16;
        [SerializeField] GameObject pointPrefab;
        public MeshFilter _meshFilter;

        public MeshRenderer _meshRenderer;

        public int segments = 32;

        public bool generateMesh = false;

        public Material sdfMaterial;
        public float smoothness = 8.0f;
        private Color32 currentColor;

        private CreatureContainer _container;
        public void ConfigureBody(CreatureContainer container, Color32 color, int bodyLength, int numLegsPoints, float sizeScale)
        {
            _container = container;
            if (generateMesh)
            {
                // _meshFilter = gameObject.AddComponent<MeshFilter>();
                // _spline = gameObject.GetComponent<SplineContainer>();
                // _meshRenderer = gameObject.GetComponent<MeshRenderer>();
                // _meshRenderer.material = sdfMaterial;
            }

            configuration.bodyLength = bodyLength;
            configuration.bodySizes = new float[bodyLength];
            for (int i = 0; i < bodyLength; i++)
                configuration.bodySizes[i] = Random.Range(0.1f, 0.2f + sizeScale);
            if (numLegsPoints == 0)
                numLegsPoints = 1;
            configuration.legsInBodyN = numLegsPoints;
            SetNewMaterial(color);
            currentColor = color;

            points = new List<BodyPoint>();
            Init();
        }

        public void SetNewMaterial(Color32 color)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshRenderer != null)
            {
                if (_meshRenderer.material != null)
                {
                    _meshRenderer.material.color = color;
                }
                else
                {
                    Material newMaterial = new Material(Shader.Find("Standard"));
                    newMaterial.color = color;
                    _meshRenderer.material = newMaterial;
                }
            }
        }


        private void Init()
        {
            float[] sizes = configuration.bodySizes;

            if (configuration)
            {
                nOfPoints = configuration.bodyLength;
                sizes = configuration.bodySizes;
            }


            for (int i = 0; i < nOfPoints; i++)
            {
                var p = Instantiate(pointPrefab, transform);
                p.name = "point_" + i;
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y,
                    p.transform.position.z - i * 4);
                var bp = p.GetComponent<BodyPoint>();
                bp.ScaleBy(sizes.Length > 0 ? 1 + 1.25f * sizes[i] : Random.Range(1.0f, 1.0f));

                if (bp.GetComponentInChildren<MeshRenderer>().material != null)
                {
                    bp.GetComponentInChildren<MeshRenderer>().material.color = currentColor;
                }
                else
                {
                    Material newMaterial = new Material(Shader.Find("Standard"));
                    newMaterial.color = currentColor;
                    bp.GetComponentInChildren<MeshRenderer>().material = newMaterial;
                }

                bp.SetIndex(i + 1);
                points.Add(bp);
            }

            points[0].gameObject.AddComponent<CreaturePlayerController>();
            points[0].gameObject.AddComponent<BodyTriggerDetector>().Init(_container, _container.Creature.Chromosome.BasicStats.perception);
            _display.transform.SetParent(points[0].transform);
            
            for (int i = 0; i < points.Count; i++)
            {
                if (i > 0)
                    points[i].parent_point = points[i - 1];
                if (i < points.Count - 1)
                    points[i].children_point = points[i + 1];
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (configuration)
                    points[i].Init(configuration ? i % configuration.legsInBodyN == 0 : i % 2 == 0);
                else points[i].Init(i % 2 == 0);
            }
            //     if (generateMesh)
            //         _meshFilter.mesh = MeshGenerator.GenerateTriangularMesh(points);
            //
            // if (generateMesh)
            //     MeshGenerator.GenerateSplineMesh(_spline, points);
        }

        private void Update()
        {
            foreach (var point in points)
            {
                point.Ping();
            }

            // if (generateMesh)
            //     _meshFilter.mesh = MeshGenerator.GenerateTriangularMesh(points);

            // if (generateMesh)
            //     MeshGenerator.UpdateSplineMesh(_spline, points);
        }


        private void OnDrawGizmos()
        {
            Point lastPoint = null;
            foreach (var point in points)
            {
                if (point != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(point.transform.position, point.size);
                    Gizmos.color = Color.magenta;
                    if (lastPoint != null)
                        Gizmos.DrawLine(point.transform.position, lastPoint.transform.position);
                }

                lastPoint = point;
            }
        }
    }
}

#region tests

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

#endregion