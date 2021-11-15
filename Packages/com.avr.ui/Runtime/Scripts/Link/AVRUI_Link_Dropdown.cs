using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using AVR.Core;
namespace AVR.UI.Link {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Dropdown))]
    public class AVRUI_Link_Dropdown : AVRUI_Link
    {
        public override List<System.Type> validTypes => new List<System.Type> {typeof(System.Enum)};

        private Dropdown input;

        protected virtual UnityEngine.Events.UnityAction<int> updateValueListener => delegate { this.updateValue(); };

        public override void setup() {
            init();

            if(!target) return;

            System.Type field_type = null;
            if(memberType==MemberType.FIELD) field_type = target.GetType().GetField(field).FieldType;
            else field_type = target.GetType().GetProperty(field).PropertyType;

            object current_value = target.GetType().GetField(field).GetValue(target);
            string current_name = System.Enum.GetName(field_type, current_value);

            string[] names = System.Enum.GetNames(field_type);

            input.options.Clear();
            for (int i = 0; i < names.Length; i++)
            {
                input.options.Add(new Dropdown.OptionData(names[i]));
                if (names[i] == current_name) input.value = i;
            }

            updateOutput();

            // Make sure we have the adequate persisten listeners by (possibly) removing and re-adding
            #if UNITY_EDITOR
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(input.onValueChanged, updateValueListener);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(input.onValueChanged, updateValueListener);
            #endif

        }

        public override void init() {
            if(!input) input = GetComponent<Dropdown>();

            if(input) old_value = input.value.ToString();
        }

        public override void updateValue()
        {
            // Get the value currently displayed
            string text = input.options[input.value].text;

            // Update it to the target, if possible.
            try {
                switch (memberType)
                {
                    case MemberType.FIELD:
                        {
                            int numval = (int)System.Enum.Parse(target.GetType().GetField(field).FieldType, text);
                            target.GetType().GetField(field).SetValue(target, numval);
                            break;
                        }
                    case MemberType.PROPERTY:
                        {
                            int numval = (int)System.Enum.Parse(target.GetType().GetProperty(field).PropertyType, text);
                            target.GetType().GetProperty(field).SetValue(target, numval);
                            break;
                        }
                }
            }
            catch(System.Exception) {
                AVR_DevConsole.cwarn("Could not update the given target value to "+text, this);
            }
        }

        private string old_value;

        public override void updateOutput()
        {
            // Do nothing if the user is still typing the value
            int current_value = 0;

            switch (memberType)
            {
                case MemberType.FIELD:
                    {
                        current_value = (int)target.GetType().GetField(field).GetValue(target);
                        break;
                    }
                case MemberType.PROPERTY:
                    {
                        current_value = (int)target.GetType().GetProperty(field).GetValue(target);
                        break;
                    }
            }

            input.value = current_value;

#if UNITY_EDITOR
            if (old_value != current_value.ToString() && !Application.isPlaying)
            {
                EditorUtility.SetDirty(input);
                old_value = current_value.ToString();
            }
#endif
        }
    }
}