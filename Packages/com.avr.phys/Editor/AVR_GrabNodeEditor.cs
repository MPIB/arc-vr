using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AVR_GrabNode), true)]
public class AVR_GrabNode_Editor : AVR.UEditor.Core.AVR_Behaviour_Editor
{
    private bool drawDebugHand => debugHand!=null;

    private GameObject debugHand = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!drawDebugHand && GUILayout.Button("Draw DebugHand"))
        {
            debugHand = AVR.UEditor.Core.AVR_EditorUtility.InstantiatePrefabAsChild(((AVR_GrabNode)target).transform, "/editor/defaultPrefabPaths/debugHand");
        }
        else if(drawDebugHand && GUILayout.Button("Hide DebugHand"))
        {
            GameObject.DestroyImmediate(debugHand);
        }
    }

    void OnEnable() {
        try {
            debugHand = ((AVR_GrabNode)target).GetComponentInChildren<AVR.Phys.AVR_DebugHand>().gameObject;
        } catch(System.Exception) { }
    }

    void OnDisable() {
        //GameObject.DestroyImmediate(debugHand);
    }

    public void OnSceneGUI()
    {
        AVR_GrabNode node = (AVR_GrabNode)target;

        const float angle_arches_size = 0.12f;

        Handles.color = new Color(0, 1, 0, 0.3f);
        Handles.DrawSolidArc(node.transform.position, node.transform.up, node.transform.forward, node.allowed_pitch, angle_arches_size);
        Handles.DrawSolidArc(node.transform.position, node.transform.up, node.transform.forward, -node.allowed_pitch, angle_arches_size);

        Handles.color = new Color(0, 0, 1, 0.3f);
        Handles.DrawSolidArc(node.transform.position, node.transform.forward, node.transform.right, node.allowed_roll, angle_arches_size);
        Handles.DrawSolidArc(node.transform.position, node.transform.forward, node.transform.right, -node.allowed_roll, angle_arches_size);

        Handles.color = new Color(1, 0, 0, 0.3f);
        Handles.DrawSolidArc(node.transform.position, node.transform.right, node.transform.up, node.allowed_yaw, angle_arches_size);
        Handles.DrawSolidArc(node.transform.position, node.transform.right, node.transform.up, -node.allowed_yaw, angle_arches_size);

        Handles.color = Color.yellow;
        const int circle_res = 4;
        for(int i=0; i<circle_res; i++) {
            float fac = ((float)i / circle_res);
            Handles.DrawWireDisc(node.transform.position, fac * node.transform.right + (1f - fac) * node.transform.forward, node.override_radius);
            Handles.DrawWireDisc(node.transform.position, fac * -node.transform.right + (1f - fac) * node.transform.forward, node.override_radius);
        }
        Handles.DrawWireDisc(node.transform.position, node.transform.up, node.override_radius);
    }
}
