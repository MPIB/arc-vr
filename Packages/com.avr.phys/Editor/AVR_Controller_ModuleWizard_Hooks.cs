using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.UEditor.Core;

namespace AVR.UEditor.Phys
{
    public class AVR_ControllerWizard_Hook_LocomotionProvider : AVR_WizardHook_DropdownChoiceToggle<AVR_Controller_ModuleWizard, AVR.Phys.AVR_BasicGrabProvider>
    {
        protected override string moduleName => "GrabProvider";

        protected override DDChoice[] options => new[] {
            new DDChoice("Basic", "/editor/defaultPrefabPaths/basicGrabProvider", (p) => p.GetType() == typeof(AVR.Phys.AVR_BasicGrabProvider)),
            new DDChoice("Offset", "/editor/defaultPrefabPaths/offsetGrabProvider", (p) => p.GetType() == typeof(AVR.Phys.AVR_OffsetGrabProvider)),
            new DDChoice("Advanced-Offset", "/editor/defaultPrefabPaths/advancedOffsetGrabProvider", (p) => p.GetType() == typeof(AVR.Phys.AVR_AdvancedOffsetGrabProvider))
        };
    }
}
