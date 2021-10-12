using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.UEditor.Core;

/// <summary>
/// Namespace of the arc-vr-motion (UnityEditor) package
/// </summary>
namespace AVR.UEditor.Motion {
    public class AVR_ControllerWizard_Hook_LocomotionProvider : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.Motion.AVR_LocomotionProvider>
    {
        protected override string moduleName => "LocomotionProvider";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/locomotionProvider";
        protected override string[] dependencies => new string[] { "InputManager" };
    }

    public class AVR_ControllerWizard_Hook_MovementProvider : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.Motion.AVR_MovementProvider>
    {
        protected override string moduleName => "MovementProvider";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/movementProvider";
        protected override string[] dependencies => new string[] { "InputManager" };
    }

    public class AVR_ControllerWizard_Hook_TurnProvider : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.Motion.AVR_TurnProvider>
    {
        protected override string moduleName => "TurnProvider";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/turnProvider";
        protected override string[] dependencies => new string[] { "InputManager" };
    }
}
