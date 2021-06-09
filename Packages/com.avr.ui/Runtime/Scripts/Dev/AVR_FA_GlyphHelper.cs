using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace AVR.UI.Utils {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    public class AVR_FA_GlyphHelper : MonoBehaviour
    {
        void Awake() {
            if(!Application.isEditor) {
                AVR.Core.AVR_DevConsole.cwarn("There is a redundant GlyphHelper object at: "+AVR.Core.Utils.Misc.GetHierarchyPath(transform)+". Consider removing it.", this);
                GameObject.Destroy(this);
            }
        }

        public void setup(Font font, int unicode) {
            Text t = GetComponent<Text>();
            t.font = font;
            t.text = ((char)unicode).ToString();

            #if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
            #endif
        }
    }
}
