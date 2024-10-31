using UnityEngine;

namespace MeshCreator
{
    [System.Serializable]
    public class Bone
    {
        public Vector3 position;
        public Quaternion rotation;
        public float weight;

        public Bone()
        {
        }

        public Bone(Bone bone)
        {
            position = bone.position;
            rotation = bone.rotation;
            weight = bone.weight;
        }


        public Bone(Vector3 pos, Quaternion rot, float w)
        {
            position = pos;
            rotation = rot;
            weight = w;
        }


    }
}