using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AVR.UEditor.Core {
    public class AVR_Controller_ModuleWizard : AVR_HookableWizard<AVR_Controller_ModuleWizard> { }

    public class AVR_ControllerWizard_Hook_InputManager : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.Core.AVR_ControllerInputManager>
    {
        protected override string moduleName => "InputManager";
        protected override string prefabPathSettingsToken => "/editor/defaultPrefabPaths/inputModule";
    }

    public class AVR_ControllerWizard_Hook_ControllerVisual : AVR_WizardHook_SimpleToggle<AVR_Controller_ModuleWizard, AVR.Core.AVR_ControllerVisual>
    {
        protected override string moduleName => "InputManager";
        protected override string prefabPathSettingsToken => _token;
        private string _token = "/editor/defaultPrefabPaths/viveControllerVisual";

        public enum visualType { ViveController, Cube, Sphere };
        protected visualType type;
        protected visualType old_type;

        public override void on_create_wizard(GameObject targetObject)
        {
            base.on_create_wizard(targetObject);

            // We can't really know what type of visual is attatched atm, so we make an educated guess based on the objects name.
            if (_module.Length > 0)
            {
                string name = _module[0].gameObject.name;
                if (name.IndexOf("vive", System.StringComparison.OrdinalIgnoreCase) >= 0) type = visualType.ViveController;
                else if (name.IndexOf("cube", System.StringComparison.OrdinalIgnoreCase) >= 0) type = visualType.Cube;
                else if (name.IndexOf("sphere", System.StringComparison.OrdinalIgnoreCase) >= 0) type = visualType.Sphere;
            }

            old_type = type;
        }

        public override void embed_GUI() {
            module = EditorGUILayout.BeginToggleGroup("ControllerVisual", module);
            type = (visualType)EditorGUILayout.EnumPopup(type);
            EditorGUILayout.EndToggleGroup();
        }

        public override void on_submit(GameObject targetObject)
        {
            if (module && old_type != type)
            {
                foreach (AVR.Core.AVR_ControllerVisual c in _module) GameObject.DestroyImmediate(c.gameObject);
                _module = new AVR.Core.AVR_ControllerVisual[0];
            }

            switch (type)
            {
                case visualType.ViveController: { _token = "/editor/defaultPrefabPaths/viveControllerVisual"; break; }
                case visualType.Cube: { _token = "/editor/defaultPrefabPaths/cubeControllerVisual"; break; }
                case visualType.Sphere: { _token = "/editor/defaultPrefabPaths/sphereControllerVisual"; break; }
                default: { AVR.Core.AVR_DevConsole.cwarn("It looks like the given visualType is not implemented: " + type.ToString(), "ControllerVisual Hook"); _token = "/editor/defaultPrefabPaths/viveControllerVisual"; break; }
            }

            base.on_submit(targetObject);
        }
    }
}
