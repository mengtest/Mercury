#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Mercury
{
    public class MultiEnumAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(MultiEnumAttribute))]
    public class MultiEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }
}
#endif