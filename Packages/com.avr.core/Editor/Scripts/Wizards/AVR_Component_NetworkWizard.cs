#if AVR_NET

using UnityEditor;
using UnityEngine;

using AVR.Core;
using System.Linq;
using System.Reflection;

namespace AVR.UEditor.Core {
    public class AVR_Component_NetworkWizard : ScriptableWizard
    {
        public AVR_Component component;
        public bool DestroyOnRemote = true;
        public bool ChangeLayerOnRemote = false;
        public bool ChangeLayerOnRemote_IncludeChildren = false;
        public int RemoteLayer = 0;
        public UnityEngine.Events.UnityEvent onRemoteStart;

        private SerializedObject serializedObject;

        public static void CreateWizard(AVR_Component component)
        {
            AVR_Component_NetworkWizard wiz = ScriptableWizard.DisplayWizard<AVR_Component_NetworkWizard>("AVR_Component Network Settings", "Apply");
            wiz.component = component;
            wiz.serializedObject = new SerializedObject(component);

            wiz.DestroyOnRemote = component.destroyOnRemote;
            wiz.ChangeLayerOnRemote = component.changeLayerOnRemote;
            wiz.ChangeLayerOnRemote_IncludeChildren = component.changeLayerOnRemote_IncludeChildren;
            wiz.RemoteLayer = component.remoteLayer;
            wiz.onRemoteStart = component.onRemoteStart;
        }

        // Called when Apply-Button is pressed
        void OnWizardCreate()
        {
            component.destroyOnRemote = DestroyOnRemote;
            component.changeLayerOnRemote = ChangeLayerOnRemote;
            component.changeLayerOnRemote_IncludeChildren = ChangeLayerOnRemote_IncludeChildren;
            component.remoteLayer = RemoteLayer;
            component.onRemoteStart = onRemoteStart;

            serializedObject.ApplyModifiedProperties();

            UnityEditor.EditorUtility.SetDirty(component);
        }

        void OnWizardUpdate()
        {
            helpString = "Set how this component behaves on a network.";

            errorString = "";
        }

        protected override bool DrawWizardGUI()
        {
            bool b = base.DrawWizardGUI();

            try
            {
                foreach (var field in component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    // Check if the field is CockpitControlled
                    var att = field.GetCustomAttributes(typeof(AVR.Core.Attributes.ShowInNetPrompt), true).FirstOrDefault();
                    if (att != null)
                    {
                        SerializedProperty prop = serializedObject.FindProperty(field.Name);
                        EditorGUILayout.PropertyField(prop);
                    }
                }
            }
            catch(System.Exception)
            {
                return false;
            }

            return b;
        }
    }
}

#endif