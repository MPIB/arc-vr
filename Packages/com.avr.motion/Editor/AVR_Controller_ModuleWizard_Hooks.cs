using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.UEditor.Core;

namespace AVR.UEditor.Motion {
    public class AVR_ControllerWizard_Hook_LocomotionProvider : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.Motion.AVR_LocomotionProvider>
    {
        protected override string moduleName => "LocomotionProvider";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/locomotionProvider";
    }

    public class AVR_ControllerWizard_Hook_MovementProvider : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.Motion.AVR_MovementProvider>
    {
        protected override string moduleName => "MovementProvider";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/movementProvider";
    }

    public class AVR_ControllerWizard_Hook_TurnProvider : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.Motion.AVR_TurnProvider>
    {
        protected override string moduleName => "TurnProvider";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/turnProvider";
    }
}
