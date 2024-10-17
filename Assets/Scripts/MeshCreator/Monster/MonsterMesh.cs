using System;
using System.Collections.Generic;
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
            
            //triangles 2do 
            
            //uvstodo 
            
            //normales tood
            //bpmes tpdp
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
            // ConstructBody(); //regenerateMesh. maybe better calling reconstruct body here
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

    }
}