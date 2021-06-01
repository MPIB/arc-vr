using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

// TODO: Documentation is very scarce for a reasonably complex component such as this.

namespace AVR.Core {
    /// <summary>
    /// Generic datalogger that logs any given variables or object properties in a csv-like format.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___logger.html")]
    public class AVR_Logger : AVR_Behaviour
    {
        public string filepath = "logs/sample.log";
        public string delimeter = ";";
        public enum deltaTypes {ON_UPDATE, ON_FIXEDUPDATE, ON_LATEUPDATE, CUSTOM}
        public deltaTypes deltaType;
        public float delta = 0.05f;
        private float stime = 0.0f;

        [System.Serializable]
        public class DataSource {
            public enum ValueTypes {CUSTOM, TIME};
            public enum ReadTypes {AUTO};

            public string label;
            public MonoBehaviour target;
            public string field;
            public ReadTypes readType;
            public ValueTypes valueType;
        }

        [SerializeField]
        public List<DataSource> columns = new List<DataSource>();

        private StreamWriter file;

        void Update() {
            if(deltaType==deltaTypes.ON_UPDATE) logObjects();
            else if(deltaType == deltaTypes.CUSTOM && Time.time >= stime + delta - 0.005f) { //0.005f is a small epsilon to account for small framerates
                logObjects();
                stime = Time.time;
            }
        }

        void FixedUpdate() {
            if (deltaType == deltaTypes.ON_FIXEDUPDATE) logObjects();
        }

        void LateUpdate() {
            if (deltaType == deltaTypes.ON_LATEUPDATE) logObjects();
        }

        void OnDestroy() {
            file.Close();
        }

        void Start() {
            init();
        }

        void logObjects() {
            string o = "";
            foreach(DataSource c in columns) {
                o += getData(c) + delimeter;
            }
            file.WriteLine(o);
        }

        void init() {
            string folder = new FileInfo(filepath).Directory.FullName;
            var dir = Directory.CreateDirectory(folder);
            file = File.CreateText(filepath);
            string header = "";
            foreach(DataSource c in columns) {
                header += c.label + delimeter;
            }
            file.WriteLine(header);
        }

        string getData(DataSource src) {
            switch(src.valueType) {
                case DataSource.ValueTypes.TIME : {
                    return Time.time.ToString();
                }
                default : {
                    var data = getPropertyValue(src.target, src.field);
                    if(data==null) return "null";
                    return data.ToString();
                }
            }
        }

        object getPropertyValue(object target, string field) {
            if (target == null || field==null) return null;

            if (field.Contains("."))//complex type nested
            {
                var temp = field.Split(new char[] { '.' }, 2);
                return getPropertyValue(getPropertyValue(target, temp[0]), temp[1]);
            }
            else
            {
                // Return field or proprty depending on which one it is
                // NOTE: use "GetProperty(field, System.Reflection.BindingFlags.NonPublic)" for non-public fields.
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default | BindingFlags.GetField | BindingFlags.GetProperty;
                try {
                    return target.GetType().GetProperty(field, flags).GetValue(target, null);
                }
                catch(System.Exception) {
                    return target.GetType().GetField(field, flags).GetValue(target);
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AVR_Logger.DataSource))]
    public class LookAtPointEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(property.FindPropertyRelative("valueType").intValue == (int)AVR_Logger.DataSource.ValueTypes.CUSTOM) {
                return 100; // We need 2 lines for custom ones
            }
            return 30;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(new Rect(position.x+0, position.y, 200, 20), property.FindPropertyRelative("label"), new GUIContent("Label:", "Header given to the value in the logged table"));
            EditorGUI.PropertyField(new Rect(position.x+200, position.y, 140, 20), property.FindPropertyRelative("valueType"), new GUIContent("Type:", "Type of value logged."));

            if(property.FindPropertyRelative("valueType").intValue == (int)AVR_Logger.DataSource.ValueTypes.CUSTOM) {
                EditorGUIUtility.labelWidth = 80;
                EditorGUI.PropertyField(new Rect(position.x + 20, position.y+20, 320, 20), property.FindPropertyRelative("target"), new GUIContent("Target:", "Target Monobehavior."));
                EditorGUI.PropertyField(new Rect(position.x + 20, position.y+40, 320, 20), property.FindPropertyRelative("field"), new GUIContent("Field:", "Field or Property of the target that will be logged."));
                EditorGUI.PropertyField(new Rect(position.x + 20, position.y+60, 320, 20), property.FindPropertyRelative("readType"), new GUIContent("ReadType:", "How we log the data extracted from the field."));
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}
