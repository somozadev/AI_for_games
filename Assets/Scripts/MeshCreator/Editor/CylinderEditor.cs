using UnityEditor;
using UnityEngine;
namespace MeshCreator.Editor
{

    [CustomEditor(typeof(Cylinder))]
    public class CylinderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Cylinder cylinderComponent = (Cylinder)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Generate Mesh"))
            {
                cylinderComponent.Generate();
            }
        }
    }

}