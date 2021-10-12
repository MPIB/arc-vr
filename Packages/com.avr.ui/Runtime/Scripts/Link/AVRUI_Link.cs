using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

/// <summary>
/// Namespace of the arc-vr-ui Linking elements
/// </summary>
namespace AVR.UI.Link {
    /// <summary>
    /// Binds a given UI element (such as slider, toggle, dropdown or inputfield) with a given property or field of a given MonoBehaviour.
    /// One intended use case would be an ingame options-screen to set, for instance, the FOV of a camera.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_u_i_1_1_link_1_1_a_v_r_u_i___link.html")]
    public abstract class AVRUI_Link : AVR.Core.AVR_Behaviour
    {
        public enum MemberType { FIELD, PROPERTY }

        public abstract List<System.Type> validTypes { get; }

        public MonoBehaviour target;

        [AVR.Core.Attributes.ReadOnly]
        public string field;

        [AVR.Core.Attributes.ReadOnly]
        public MemberType memberType;

        void Start() {
            init();
        }

        void OnGUI() {
            if(target && field!="") updateOutput();
        }

        /// <summary>
        /// Method is run on program start
        /// </summary>
        public abstract void init();

        /// <summary>
        /// Method is run when the user selects a field in the inspector.
        /// </summary>
        public abstract void setup();

        /// <summary>
        /// This method is executed when the runtime user changes the value of the UI element.
        /// This method should change the field on the actual target respectively.
        /// </summary>
        public abstract void updateValue();

        /// <summary>
        /// Method is run continously from OnGUI.
        /// This method should update the value displayed by the UI when the target value changes.
        /// </summary>
        public abstract void updateOutput();
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(AVRUI_Link), true)]
    class AVRUI_Link_Editor : Editor
    {
        private bool show_fields = false;

        public override void OnInspectorGUI()
        {
            AVRUI_Link l = (AVRUI_Link)target;

            DrawDefaultInspector();

            EditorGUILayout.Space();

            // Allow to toggle fields
            if (!show_fields)
            {
                if (GUILayout.Button("Show available fields", GUILayout.MaxWidth(200))) show_fields = true;
            }
            else if (GUILayout.Button("Hide available fields", GUILayout.MaxWidth(200)))
            {
                show_fields = false;
            }

            // Show available fields
            if (show_fields && l.target!=null)
            {

                GUIStyle bttnStyle = new GUIStyle(GUI.skin.button);
                bttnStyle.margin = new RectOffset(0, 0, 0, 0);

                BindingFlags commonFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.GetField | BindingFlags.GetProperty;

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginFoldoutHeaderGroup(true, "Public Properties");
                    foreach (var v in l.target.GetType().GetProperties(System.Reflection.BindingFlags.Public | commonFlags))
                    {
                        //l.validTypes.Any((t) => v.PropertyType.IsSubclassOf(t))
                        if (l.validTypes.Contains(v.PropertyType) || l.validTypes.Any((t) => v.PropertyType.IsSubclassOf(t)) && v.CanRead && v.CanWrite)
                            if (GUILayout.Button(v.Name, bttnStyle, GUILayout.MaxWidth(200)))
                            {
                                l.memberType = AVRUI_Link.MemberType.PROPERTY;
                                l.field = v.Name;
                                l.setup();
                            }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginFoldoutHeaderGroup(true, "Private Properties");
                    foreach (var v in l.target.GetType().GetProperties(System.Reflection.BindingFlags.NonPublic | commonFlags))
                    {
                        if (l.validTypes.Contains(v.PropertyType) || l.validTypes.Any((t) => v.PropertyType.IsSubclassOf(t)) && v.CanRead && v.CanWrite)
                            if (GUILayout.Button(v.Name, bttnStyle, GUILayout.MaxWidth(200)))
                            {
                                l.memberType = AVRUI_Link.MemberType.PROPERTY;
                                l.field = v.Name;
                                l.setup();
                            }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginFoldoutHeaderGroup(true, "Public Fields");
                    foreach (var v in l.target.GetType().GetFields(System.Reflection.BindingFlags.Public | commonFlags))
                    {
                        if (l.validTypes.Contains(v.FieldType) || l.validTypes.Any((t) => v.FieldType.IsSubclassOf(t)))
                            if (GUILayout.Button(v.Name, bttnStyle, GUILayout.MaxWidth(200)))
                            {
                                l.memberType = AVRUI_Link.MemberType.FIELD;
                                l.field = v.Name;
                                l.setup();
                            }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginFoldoutHeaderGroup(true, "Private Fields");
                    foreach (var v in l.target.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | commonFlags))
                    {
                        if (l.validTypes.Contains(v.FieldType) || l.validTypes.Any((t) => v.FieldType.IsSubclassOf(t)))
                            if (GUILayout.Button(v.Name, bttnStyle, GUILayout.MaxWidth(200)))
                            {
                                l.memberType = AVRUI_Link.MemberType.FIELD;
                                l.field = v.Name;
                                l.setup();
                            }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private System.Boolean CanCovert(System.String value, System.Type type)
        {
            System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
            return converter.IsValid(value);
        }
    }
#endif
}
