using UnityEditor;
using UnityEngine;

using AVR.Core;

namespace AVR.UEditor.Core {
    public class AVR_Component_EventsWizard : ScriptableWizard
    {
        public AVR_Component component;
        
        public UnityEngine.Events.UnityEvent onAwake = new UnityEngine.Events.UnityEvent();
        public UnityEngine.Events.UnityEvent onStart = new UnityEngine.Events.UnityEvent();
        public UnityEngine.Events.UnityEvent onEnable = new UnityEngine.Events.UnityEvent();
        public UnityEngine.Events.UnityEvent onDisable = new UnityEngine.Events.UnityEvent();

        public static void CreateWizard(AVR_Component component)
        {
            AVR_Component_EventsWizard wiz = ScriptableWizard.DisplayWizard<AVR_Component_EventsWizard>("AVR_Component Event Settings", "Apply");
            wiz.component = component;

            wiz.onAwake = component.onAwake;
            wiz.onStart = component.onStart;
            wiz.onEnable = component.onEnable;
            wiz.onDisable = component.onDisable;
        }

        // Called when Apply-Button is pressed
        void OnWizardCreate()
        {
            component.onAwake = onAwake;
            component.onStart = onStart;
            component.onEnable = onEnable;
            component.onDisable = onDisable;

            UnityEditor.EditorUtility.SetDirty(component);

        }

        void OnWizardUpdate()
        {
            helpString = "Set custom Events for this object.";

            errorString = "";

        }
    }
}