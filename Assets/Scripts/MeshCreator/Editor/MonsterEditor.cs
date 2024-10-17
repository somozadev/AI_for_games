using UnityEditor;
using UnityEngine;

namespace MeshCreator.Editor
{
    [CustomEditor(typeof(Monster))]
    public class MonsterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Monster monsterComponent = (Monster)target;
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