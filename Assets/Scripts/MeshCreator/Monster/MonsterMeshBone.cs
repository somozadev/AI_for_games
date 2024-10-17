using UnityEngine;

namespace MonsterCreator
{
    public class MonsterMeshBone : MonoBehaviour
    {
        public Vector3 position;
        public Quaternion rotation;
        public float weight;

        public MonsterMeshBone(MonsterMeshBone bone)
        {
            position = bone.position;
            rotation = bone.rotation;
            weight = bone.weight;
        }

        public MonsterMeshBone(Vector3 pos, Quaternion rot, float w)
        {
            transform.localPosition = position = pos;
            transform.localRotation =  rotation = rot;
            weight = w;
        }

    }
}