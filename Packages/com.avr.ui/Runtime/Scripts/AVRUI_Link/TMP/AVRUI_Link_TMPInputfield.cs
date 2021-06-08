using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;

namespace AVR.UI.Link {
    [ExecuteInEditMode]
    [RequireComponent(typeof(TMPro.TMP_InputField))]
    public class AVRUI_Link_TMPInputfield : AVRUI_Link
    {
        public override List<System.Type> validTypes => new List<System.Type> {typeof(bool), typeof(float), typeof(int), typeof(double), typeof(string)};

        private TMPro.TMP_InputField input;

        protected virtual UnityEngine.Events.UnityAction<string> updateValueListener => delegate { this.updateValue(); };

        public override void setup() {
            init();

            updateOutput();

            // Make sure we have the adequate persisten listeners by (possibly) removing and re-adding
            #if UNITY_EDITOR
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(input.onEndEdit, updateValueListener);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(input.onEndEdit, updateValueListener);

            UnityEditor.Events.UnityEventTools.RemovePersistentListener(input.onValueChanged, updateValueListener);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(input.onValueChanged, updateValueListener);
            #endif

        }

        public override void init() {
            if(!input) input = GetComponent<TMPro.TMP_InputField>();
        }

        public override void updateValue()
        {
            // Do nothing if the user is still typing the value
            if (input.isFocused) return;

            // Get the value currently displayed in the inputfield
            string text = input.text;

            // Update it to the target, if possible.
            try {
                switch (memberType)
                {
                    case MemberType.FIELD:
                        {
                            target.GetType().GetField(field).SetValue(target,
                                System.Convert.ChangeType(text, target.GetType().GetField(field).FieldType)
                            );
                            break;
                        }
                    case MemberType.PROPERTY:
                        {
                            target.GetType().GetProperty(field).SetValue(target,
                                System.Convert.ChangeType(text, target.GetType().GetProperty(field).PropertyType)
                            );
                            break;
                        }
                }
            }
            catch(System.Exception) {
                AVR_DevConsole.cwarn("Could not update the given target value to "+text, this);
            }
        }

        private string old_value = "";

        public override void updateOutput()
        {
            // Do nothing if the user is still typing the value
            if(input.isFocused) return;

            object current_value = null;

            switch (memberType)
            {
                case MemberType.FIELD:
                    {
                        current_value = target.GetType().GetField(field).GetValue(target);
                        break;
                    }
                case MemberType.PROPERTY:
                    {
                        current_value = target.GetType().GetProperty(field).GetValue(target);
                        break;
                    }
            }

            input.text = current_value.ToString();

#if UNITY_EDITOR
            if (old_value != current_value.ToString())
            {
                EditorUtility.SetDirty(input);
                old_value = current_value.ToString();
            }
#endif
        }
    }
}
