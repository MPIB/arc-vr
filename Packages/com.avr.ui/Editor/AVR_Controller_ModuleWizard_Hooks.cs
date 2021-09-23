using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;
using AVR.UEditor.Core;
//using AVR.UI;

namespace AVR.UEditor.UI {
    public class AVR_ControllerWizard_Hook_UIInteractionProvider : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.UI.AVR_UIInteractionProvider>
    {
        protected override string moduleName => "UIInteractionProvider";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/uiInteractionProvider";
        protected override string[] dependencies => new string[] { "InputManager" };
    }
}