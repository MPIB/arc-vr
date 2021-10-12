using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Phys;

/// <summary>
/// Namespace of the arc-vr-phys (UnityEditor) package
/// </summary>
namespace AVR.UEditor.Phys
{
    [CustomEditor(typeof(AVR_BasicGrabProvider), true)]
    [CanEditMultipleObjects]
    public class AVR_BasicGrabProvider_Editor : AVR.UEditor.Core.AVR_Component_Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(Application.isPlaying) {
                if(GUILayout.Button("Debug Grab Toggle")) {
                    AVR_BasicGrabProvider gp = (AVR_BasicGrabProvider) target;
                    if(gp.isGrabbing) gp.makeRelease();
                    else gp.makeGrab();
                }
            }
        }
    }
}

