using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace AVR.UEditor.Core {
    public class AVR_ArcVRWindow : EditorWindow
    {
        GUIStyle _titleStyle;
        GUIStyle _labelStyle;

        Texture2D logo;

        bool initialized = false;

        void init() {
            _titleStyle = new GUIStyle(GUI.skin.label);
            _titleStyle.fontStyle = FontStyle.Bold;

            _labelStyle = new GUIStyle(GUI.skin.label);

            logo = AVR.Core.Utils.Misc.Image2Texture("Packages/com.avr.core/Package_Resources/avr_logo_vr.png");

            initialized = true;
        }

        void OnGUI() {
            if(!initialized) init();

            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                float logo_width = 250;
                GUILayout.Box(logo, GUILayout.Width(logo_width), GUILayout.Height(logo.height / (logo.width/250)));
                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (AVR_EditorUtility.FAButton("f02d", buttonSize:35))
                {
                    string path = System.IO.Path.GetFullPath(AVR.Core.AVR_Settings.get_string("/editor/documentationPath")+"index.html");
                    Application.OpenURL("file:///" + path);
                }

                if (AVR_EditorUtility.FAButton("f09b", isBrandIcon:true, buttonSize:35))
                {
                    Application.OpenURL("https://github.com/MPIB/arc-vr");
                }

                if (AVR_EditorUtility.FAButton("f013", buttonSize: 35))
                {
                    AVR_Settings_Editor.ShowWindow();
                }

                if (AVR_EditorUtility.FAButton("f120", buttonSize: 35))
                {
                    AVR_DevConsoleWindow.ShowWindow();
                }

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label("Installed packages:", _titleStyle);
        #if AVR_CORE
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.green; AVR_EditorUtility.FALabel("f00c"); GUILayout.Label("arc-vr-core", _labelStyle); }
        #else
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.grey; AVR_EditorUtility.FALabel("f00d"); GUILayout.Label("arc-vr-core", _labelStyle); }
        #endif
        #if AVR_AVATAR
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.green; AVR_EditorUtility.FALabel("f00c"); GUILayout.Label("arc-vr-avatar", _labelStyle); }
        #else
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.grey; AVR_EditorUtility.FALabel("f00d"); GUILayout.Label("arc-vr-avatar", _labelStyle); }
        #endif
        #if AVR_MOTION
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.green; AVR_EditorUtility.FALabel("f00c"); GUILayout.Label("arc-vr-motion", _labelStyle); }
        #else
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.grey; AVR_EditorUtility.FALabel("f00d"); GUILayout.Label("arc-vr-motion", _labelStyle); }
        #endif
        #if AVR_PHYS
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.green; AVR_EditorUtility.FALabel("f00c"); GUILayout.Label("arc-vr-phys", _labelStyle); }
        #else
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.grey; AVR_EditorUtility.FALabel("f00d"); GUILayout.Label("arc-vr-phys", _labelStyle); }
        #endif
        #if AVR_UI
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.green; AVR_EditorUtility.FALabel("f00c"); GUILayout.Label("arc-vr-ui", _labelStyle); }
        #else
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.grey; AVR_EditorUtility.FALabel("f00d"); GUILayout.Label("arc-vr-ui", _labelStyle); }
        #endif
        #if AVR_NET
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.green; AVR_EditorUtility.FALabel("f00c"); GUILayout.Label("arc-vr-net", _labelStyle); }
        #else
                    using (new EditorGUILayout.HorizontalScope()) { GUI.contentColor=Color.grey; AVR_EditorUtility.FALabel("f00d"); GUILayout.Label("arc-vr-net", _labelStyle); }
    #endif
                }
                GUI.contentColor = Color.white;
                GUILayout.FlexibleSpace();
            }
        }
    }
}
