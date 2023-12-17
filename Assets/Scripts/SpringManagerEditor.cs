#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// nice, it is always useful to have this kind of features to see what's is happening
// with a simple glance
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