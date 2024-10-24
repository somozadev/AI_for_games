using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterCreator
{
    public class MonsterMesh : MonoBehaviour
    {
        [SerializeField] private MonsterMeshSettings monsterMeshSettings;
        [SerializeField] private List<MonsterMeshBone> bones;

        
        [SerializeField] private Transform root;
        [SerializeField] private Transform body;
        [SerializeField] private Transform model;
        [SerializeField] private AnimationCurve weightCurve;

        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private Mesh _mesh;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _skinnedMeshRenderer = model.GetComponent<SkinnedMeshRenderer>();
            _mesh = new Mesh();
            _skinnedMeshRenderer.sharedMesh = _mesh;
            _mesh.name = "base";
            bones = new List<MonsterMeshBone>();
            weightCurve ??= AnimationCurve.Linear(0, 1, 1, 0);
        }

        private void Construct()
        {
            ClearMesh();
            List<Vector3> vertices = new List<Vector3>(); 
            List<BoneWeight> weights = new List<BoneWeight>(); 
            
        }

        private void ClearMesh()
        {
            _mesh.Clear();
            _mesh.ClearBlendShapes();
            //vertices
            List<Vector3> vertices = new List<Vector3>();
            List<BoneWeight> weights = new List<BoneWeight>();
            GenerateRingMesh(ref vertices,ref weights, true);
            GenerateMiddleMesh(ref vertices, ref weights);
            GenerateRingMesh(ref vertices,ref weights, false);
            
            _mesh.SetVertices(vertices);
            _mesh.boneWeights = weights.ToArray();
            #region Triangles

            List<int> triangles = new List<int>();

            // Top Cap
            for (var i = 0; i < monsterMeshSettings.Segments; i++)
            {
                triangles.Add(i);
                triangles.Add(i + (monsterMeshSettings.Segments + 1) + 1);
                triangles.Add(i + (monsterMeshSettings.Segments + 1));
            }

            // Main
            var midRings = (2 * (monsterMeshSettings.Segments / 2) - 2) + (monsterMeshSettings.Rings * bones.Count + 1);
            for (var ringIndex = 1; ringIndex < midRings; ringIndex++)
            {
                var ringOffset = ringIndex * (monsterMeshSettings.Segments + 1);
                for (var i = 0; i < monsterMeshSettings.Segments; i++)
                {
                    triangles.Add(ringOffset + i);
                    triangles.Add(ringOffset + i + 1);
                    triangles.Add(ringOffset + i + 1 + (monsterMeshSettings.Segments + 1));

                    triangles.Add(ringOffset + i + 1 + (monsterMeshSettings.Segments + 1));
                    triangles.Add(ringOffset + i + (monsterMeshSettings.Segments + 1));
                    triangles.Add(ringOffset + i);
                }
            }

            // Bottom Cap
            var topOffset = (midRings) * (monsterMeshSettings.Segments + 1);
            for (var i = 0; i < monsterMeshSettings.Segments; i++)
            {
                triangles.Add(topOffset + i);
                triangles.Add(topOffset + i + 1);
                triangles.Add(topOffset + i + (monsterMeshSettings.Segments + 1));
            }

            _mesh.SetTriangles(triangles, 0);

            #endregion
            #region UVs

            List<Vector2> uv = new List<Vector2>();

            var totalRings = midRings + 2;
            for (var ringIndex = 0; ringIndex < totalRings; ringIndex++)
            {
                var v = (ringIndex / (float)totalRings) * (bones.Count + 1);
                for (var i = 0; i < monsterMeshSettings.Segments + 1; i++)
                {
                    var u = i / (float)(monsterMeshSettings.Segments);
                    uv.Add(new Vector2(u, v));
                }
            }

            _mesh.SetUVs(7, uv); // Store copy of UVs in mesh.
            _mesh.uv = _mesh.uv8;

            #endregion
            #region Normals

            Vector3[] normals = new Vector3[vertices.Count];

            for (var i = 0; i < vertices.Count; i++)
            {
                var xyDir = Vector3.ProjectOnPlane(vertices[i], Vector3.forward);
                var zDir = ((i > vertices.Count / 2) ? 1 : -1) * (monsterMeshSettings.Radius - xyDir.magnitude) *
                           Vector3.forward;

                normals[i] = (xyDir + zDir).normalized;
            }

            _mesh.SetNormals(normals);

            #endregion            //bpmes tpdp
            
            #region Bones

            Matrix4x4[] bindPoses = new Matrix4x4[bones.Count];
            Vector3[] deltaZeroArray = new Vector3[vertices.Count];
            for (var vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
                deltaZeroArray[vertIndex] = Vector3.zero;

            for (var boneIndex = 0; boneIndex < bones.Count; boneIndex++)
            {
                // Bind Pose
                bones[boneIndex].position = Vector3.forward *
                                                  (monsterMeshSettings.Radius +
                                                   monsterMeshSettings.Length * (0.5f + boneIndex));
                bones[boneIndex].rotation = Quaternion.identity;
                bindPoses[boneIndex] = bones[boneIndex].transform.worldToLocalMatrix * body.localToWorldMatrix;

                // Blend Shapes
                Vector3[] deltaVertices = new Vector3[vertices.Count];
                for (var vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
                {
                    var maxDistanceAlongBone = monsterMeshSettings.Length * 2f;
                    var maxHeightAboveBone = monsterMeshSettings.Radius * 2f;

                    var displacementAlongBone = vertices[vertIndex].z - bones[boneIndex].position.z;

                    var x = Mathf.Clamp(displacementAlongBone / maxDistanceAlongBone, -1, 1);
                    var a = maxHeightAboveBone;
                    var b = 1f / a;

                    var heightAboveBone = (Mathf.Cos(x * Mathf.PI) / b + a) / 2f;

                    deltaVertices[vertIndex] = new Vector2(vertices[vertIndex].x, vertices[vertIndex].y).normalized *
                                               heightAboveBone;
                }

                _mesh.AddBlendShapeFrame("Bone." + boneIndex, 0, deltaZeroArray, deltaZeroArray, deltaZeroArray);
                _mesh.AddBlendShapeFrame("Bone." + boneIndex, 100, deltaVertices, deltaZeroArray, deltaZeroArray);

                // OnSetupBone?.Invoke(boneIndex);
            }

            _mesh.bindposes = bindPoses;
            Transform[] bonestrf = bones.Select(bone => bone.transform).ToArray();
            List<float> boneWeights = (List<float>)bones.Select(bone => bone.weight);
            _skinnedMeshRenderer.bones = bonestrf;

            #endregion

            for (int boneIndex = 0; boneIndex < bones.Count; boneIndex++)
            {
                bones[boneIndex].position = transform.TransformPoint(bones[boneIndex].position);
                bones[boneIndex].rotation = transform.rotation * bones[boneIndex].rotation;
                SetBlendShapeWeight(boneIndex, bones[boneIndex].weight);
            }

            ApplyBoneWeights(vertices, boneWeights);
            
        }

        private void GenerateRingMesh(ref List<Vector3> vertices ,ref  List<BoneWeight> weights, bool topHemisphere)
        {
            if(topHemisphere)
            {
                for (var ringIndex = 0; ringIndex < monsterMeshSettings.Segments / 2; ringIndex++)
                {
                    var percent = (float)ringIndex / (monsterMeshSettings.Segments / 2);
                    var ringRadius = monsterMeshSettings.Radius * Mathf.Sin(90f * percent * Mathf.Deg2Rad);
                    var ringDistance = monsterMeshSettings.Radius * (-Mathf.Cos(90f * percent * Mathf.Deg2Rad) + 1f);
                    for (var i = 0; i < monsterMeshSettings.Segments + 1; i++)
                    {
                        var angle = i * 360f / monsterMeshSettings.Segments;
                        var x = ringRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                        var y = ringRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                        var z = ringDistance;
                        vertices.Add(new Vector3(x, y, z));
                        weights.Add(new BoneWeight() { boneIndex0 = 0, weight0 = 1f });
                    }
                }
            }
            else
            {
                for (var ringIndex = 0; ringIndex < monsterMeshSettings.Segments / 2 + 1; ringIndex++)
                {
                    var percent = (float)ringIndex / (monsterMeshSettings.Segments / 2);
                    var ringRadius = monsterMeshSettings.Radius * Mathf.Cos(90f * percent * Mathf.Deg2Rad);
                    var ringDistance = monsterMeshSettings.Radius * Mathf.Sin(90f * percent * Mathf.Deg2Rad);

                    for (var i = 0; i < monsterMeshSettings.Segments + 1; i++)
                    {
                        var angle = i * 360f / monsterMeshSettings.Segments;

                        var x = ringRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                        var y = ringRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                        var z = ringDistance;

                        vertices.Add(new Vector3(x, y, monsterMeshSettings.Radius + (monsterMeshSettings.Length * bones.Count) + z));
                        weights.Add(new BoneWeight() { boneIndex0 = bones.Count - 1, weight0 = 1 });
                    }
                }
            }
        }

        private void GenerateMiddleMesh(ref List<Vector3> vertices ,ref  List<BoneWeight> weights)
        {
            for (var ringIndex = 0; ringIndex < monsterMeshSettings.Rings * bones.Count; ringIndex++)
            {
                var boneIndexFloat = (float)ringIndex / monsterMeshSettings.Rings;
                var boneIndex = Mathf.FloorToInt(boneIndexFloat);
                var bonePercent = boneIndexFloat - boneIndex;
                var boneIndex0 = (boneIndex > 0) ? boneIndex - 1 : 0;
                var boneIndex2 = (boneIndex < bones.Count - 1) ? boneIndex + 1 : bones.Count - 1;
                var boneIndex1 = boneIndex;
                var weight0 = (boneIndex > 0) ? (1f - bonePercent) * 0.5f : 0f;
                var weight2 = (boneIndex < bones.Count - 1) ? bonePercent * 0.5f : 0f;
                var weight1 = 1f - (weight0 + weight2);

                for (var i = 0; i < monsterMeshSettings.Segments + 1; i++)
                {
                    var angle = i * 360f / monsterMeshSettings.Segments;

                    var x = monsterMeshSettings.Radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    var y = monsterMeshSettings.Radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    var z = ringIndex * monsterMeshSettings.Length / monsterMeshSettings.Rings;

                    vertices.Add(new Vector3(x, y, monsterMeshSettings.Radius + z));
                    weights.Add(new BoneWeight()
                    {
                        boneIndex0 = boneIndex0,
                        boneIndex1 = boneIndex1,
                        boneIndex2 = boneIndex2,
                        weight0 = weight0,
                        weight1 = weight1,
                        weight2 = weight2
                    });
                }
            }
        }
        private void AddBone(Vector3 pos, Quaternion quat, float weight, bool front)
        {
            var go = new GameObject("Bone." + bones.Count);
            go.AddComponent<MonsterMeshBone>();
            go.transform.SetParent(root);
            if(front)
                bones.Add(go.GetComponent<MonsterMeshBone>());
            else
                bones.Insert( 0,go.GetComponent<MonsterMeshBone>());
            Construct(); //regenerateMesh. maybe better calling reconstruct body here
        }
        
        public void AddBoneToFront()
        {
            if(bones.Count <0 )return;
            var pos = bones[0].transform.position - bones[0].transform.forward * monsterMeshSettings.Length;
            var rot = bones[0].transform.rotation;

            pos = transform.InverseTransformPoint(pos);
            rot = Quaternion.Inverse(transform.rotation) * rot;
            AddBone(pos, rot, GetBlendShapeWeight(0), true);
        }
        public void AddBoneToBack()
        {
            if(bones.Count <0 )return;
            var index = bones.Count - 1;
            var pos = bones[index].transform.position - bones[index].transform.forward * monsterMeshSettings.Length;
            var rot = bones[index].transform.rotation;

            pos = transform.InverseTransformPoint(pos);
            rot = Quaternion.Inverse(transform.rotation) * rot;
            AddBone(pos, rot, GetBlendShapeWeight(index) * 0.75f, true);
        }
        private float GetBlendShapeWeight(int index)
        {
            return _skinnedMeshRenderer.GetBlendShapeWeight(index);
        }
        private void SetBlendShapeWeight(int index, float weight)
        {
            weight = Mathf.Clamp(weight, 0f, 100f);
            bones[index].weight = weight;
            _skinnedMeshRenderer.SetBlendShapeWeight(index, weight);

            // UpdateOrigin();
        }
        private void ApplyBoneWeights(List<Vector3> vertices, List<BoneWeight> boneWeights)
        {
            for (int i = 0; i < boneWeights.Count; i++)
            {
                var boneWeight = boneWeights[i];
                var distanceAlongBone = vertices[i].z / (monsterMeshSettings.Length * bones.Count);

                float weightModifier = weightCurve.Evaluate(distanceAlongBone);

                boneWeight.weight0 *= weightModifier;
                boneWeight.weight1 *= weightModifier;
                boneWeight.weight2 *= weightModifier;

                boneWeights[i] = boneWeight;
            }
        }
    }
}