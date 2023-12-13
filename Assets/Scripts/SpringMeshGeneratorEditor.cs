#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FinalProject
{
    [CustomEditor(typeof(SpringMeshGenerator))]
    public class SpringMeshGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SpringMeshGenerator mesh = target as SpringMeshGenerator;
            if (mesh == null) return;

            //     if (Application.isPlaying)
            //     {
            //         if (GUILayout.Button("Re-apply Spring Settings"))
            //         {
            //             mesh.ReapplySpringSettings();
            //         }
            //         
            //         GUILayout.Label("Attachments: " + mesh.GetAttachmentCount());
            //     }
            // }
        }
    }
}
#endif