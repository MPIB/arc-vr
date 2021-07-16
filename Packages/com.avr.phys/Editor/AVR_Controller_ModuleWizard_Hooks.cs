using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.UEditor.Core;

namespace AVR.UEditor.Phys
{
    public class AVR_ControllerWizard_Hook_GrabProvider : AVR_WizardHook_DropdownChoiceToggle<AVR_Controller_ModuleWizard, AVR.Phys.AVR_BasicGrabProvider>
    {
        protected override string moduleName => "GrabProvider";

        protected override DDChoice[] options => new[] {
            new DDChoice("Basic", "/editor/defaultPrefabPaths/basicGrabProvider", (p) => p.GetType() == typeof(AVR.Phys.AVR_BasicGrabProvider)),
            new DDChoice("Offset", "/editor/defaultPrefabPaths/offsetGrabProvider", (p) => p.GetType() == typeof(AVR.Phys.AVR_OffsetGrabProvider)),
            new DDChoice("Advanced-Offset", "/editor/defaultPrefabPaths/advancedOffsetGrabProvider", (p) => p.GetType() == typeof(AVR.Phys.AVR_AdvancedOffsetGrabProvider))
        };
    }

    public class AVR_ControllerWizard_Hook_HandInteractor : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.Phys.AVR_Hand>
    {
        private bool is_right_hand = false;

        protected override string moduleName => "HandInteractor";

        protected override string prefabPathSettingsToken => is_right_hand ? "/editor/defaultPrefabPaths/rightHandInteractor" : "/editor/defaultPrefabPaths/leftHandInteractor";

        public override void on_submit(GameObject targetObject)
        {
            try {
                is_right_hand = targetObject.GetComponent<AVR.Core.AVR_Controller>().controllerNode == UnityEngine.XR.XRNode.RightHand;
            } catch (System.Exception) { }
            base.on_submit(targetObject);
        }
    }
}
