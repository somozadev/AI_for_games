using UnityEngine;

namespace MonsterCreator
{
    public class MonsterMeshBone : MonoBehaviour
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public float weight;

        public MonsterMeshBone(MonsterMeshBone bone)
        {
            position = bone.position;
            rotation = bone.rotation;
            weight = bone.weight;
            scale = bone.scale;
        }

        public MonsterMeshBone(Vector3 pos, Quaternion rot, Vector3 scal, float w)
        {
            transform.localPosition = position = pos;
            transform.localRotation =  rotation = rot;
            transform.localScale =  scale = scal;
            weight = w;
        }

    }
}