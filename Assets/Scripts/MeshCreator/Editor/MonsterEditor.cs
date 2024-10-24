using UnityEditor;
using UnityEngine;

namespace MeshCreator.Editor
{
    [CustomEditor(typeof(MonsterBody))]
    public class MonsterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MonsterBody monsterComponent = (MonsterBody)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Add Bone Front"))
            {
                monsterComponent.AddBoneToFront();
            }

            if (GUILayout.Button("Add Bone Back"))
            {
                monsterComponent.AddBoneToBack();
            }
        }
    }
}