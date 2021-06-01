using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

using AVR.Core;

namespace AVR.UEditor.Core {
    public class AVR_PlayerRig_ModuleWizard : AVR_HookableWizard<AVR_PlayerRig_ModuleWizard> {  }

    public class AVR_PlayerRigWizard_Hook_LeftController : AVR_WizardHook_SimpleFilteredToggle<AVR_PlayerRig_ModuleWizard, AVR_Controller>
    {
        protected override string moduleName => "Left Controller";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/leftController";
        protected override System.Func<AVR_Controller, bool> filter => (c) => c.controllerNode==UnityEngine.XR.XRNode.LeftHand;
    }

    public class AVR_PlayerRigWizard_Hook_RightController : AVR_WizardHook_SimpleFilteredToggle<AVR_PlayerRig_ModuleWizard, AVR_Controller>
    {
        protected override string moduleName => "Right Controller";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/rightController";
        protected override System.Func<AVR_Controller, bool> filter => (c) => c.controllerNode == UnityEngine.XR.XRNode.RightHand;
    }
}
