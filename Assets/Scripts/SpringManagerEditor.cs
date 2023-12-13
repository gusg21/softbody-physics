#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FinalProject
{
    [CustomEditor(typeof(SpringManager))]
    public class SpringManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            SpringManager manager = target as SpringManager;
            if (manager == null) return;

            var springs = manager.GetSprings();
            
            GUILayout.Label("Springs: " + springs.Count);
        }
    }
}
#endif