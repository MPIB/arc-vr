using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using AVR.Core;

namespace AVR.UI.Link {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Slider))]
    public class AVRUI_Link_Slider : AVRUI_Link
    {
        public override List<System.Type> validTypes => new List<System.Type> {typeof(float), typeof(int), typeof(double)};

        private Slider input;

        protected virtual UnityEngine.Events.UnityAction<float> updateValueListener => delegate { this.updateValue(); };

        public override void setup() {
            init();

            updateOutput();

            // Make sure we have the adequate persisten listeners by (possibly) removing and re-adding
            #if UNITY_EDITOR
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(input.onValueChanged, updateValueListener);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(input.onValueChanged, updateValueListener);
            #endif

        }

        public override void init() {
            if(!input) input = GetComponent<Slider>();
        }

        public override void updateValue()
        {
            // Get the value currently displayed
            float val = input.value;

            // Update it to the target, if possible.
            try {
                switch (memberType)
                {
                    case MemberType.FIELD:
                        {
                            target.GetType().GetField(field).SetValue(target,
                                System.Convert.ChangeType(val, target.GetType().GetField(field).FieldType)
                            );
                            break;
                        }
                    case MemberType.PROPERTY:
                        {
                            target.GetType().GetProperty(field).SetValue(target,
                                System.Convert.ChangeType(val, target.GetType().GetProperty(field).PropertyType)
                            );
                            break;
                        }
                }
            }
            catch(System.Exception) {
                AVR_DevConsole.cwarn("Could not update the given target value to "+val, this);
            }
        }

        private string old_value;

        public override void updateOutput()
        {
            float current_value = 0f;

            switch (memberType)
            {
                case MemberType.FIELD:
                    {
                        current_value = (float)target.GetType().GetField(field).GetValue(target);
                        break;
                    }
                case MemberType.PROPERTY:
                    {
                        current_value = (float)target.GetType().GetProperty(field).GetValue(target);
                        break;
                    }
            }

            input.value = current_value;

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
