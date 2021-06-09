using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.UI.Utils;
using AVR.UEditor.Core;

namespace AVR.UEditor.UI {
    [CustomEditor(typeof(AVR_FA_GlyphHelper))]
    public class AVR_FA_GlyphHelper_Editor : Editor
    {
        private enum FA_type
        {
            SOLID, BRAND, REGULAR
        }

        FA_type ShowType = FA_type.SOLID;

        string unicode_input;

        List<int> regular_unicodes = new List<int>();
        List<int> solid_unicodes = new List<int>();
        List<int> brand_unicodes = new List<int>();

        Font regular_static;
        Font regular_dynamic;
        Font regular_runtime;

        Font brand_static;
        Font brand_dynamic;
        Font brand_runtime;

        Font solid_static;
        Font solid_dynamic;
        Font solid_runtime;

        GUIStyle button_style;

        Vector2 scrollPos = Vector2.zero;

        bool show_glyphs = false;

        void OnEnable()
        {
            solid_static = AVR_EditorUtility.GetFont("/editor/editorfonts/font-awesome-solid-static");
            solid_dynamic = AVR_EditorUtility.GetFont("/editor/editorfonts/font-awesome-solid");
            solid_runtime = AVR_EditorUtility.GetFont("/editor/fonts/font-awesome-solid");

            regular_static = AVR_EditorUtility.GetFont("/editor/editorfonts/font-awesome-regular-static");
            regular_dynamic = AVR_EditorUtility.GetFont("/editor/editorfonts/font-awesome-regular");
            regular_runtime = AVR_EditorUtility.GetFont("/editor/fonts/font-awesome-regular");

            brand_static = AVR_EditorUtility.GetFont("/editor/editorfonts/font-awesome-brands-static");
            brand_dynamic = AVR_EditorUtility.GetFont("/editor/editorfonts/font-awesome-brands");
            brand_runtime = AVR_EditorUtility.GetFont("/editor/fonts/font-awesome-brands");

            solid_unicodes = new List<int>();
            foreach (var c in solid_static.characterInfo)
            {
                solid_unicodes.Add(c.index);
            }

            regular_unicodes = new List<int>();
            foreach (var c in regular_static.characterInfo)
            {
                regular_unicodes.Add(c.index);
            }

            brand_unicodes = new List<int>();
            foreach (var c in brand_static.characterInfo)
            {
                brand_unicodes.Add(c.index);
            }
        }

        private void FromUnicode(Font UseFont) {
            if (GUILayout.Button("Search Glyphs", GUILayout.Height(30)))
            {
                Application.OpenURL("https://fontawesome.com/v5.15/icons?d=gallery&p=2&s=brands,regular,solid&m=free");
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Hex. Unicode:");
                unicode_input = GUILayout.TextField(unicode_input);
                if(GUILayout.Button("Set", GUILayout.Width(60))) {
                    ((AVR_FA_GlyphHelper)target).setup(UseFont, int.Parse(unicode_input, System.Globalization.NumberStyles.HexNumber));
                }
            }
        }

        private void Glyphs(List<int> unicodes, Font DisplayFont, Font UseFont)
        {
            const float button_size = 30;
            const int fontSize = 16;

            float max_width = Screen.width - button_size;

            float width_acc = 0.0f;

            button_style = new GUIStyle(GUI.skin.button);
            button_style.margin = new RectOffset(0, 0, 0, 0);
            button_style.fontSize = fontSize;
            button_style.font = DisplayFont;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(500), GUILayout.Width(max_width));
            GUILayout.BeginHorizontal();
            for (int i = 0; i < unicodes.Count; i++)
            {
                if (width_acc >= max_width)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    width_acc = 0.0f;
                }

                int u = unicodes[i];

                if(GUILayout.Button(AVR_EditorUtility.Unicode_to_String(u), button_style, GUILayout.Width(button_size), GUILayout.Height(button_size))) {
                    ((AVR_FA_GlyphHelper)target).setup(UseFont, u);
                    show_glyphs = false;
                }

                width_acc += button_size + 2; //+2 because inherently there is 1 pixel of padding on each side.
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (!show_glyphs && GUILayout.Button("Show Glyphs", GUILayout.Height(60)))
                {
                    show_glyphs = true;
                }

                if (show_glyphs && GUILayout.Button("Hide Glyphs", GUILayout.Height(60)))
                {
                    show_glyphs = false;
                }

                if (GUILayout.Button("Remove Helper", GUILayout.Height(60), GUILayout.Width(120)))
                {
                    DestroyImmediate(target);
                }
            }

            EditorGUILayout.Space();

            if (show_glyphs)
            {
                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Solid Glyphs", GUILayout.Height(30))) ShowType = FA_type.SOLID;
                    if (GUILayout.Button("Regular Glyphs", GUILayout.Height(30))) ShowType = FA_type.REGULAR;
                    if (GUILayout.Button("Brand Glyphs", GUILayout.Height(30))) ShowType = FA_type.BRAND;
                }

                switch(ShowType) {
                    case FA_type.SOLID:
                        {
                            EditorGUILayout.Space();
                            FromUnicode(solid_runtime);
                            EditorGUILayout.Space();
                            Glyphs(solid_unicodes, solid_dynamic, solid_runtime);
                            break;
                        }
                    case FA_type.REGULAR:
                        {
                            EditorGUILayout.Space();
                            FromUnicode(regular_runtime);
                            EditorGUILayout.Space();
                            Glyphs(regular_unicodes, regular_dynamic, regular_runtime);
                            break;
                        }
                    case FA_type.BRAND:
                        {
                            EditorGUILayout.Space();
                            FromUnicode(brand_runtime);
                            EditorGUILayout.Space();
                            Glyphs(brand_unicodes, brand_dynamic, brand_runtime);
                            break;
                        }
                }
            }
        }
    }
}
