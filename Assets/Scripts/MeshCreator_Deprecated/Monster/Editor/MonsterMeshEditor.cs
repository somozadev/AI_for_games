using UnityEditor;
using UnityEngine;

namespace MonsterCreator
{
    [CustomEditor(typeof(MonsterMesh))]
    public class MonsterMeshEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MonsterMesh monsterMesh = (MonsterMesh)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Add Bone Front"))
            {
                monsterMesh.AddBoneToFront();
            }

            if (GUILayout.Button("Add Bone Back"))
            {
                monsterMesh.AddBoneToBack();
            }
        }
    }
}