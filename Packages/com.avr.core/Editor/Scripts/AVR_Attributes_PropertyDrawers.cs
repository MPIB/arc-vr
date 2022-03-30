using UnityEditor;
using UnityEngine;
using ct = AVR.Core.Attributes.ConditionalHideInInspector.compareType;

namespace AVR.Core.Attributes {
    [CustomPropertyDrawer(typeof(ReadOnly))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

    [CustomPropertyDrawer(typeof(FoldoutGroup))]
    public class FoldoutPropertyDrawer : PropertyDrawer {
        private static bool last_foldout = false;

#nullable enable
        // Here we store the group of the last drawn FoldoutProperty
        private static string? last_group = null;

        // Here we store a string-based reference for some property. We use this as a cycle-reference to determine when to reset last_group
        private static string? prop_reference = null;
#nullable disable

        // Here we store a reference to the last object for which we drew a FoldoutProperty. We use this to determine when to reset prop_reference
        private static Object obj_reference = null;

        private bool foldout = false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        private void NewGroup(string group_name, SerializedProperty prop) {
            last_group = group_name;

            // Begin foldout
            foldout = EditorGUILayout.Foldout(foldout, group_name);
            EditorGUI.indentLevel++;
            if (foldout) EditorGUILayout.PropertyField(prop, new GUIContent(prop.displayName));
            EditorGUI.indentLevel--;

            last_foldout = foldout;
        }

        private void ContinuationGroup(SerializedProperty prop) {
            if (last_foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(prop, new GUIContent(prop.displayName));
                EditorGUI.indentLevel--;
            }
            foldout = last_foldout;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // TL;DR: If the reference object is changed (by for instance selecting a different object in the inspector, or when drawing multpile objects with foldout-groups)
            // then we reset the prop-reference and last-group, as not to interfere with the foreign object.
            if(obj_reference==null) obj_reference = property.serializedObject.targetObject;
            else if(obj_reference!=property.serializedObject.targetObject) {
                prop_reference = null;
                last_group = null;
                obj_reference = property.serializedObject.targetObject;
            }

            // TL;DR: prop_reference acts as a cycle reference. if prop_reference==property.name, that means we are drawing a property we have already drawn previously
            // meaning, we are in a brand new OnGUI cycle. Respectively, we need to reset the last_group variable.
            if(prop_reference==null) prop_reference = property.name;
            else if(prop_reference==property.name) {
                last_group = null;
            }

            // Get attribute
            FoldoutGroup attr = attribute as FoldoutGroup;

            if(last_group!=attr.group_id) {
                NewGroup(attr.group_id, property);
            } 
            else {
                ContinuationGroup(property);
            }
        }
    }

    [CustomPropertyDrawer(typeof(ConditionalHideInInspector))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        // Reference to the attribute on the property.
        ConditionalHideInInspector attr;

        // Field that is being compared.
        SerializedProperty hideConditionProperty;

        // Height of the property.
        private float propertyHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return propertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get attribute
            ConditionalHideInInspector attr = attribute as ConditionalHideInInspector;

            // Get property that represents the hide condition
            SerializedProperty prop = property.serializedObject.FindProperty(attr.hideConditionPropertyName);

            // Get its value
            bool val = false;
            
            if(prop.type=="bool") {
                val = prop.boolValue;
            }
            else if(prop.type=="float") {
                val =
                    attr.ctype == ct.BIGGEREQUAL && prop.floatValue >= attr.compareValue ||
                    attr.ctype == ct.BIGGER && prop.floatValue > attr.compareValue ||
                    attr.ctype == ct.SMALLEREQUAL && prop.floatValue <= attr.compareValue ||
                    attr.ctype == ct.SMALLER && prop.floatValue < attr.compareValue ||
                    attr.ctype == ct.EQUAL && prop.floatValue == attr.compareValue ||
                    attr.ctype == ct.UNEQUAL && prop.floatValue != attr.compareValue;
            }
            else if (prop.type == "int") {
                val =
                    attr.ctype == ct.BIGGEREQUAL && prop.intValue >= attr.compareValue ||
                    attr.ctype == ct.BIGGER && prop.intValue > attr.compareValue ||
                    attr.ctype == ct.SMALLEREQUAL && prop.intValue <= attr.compareValue ||
                    attr.ctype == ct.SMALLER && prop.intValue < attr.compareValue ||
                    attr.ctype == ct.EQUAL && prop.intValue == attr.compareValue ||
                    attr.ctype == ct.UNEQUAL && prop.intValue != attr.compareValue;
            }
            else if (prop.type == "double") {
                val =
                    attr.ctype == ct.BIGGEREQUAL && prop.doubleValue >= attr.compareValue ||
                    attr.ctype == ct.BIGGER && prop.doubleValue > attr.compareValue ||
                    attr.ctype == ct.SMALLEREQUAL && prop.doubleValue <= attr.compareValue ||
                    attr.ctype == ct.SMALLER && prop.doubleValue < attr.compareValue ||
                    attr.ctype == ct.EQUAL && prop.doubleValue == attr.compareValue ||
                    attr.ctype == ct.UNEQUAL && prop.doubleValue != attr.compareValue;
            }
            else if (prop.type == "long") {
                val =
                    attr.ctype == ct.BIGGEREQUAL && prop.longValue >= attr.compareValue ||
                    attr.ctype == ct.BIGGER && prop.longValue > attr.compareValue ||
                    attr.ctype == ct.SMALLEREQUAL && prop.longValue <= attr.compareValue ||
                    attr.ctype == ct.SMALLER && prop.longValue < attr.compareValue ||
                    attr.ctype == ct.EQUAL && prop.longValue == attr.compareValue ||
                    attr.ctype == ct.UNEQUAL && prop.longValue != attr.compareValue;
            }
            else if (prop.type == "Enum") {
                val =
                    attr.ctype == ct.BIGGEREQUAL && prop.enumValueIndex >= attr.compareValue ||
                    attr.ctype == ct.BIGGER && prop.enumValueIndex > attr.compareValue ||
                    attr.ctype == ct.SMALLEREQUAL && prop.enumValueIndex <= attr.compareValue ||
                    attr.ctype == ct.SMALLER && prop.enumValueIndex < attr.compareValue ||
                    attr.ctype == ct.EQUAL && prop.enumValueIndex == attr.compareValue ||
                    attr.ctype == ct.UNEQUAL && prop.enumValueIndex != attr.compareValue;
            }
            else {
                val = prop.objectReferenceValue != null;
            }

            val ^= attr.invertCondition;
            

            if (val)
            {
                // Dont draw
                propertyHeight = 0f;
            }
            else
            {
                // draw
                propertyHeight = base.GetPropertyHeight(property, label);
                EditorGUI.PropertyField(position, property, new GUIContent(property.displayName), true);
            }
        }
    }
}
