using UnityEditor;
using UnityEngine;

namespace SortingAlgorithms.Editor
{
  [CustomEditor(typeof(Numbers))]
  public class NumbersEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      Numbers numbers = (Numbers)target;

      GUI.enabled = false;
      EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject(numbers), typeof(Numbers), false);
      GUI.enabled = true;

      serializedObject.Update();
      EditorGUILayout.LabelField("Properties for automatic initialization with button below.", EditorStyles.boldLabel);
      numbers.NumValues = EditorGUILayout.IntField("NumValues", numbers.NumValues);
      numbers.MinValue = EditorGUILayout.IntField("MinValue", numbers.MinValue);
      numbers.MaxValue = EditorGUILayout.IntField("MaxValue", numbers.MaxValue);
      numbers.Randomize = EditorGUILayout.Toggle("Randomize", numbers.Randomize);

      if (GUILayout.Button("Initialize"))
      {
        numbers.Init(numbers.NumValues, numbers.MinValue, numbers.MaxValue, numbers.Randomize);
        EditorUtility.SetDirty(numbers);
      }

      EditorGUILayout.PropertyField(serializedObject.FindProperty("Values"), true);
      serializedObject.ApplyModifiedProperties();
    }
  }
}
