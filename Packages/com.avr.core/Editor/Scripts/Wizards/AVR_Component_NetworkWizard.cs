#if AVR_NET

using UnityEditor;
using UnityEngine;

using AVR.Core;

namespace AVR.UEditor.Core {
    public class AVR_Component_NetworkWizard : ScriptableWizard
    {
        public AVR_Component component;
        public bool DestroyOnRemote = true;
        public bool ChangeLayerOnRemote = false;
        public bool ChangeLayerOnRemote_IncludeChildren = false;
        public int RemoteLayer = 0;
        public UnityEngine.Events.UnityEvent onRemoteStart;

        public static void CreateWizard(AVR_Component component)
        {
            AVR_Component_NetworkWizard wiz = ScriptableWizard.DisplayWizard<AVR_Component_NetworkWizard>("AVR_Component Network Settings", "Apply");
            wiz.component = component;

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

            UnityEditor.EditorUtility.SetDirty(component);
        }

        void OnWizardUpdate()
        {
            helpString = "Set how this component behaves on a network.";

            errorString = "";
        }
    }
}

#endif