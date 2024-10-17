using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCreator
{
    public class Monster : MonoBehaviour
    {
        [SerializeField] private BoneSettings _boneSettings;
        [SerializeField] private MinMax minMaxBones;
        [SerializeField] private MinMax minMaxBoneBlendShapeWeights;
        private List<Bone> _bones;
        private List<Transform> _bonesTransforms;

        [SerializeField] private Mesh _mesh;
        [SerializeField] private Transform _root;
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _model;
        private SkinnedMeshRenderer _skinnedMeshRenderer;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _root = transform.GetChild(0);
            _skinnedMeshRenderer = _model.GetComponent<SkinnedMeshRenderer>();
            // SkinnedMeshRenderer.material = BodyMat = new Material(bodyMaterial);
            _mesh = new Mesh();
            _skinnedMeshRenderer.sharedMesh = _mesh;
            _mesh.name = "Body";
            _bones = new List<Bone>();
            _bonesTransforms = new List<Transform>();
            // Bone b = new Bone(Vector3.zero, Quaternion.identity, 1);
            // _bones.Add(b);
        }

        // private void Construct()
        // {
        //     Bone b = new Bone(Vector3.zero, Quaternion.identity, 1);
        //     List<Bone> auxBones = new List<Bone>();
        //     auxBones.Add(b);
        //     for (int i = 0; i < auxBones.Count; i++)
        //     {
        //         Bone bone = auxBones[i];
        //         AddBone(i, bone.position, bone.rotation, bone.weight);
        //     }
        // }

        private void ConstructBody()
        {
            _mesh.Clear();
            _mesh.ClearBlendShapes();

            #region Vertices

            List<Vector3> vertices = new List<Vector3>();
            List<BoneWeight> boneWeights = new List<BoneWeight>();

            #region Top hemisphere

            for (var ringIndex = 0; ringIndex < _boneSettings.Segments / 2; ringIndex++)
            {
                var percent = (float)ringIndex / (_boneSettings.Segments / 2);
                var ringRadius = _boneSettings.Radius * Mathf.Sin(90f * percent * Mathf.Deg2Rad);
                var ringDistance = _boneSettings.Radius * (-Mathf.Cos(90f * percent * Mathf.Deg2Rad) + 1f);
                for (var i = 0; i < _boneSettings.Segments + 1; i++)
                {
                    var angle = i * 360f / _boneSettings.Segments;
                    var x = ringRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    var y = ringRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    var z = ringDistance;
                    vertices.Add(new Vector3(x, y, z));
                    boneWeights.Add(new BoneWeight() { boneIndex0 = 0, weight0 = 1f });
                }
            }

            #endregion

            #region Middle cylinder

            for (var ringIndex = 0; ringIndex < _boneSettings.Rings * _bones.Count; ringIndex++)
            {
                var boneIndexFloat = (float)ringIndex / _boneSettings.Rings;
                var boneIndex = Mathf.FloorToInt(boneIndexFloat);
                var bonePercent = boneIndexFloat - boneIndex;
                var boneIndex0 = (boneIndex > 0) ? boneIndex - 1 : 0;
                var boneIndex2 = (boneIndex < _bones.Count - 1) ? boneIndex + 1 : _bones.Count - 1;
                var boneIndex1 = boneIndex;
                var weight0 = (boneIndex > 0) ? (1f - bonePercent) * 0.5f : 0f;
                var weight2 = (boneIndex < _bones.Count - 1) ? bonePercent * 0.5f : 0f;
                var weight1 = 1f - (weight0 + weight2);

                for (var i = 0; i < _boneSettings.Segments + 1; i++)
                {
                    var angle = i * 360f / _boneSettings.Segments;

                    var x = _boneSettings.Radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    var y = _boneSettings.Radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    var z = ringIndex * _boneSettings.Length / _boneSettings.Rings;

                    vertices.Add(new Vector3(x, y, _boneSettings.Radius + z));
                    boneWeights.Add(new BoneWeight()
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

            #endregion

            #region Bot hemisphere

            for (var ringIndex = 0; ringIndex < _boneSettings.Segments / 2 + 1; ringIndex++)
            {
                var percent = (float)ringIndex / (_boneSettings.Segments / 2);
                var ringRadius = _boneSettings.Radius * Mathf.Cos(90f * percent * Mathf.Deg2Rad);
                var ringDistance = _boneSettings.Radius * Mathf.Sin(90f * percent * Mathf.Deg2Rad);

                for (var i = 0; i < _boneSettings.Segments + 1; i++)
                {
                    var angle = i * 360f / _boneSettings.Segments;

                    var x = ringRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    var y = ringRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    var z = ringDistance;

                    vertices.Add(new Vector3(x, y, _boneSettings.Radius + (_boneSettings.Length * _bones.Count) + z));
                    boneWeights.Add(new BoneWeight() { boneIndex0 = _bones.Count - 1, weight0 = 1 });
                }
            }

            #endregion

            _mesh.SetVertices(vertices);
            _mesh.boneWeights = boneWeights.ToArray();

            #endregion

            #region Triangles

            List<int> triangles = new List<int>();

            // Top Cap
            for (var i = 0; i < _boneSettings.Segments; i++)
            {
                triangles.Add(i);
                triangles.Add(i + (_boneSettings.Segments + 1) + 1);
                triangles.Add(i + (_boneSettings.Segments + 1));
            }

            // Main
            var midRings = (2 * (_boneSettings.Segments / 2) - 2) + (_boneSettings.Rings * _bones.Count + 1);
            for (var ringIndex = 1; ringIndex < midRings; ringIndex++)
            {
                var ringOffset = ringIndex * (_boneSettings.Segments + 1);
                for (var i = 0; i < _boneSettings.Segments; i++)
                {
                    triangles.Add(ringOffset + i);
                    triangles.Add(ringOffset + i + 1);
                    triangles.Add(ringOffset + i + 1 + (_boneSettings.Segments + 1));

                    triangles.Add(ringOffset + i + 1 + (_boneSettings.Segments + 1));
                    triangles.Add(ringOffset + i + (_boneSettings.Segments + 1));
                    triangles.Add(ringOffset + i);
                }
            }

            // Bottom Cap
            var topOffset = (midRings) * (_boneSettings.Segments + 1);
            for (var i = 0; i < _boneSettings.Segments; i++)
            {
                triangles.Add(topOffset + i);
                triangles.Add(topOffset + i + 1);
                triangles.Add(topOffset + i + (_boneSettings.Segments + 1));
            }

            _mesh.SetTriangles(triangles, 0);

            #endregion

            #region UVs

            List<Vector2> uv = new List<Vector2>();

            var totalRings = midRings + 2;
            for (var ringIndex = 0; ringIndex < totalRings; ringIndex++)
            {
                var v = (ringIndex / (float)totalRings) * (_bones.Count + 1);
                for (var i = 0; i < _boneSettings.Segments + 1; i++)
                {
                    var u = i / (float)(_boneSettings.Segments);
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
                var zDir = ((i > vertices.Count / 2) ? 1 : -1) * (_boneSettings.Radius - xyDir.magnitude) *
                           Vector3.forward;

                normals[i] = (xyDir + zDir).normalized;
            }

            _mesh.SetNormals(normals);

            #endregion

            #region Bones

            Transform[] boneTransforms = new Transform[_bones.Count];
            Matrix4x4[] bindPoses = new Matrix4x4[_bones.Count];
            Vector3[] deltaZeroArray = new Vector3[vertices.Count];
            for (var vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
                deltaZeroArray[vertIndex] = Vector3.zero;

            for (var boneIndex = 0; boneIndex < _bones.Count; boneIndex++)
            {
                boneTransforms[boneIndex] = _bonesTransforms[boneIndex];

                // Bind Pose
                boneTransforms[boneIndex].localPosition = Vector3.forward *
                                                          (_boneSettings.Radius +
                                                           _boneSettings.Length * (0.5f + boneIndex));
                boneTransforms[boneIndex].localRotation = Quaternion.identity;
                bindPoses[boneIndex] = boneTransforms[boneIndex].worldToLocalMatrix * _body.localToWorldMatrix;

                // Blend Shapes
                Vector3[] deltaVertices = new Vector3[vertices.Count];
                for (var vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
                {
                    var maxDistanceAlongBone = _boneSettings.Length * 2f;
                    var maxHeightAboveBone = _boneSettings.Radius * 2f;

                    var displacementAlongBone = vertices[vertIndex].z - boneTransforms[boneIndex].localPosition.z;

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
            _skinnedMeshRenderer.bones = boneTransforms;

            #endregion

            for (int boneIndex = 0; boneIndex < _bones.Count; boneIndex++)
            {
                boneTransforms[boneIndex].position = transform.TransformPoint(_bones[boneIndex].position);
                boneTransforms[boneIndex].rotation = transform.rotation * _bones[boneIndex].rotation;
                SetBlendShapeWeight(boneIndex, _bones[boneIndex].weight);
            }
            // OnConstructBody?.Invoke();
        }

        public void AddBone(int index, Vector3 pos, Quaternion quat, float weight)
        {
            if (!CanAddBone()) return;
            //detach body parts here

            Transform bone = new GameObject("Bone." + _bones.Count).transform;
            bone.SetParent(_root, false);
            _bonesTransforms.Add(bone);
            //invoke on added bone event 
            _bones.Insert(index, new Bone(pos, quat, weight));
            ConstructBody();

            //construct body 

            //reattatch body parts 
        }

        public void RemoveBone(int index)
        {
            if (!CanAddBone()) return;

            //detach body parts

            //invoke event pre removebone

            Transform bone = _bonesTransforms[_bonesTransforms.Count - 1];
            _bonesTransforms.Remove(bone);
            DestroyImmediate(bone.gameObject);
            //invoke event  removebone
            _bones.RemoveAt(index);
            //reattatch body parts 
        }

        public void AddBoneToFront()
        {
            if (_bonesTransforms.Count > 0)
            {
                Vector3 position = _bonesTransforms[0].position - _bonesTransforms[0].forward * _boneSettings.Length;
                Quaternion rotation = _bonesTransforms[0].rotation;

                position = transform.InverseTransformPoint(position);
                rotation = Quaternion.Inverse(transform.rotation) * rotation;

                AddBone(0, position, rotation, Mathf.Clamp(GetBlendShapeWeight(0) * 0.75f, minMaxBoneBlendShapeWeights.min, minMaxBoneBlendShapeWeights.max));
            }
            else
                AddBone(0, _body.position, _body.rotation, 1);
        }

        public void AddBoneToBack()
        {
            if (_bonesTransforms.Count > 0)
            {
                Vector3 position = _bonesTransforms[_bonesTransforms.Count - 1].position +
                                   _bonesTransforms[_bonesTransforms.Count - 1].forward * _boneSettings.Length;
                Quaternion rotation = _bonesTransforms[_bonesTransforms.Count - 1].rotation;

                position = transform.InverseTransformPoint(position);

                rotation = Quaternion.Inverse(transform.rotation) * rotation;

                AddBone(_bonesTransforms.Count, position, rotation,
                    Mathf.Clamp(GetBlendShapeWeight(_bonesTransforms.Count - 1) * 0.75f,minMaxBoneBlendShapeWeights.min, minMaxBoneBlendShapeWeights.max));
            }
            else
                AddBone(0, _body.position, _body.rotation, 1);
        }

        private bool CanAddBone()
        {
            return !(_bonesTransforms.Count + 1 > minMaxBones.max);
        }

        private bool CanRemoveBone()
        {
            return !(_bonesTransforms.Count - 1 < minMaxBones.min); // && no limbs attatchd
        }


        public float GetBlendShapeWeight(int index)
        {
            return _skinnedMeshRenderer.GetBlendShapeWeight(index);
        }

        public void SetBlendShapeWeight(int index, float weight)
        {
            weight = Mathf.Clamp(weight, 0f, 100f);
            _bones[index].weight = weight;
            _skinnedMeshRenderer.SetBlendShapeWeight(index, weight);

            // UpdateOrigin();
            // OnSetWeight?.Invoke(index, weight);
        }

        public void AddBlendShapeWeight(int index, float weight, int dir = 0)
        {
            SetBlendShapeWeight(index, GetBlendShapeWeight(index) + weight);

            if (index > 0 && (dir == -1 || dir == 0))
            {
                AddBlendShapeWeight(index - 1, weight / 2f, -1);
            }

            if (index < _bones.Count - 1 && (dir == 1 || dir == 0))
            {
                AddBlendShapeWeight(index + 1, weight / 2f, 1);
            }

            // OnAddBlendShapeWeight?.Invoke(index, weight);
        }

        public void RemoveBlendShapeWeight(int index, float weight)
        {
            SetBlendShapeWeight(index, GetBlendShapeWeight(index) - weight);

            if (index > 0)
            {
                SetBlendShapeWeight(index - 1, GetBlendShapeWeight(index - 1) - weight / 2f);
            }

            if (index < _bones.Count - 1)
            {
                SetBlendShapeWeight(index + 1, GetBlendShapeWeight(index + 1) - weight / 2f);
            }

            // OnRemoveWeight?.Invoke(index, weight);
        }


        public void UpdateBoneConfiguration()
        {
            for (int boneIndex = 0; boneIndex < _bonesTransforms.Count; boneIndex++)
            {
                _bones[boneIndex].position = transform.InverseTransformPoint(_bonesTransforms[boneIndex].position);
                _bones[boneIndex].rotation =
                    Quaternion.Inverse(transform.rotation) * _bonesTransforms[boneIndex].rotation;
                _bones[boneIndex].weight = GetBlendShapeWeight(boneIndex);
            }
        }

        public void UpdateOrigin()
        {
            Vector3 sum = Vector3.zero;
            float totalWeights = 0;
            for (int i = 0; i < _bonesTransforms.Count; i++)
            {
                float weight = _bones[i].weight + 1; // can't be zero

                sum += _bonesTransforms[i].position * weight;
                totalWeights += weight;
            }

            Vector3 mean = sum / totalWeights;
            Vector3 displacement = mean - _body.position;
            foreach (Transform bone in _bonesTransforms)
            {
                bone.position -= displacement;
            }

            _body.position = mean;
        }
    }
}