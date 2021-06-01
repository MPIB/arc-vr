#if AVR_NET

using UnityEditor;
using UnityEngine;

using AVR.Core;

namespace AVR.UEditor.Core {
    public class AVR_Component_NetworkWizard : ScriptableWizard
    {
        public AVR_Component component;
        public bool KeepOnRemote = false;
        public bool ChangeLayerOnRemote = false;
        public bool ChangeLayerOnRemote_IncludeChildren = false;
        public int RemoteLayer = 0;

        public static void CreateWizard(AVR_Component component)
        {
            AVR_Component_NetworkWizard wiz = ScriptableWizard.DisplayWizard<AVR_Component_NetworkWizard>("AVR_Component Network Settings", "Apply");
            wiz.component = component;

            wiz.KeepOnRemote = component.KeepOnRemote;
            wiz.ChangeLayerOnRemote = component.ChangeLayerOnRemote;
            wiz.ChangeLayerOnRemote_IncludeChildren = component.ChangeLayerOnRemote_IncludeChildren;
            wiz.RemoteLayer = component.RemoteLayer;
        }

        // Called when Apply-Button is pressed
        void OnWizardCreate()
        {
            component.KeepOnRemote = KeepOnRemote;
            component.ChangeLayerOnRemote = ChangeLayerOnRemote;
            component.ChangeLayerOnRemote_IncludeChildren = ChangeLayerOnRemote_IncludeChildren;
            component.RemoteLayer = RemoteLayer;

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