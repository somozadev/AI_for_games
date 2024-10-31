using System;
using System.Collections.Generic;
using System.Linq;
using MonsterCreator;
using UnityEngine;
using UnityEngine.Serialization;

namespace MonsterCreator
{
    public class Monster : MonoBehaviour
    {
        [SerializeField] private List<MonsterMesh> parts;
        [SerializeField] private MonsterMesh monsterHead;
        [SerializeField] private MonsterMesh MonsterBody;
        [SerializeField] private MonsterMesh monsterLeg1;
        [SerializeField] private MonsterMesh monsterLeg2;
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private Mesh combinedMesh;

        private void Awake()
        {
            InitializeMonster();
        }

        [ContextMenu("test")]
        public void Test()
        {
            AddPart(monsterHead);
            AddPart(MonsterBody);
            AddPart(monsterLeg1);
            AddPart(monsterLeg2);
        }

        private void InitializeMonster()
        {
            _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            combinedMesh = new Mesh { name = "CombinedMonsterMesh" };
            _skinnedMeshRenderer.sharedMesh = combinedMesh;

            parts = new List<MonsterMesh>();
        }

        public void AddPart(MonsterMesh part)
        {
            parts.Add(part);
            CombineMeshes();
        }

        private void CombineMeshes()
        {
            // List to hold the meshes of the monster meshes
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            List<BoneWeight> combinedBoneWeights = new List<BoneWeight>();
            int currentVertexOffset = 0; // Current offset for each mesh

            // Iterate through each MonsterMesh to combine their meshes
            for (int i = 0; i < parts.Count; i++)
            {
                var monsterMesh = parts[i];
                if (monsterMesh.MeshRenderer != null)
                {
                    CombineInstance combineInstance = new CombineInstance
                    {
                        mesh = monsterMesh.MeshRenderer.sharedMesh,
                        transform = monsterMesh.transform.localToWorldMatrix
                    };

                    combineInstances.Add(combineInstance);

                    // Get the vertex count and bone weights for the current mesh
                    int vertexCount = combineInstance.mesh.vertexCount;
                    BoneWeight[] boneWeights = combineInstance.mesh.boneWeights;

                    // Log vertex count and bone weight information
                    Debug.Log($"Mesh {i}: Vertex Count = {vertexCount}, Bone Weights Count = {boneWeights.Length}");

                    // Collect the bone weights for the current mesh
                    if (boneWeights.Length > 0)
                    {
                        for (int j = 0; j < boneWeights.Length; j++)
                        {
                            BoneWeight bw = boneWeights[j];
                            // Adjust bone indices to match combined mesh
                            bw.boneIndex0 += currentVertexOffset;
                            bw.boneIndex1 += currentVertexOffset;
                            bw.boneIndex2 += currentVertexOffset;
                            bw.boneIndex3 += currentVertexOffset;
                            combinedBoneWeights.Add(bw);
                        }
                    }
                    else
                    {
                        // If no bone weights, add default weights for this mesh's vertices
                        for (int j = 0; j < vertexCount; j++)
                        {
                            combinedBoneWeights.Add(new BoneWeight()); // Default bone weight
                        }
                    }

                    // Update offset for next mesh
                    currentVertexOffset += vertexCount;
                }
            }

            // Combine the meshes
            combinedMesh.Clear(); // Clear the previous mesh data
            combinedMesh.CombineMeshes(combineInstances.ToArray(), false, true);

            // Log the combined counts
            Debug.Log($"Total Combined Vertex Count: {combinedMesh.vertexCount}");
            Debug.Log($"Total Combined Bone Weights Count: {combinedBoneWeights.Count}");

            // Ensure the combined bone weights array matches the total vertex count
            if (combinedBoneWeights.Count != combinedMesh.vertexCount)
            {
                Debug.LogError(
                    $"The combined bone weights count ({combinedBoneWeights.Count}) does not match the combined mesh vertex count ({combinedMesh.vertexCount}).");
                return; // Exit to avoid further errors
            }

            // Assign the combined bone weights to the combined mesh
            combinedMesh.boneWeights = combinedBoneWeights.ToArray();

            // If a SkinnedMeshRenderer already exists, we might want to reuse it instead of adding a new one
            if (_skinnedMeshRenderer == null)
            {
                _skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
            }

            // Assign the combined mesh to the SkinnedMeshRenderer
            _skinnedMeshRenderer.sharedMesh = combinedMesh;

            // Assign bones
            Transform[] allBones = new Transform[0];
            foreach (var monsterMesh in parts)
            {
                Transform[] meshBones = monsterMesh.MeshRenderer.bones;
                Transform[] newBones = new Transform[allBones.Length + meshBones.Length];
                System.Array.Copy(allBones, newBones, allBones.Length);
                System.Array.Copy(meshBones, 0, newBones, allBones.Length, meshBones.Length);
                allBones = newBones;
            }

            _skinnedMeshRenderer.bones = allBones;

            // Combine blend shapes
            CombineBlendShapes();
        }


        private void CombineBlendShapes()
        {
            // Combine the blend shapes from each MonsterMesh
            for (int i = 0; i < parts.Count; i++)
            {
                var monsterMesh = parts[i];
                var mesh = monsterMesh.MeshRenderer.sharedMesh;

                // Iterate through each blend shape
                for (int j = 0; j < mesh.blendShapeCount; j++)
                {
                    // Get the name and vertex count of the blend shape
                    string blendShapeName = mesh.GetBlendShapeName(j);
                    int vertexCount = mesh.vertexCount;

                    // Create arrays to hold the delta vertices for the blend shapes
                    Vector3[] deltaVertices = new Vector3[vertexCount];
                    Vector3[] deltaNormals = new Vector3[vertexCount];
                    Vector3[] deltaTangents = new Vector3[vertexCount];

                    // Sum the delta vertices from each monster mesh's blend shape
                    for (int k = 0; k < parts.Count; k++)
                    {
                        var currentMesh = parts[k].MeshRenderer.sharedMesh;
                        if (currentMesh == null || currentMesh.blendShapeCount <= j) continue;

                        // Get the delta vertices for the current blend shape
                        Vector3[] currentDeltaVertices = new Vector3[vertexCount];
                        currentMesh.GetBlendShapeFrameVertices(j, 0, currentDeltaVertices, null, null);

                        // Sum the delta vertices
                        for (int v = 0; v < vertexCount; v++)
                        {
                            deltaVertices[v] += currentDeltaVertices[v];
                        }
                    }

                    // Add the combined blend shape frame to the combined mesh
                    combinedMesh.AddBlendShapeFrame(blendShapeName, 100, deltaVertices, deltaNormals, deltaTangents);
                }
            }
        }
    }
}