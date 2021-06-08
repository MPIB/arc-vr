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
                EditorGUI.PropertyField(position, property);
            }
        }
    }
}