using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Namespace of the arc-vr-phys package
/// </summary>
namespace AVR.Phys {
    /// <summary>
    /// Represents a grabbable node on an AVR_Grabbable. Nodes have preset poses/constraints for the hand that are applied when grabbed.
    /// </summary>
    [ExecuteInEditMode]
    public class AVR_GrabNode : AVR.Core.AVR_Behaviour
    {
        [Range(0f, 1f)]
        public float index_pose = 0.0f;
        [Range(0f, 1f)]
        public float middle_pose = 0.0f;
        [Range(0f, 1f)]
        public float ring_pose = 0.0f;
        [Range(0f, 1f)]
        public float pinky_pose = 0.0f;
        [Range(0f, 1f)]
        public float thumb_pose = 0.0f;

        public float allowed_pitch = 0.0f;
        public float allowed_yaw = 0.0f;
        public float allowed_roll = 0.0f;

        [Range(-1f, 1f)]
        public float pitch_test = 0.0f;
        [Range(-1f, 1f)]
        public float roll_test = 0.0f;
        [Range(-1f, 1f)]
        public float yaw_test = 0.0f;

        public float override_radius = 0.1f;

        private AVR.Phys.AVR_Grabbable master;

        public void Awake() {
            master = GetComponentInParent<AVR.Phys.AVR_Grabbable>();
        }

        public Quaternion get_target_rotation(Quaternion hand_rot) {
            // Hand rotation * local node rotation (relative to the master grabbable). This is the rotation the grabbed object should have
            // if allowed pitch, yaw and roll were to be 0
            Quaternion zeroAngRot = hand_rot * (Quaternion.Inverse(transform.rotation) * master.transform.rotation);

            // Rotation of the master grabbable. This is the rotation the object should have if allowd yaw pitch and roll were to be >360.
            Quaternion fullAngRot = master.transform.rotation;

            // Rotation from zeroAng to fullAng
            Quaternion fromToRot = fullAngRot * Quaternion.Inverse(zeroAngRot);

            // Clamping rotation by yaw pitch roll values
            fromToRot = AVR.Core.Utils.Geom.ClampQuaternionRotation(fromToRot, new Vector3(allowed_pitch, allowed_yaw, allowed_roll));

            return zeroAngRot * fromToRot;
        }    
    }
}
