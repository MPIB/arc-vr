using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

using AVR.Core;
using AVR.UEditor.Core;
using AVR.Phys;

/*
namespace AVR.UEditor.Phys {
    public class AVR_Controller_ModuleWizard_Hook_HandVisual : AVR_Controller_ModuleWizard_Hook
    {
        private bool HandVisual;
        private enum hvType { CONTROLLER, HAND, SPHERE };
        private hvType handVisualType;
        private hvType old_handVisualType;
        private AVR_HandVisual[] _handVisual;

        public override void on_create_wizard(AVR_Controller controller)
        {
            _handVisual = controller.GetComponentsInChildren<AVR_HandVisual>();
            HandVisual = _handVisual.Length > 0;
            if (_handVisual.All(p => p.GetType() == typeof(AVR_Hand))) old_handVisualType = handVisualType = hvType.HAND;
            if (_handVisual.All(p => p.GetType() == typeof(AVR_HandVisual_Controller))) old_handVisualType = handVisualType = hvType.CONTROLLER;
            // TODO: other HandVisual types (SPHERE)
        }

        public override void embed_GUI() {
            HandVisual = EditorGUILayout.BeginToggleGroup("HandVisual", HandVisual);

            handVisualType = (hvType)EditorGUILayout.EnumPopup(handVisualType);
            
            // Nothing to see here
            EditorGUILayout.EndToggleGroup();
        }

        public override void on_submit(AVR_Controller controller)
        {
            if (HandVisual && (_handVisual.Length < 1 || old_handVisualType != handVisualType))
            {
                // Grabprovider was changed. First delete all the old ones, then add the new ones
                if (old_handVisualType != handVisualType) foreach (AVR_HandVisual c in _handVisual) GameObject.DestroyImmediate(c.gameObject);
                // Add new grabprovider
                if (this.handVisualType == hvType.HAND && controller.controllerHand == AVR_Controller.ControllerHand.Left) AVR_Phys_EditorUtility.InstantiatePrefabAsChild(controller.transform, "editor/defaultPrefabPaths/leftHandVisual");
                else if (this.handVisualType == hvType.HAND) AVR_Phys_EditorUtility.InstantiatePrefabAsChild(controller.transform, "editor/defaultPrefabPaths/rightHandVisual");
                else if (this.handVisualType == hvType.CONTROLLER) AVR_Phys_EditorUtility.InstantiatePrefabAsChild(controller.transform, "editor/defaultPrefabPaths/HandVisual_Controller");
                // TODO: other HANDVISUAL types (SPHERE)
            }
            else if (!HandVisual && _handVisual.Length > 0)
            {
                foreach (AVR_HandVisual c in _handVisual) GameObject.DestroyImmediate(c.gameObject);
            }
        }
    }
}
*/