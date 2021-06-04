using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR;
using AVR.Core;
using AVR.Core.Attributes;

namespace AVR.Motion {
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_motion_1_1_a_v_r___movement_restrictor.html")]
    [CreateAssetMenu(fileName = "Data", menuName = "arc-vr/motion/MovementRestrictor", order = 1)]
    /// <summary>
    /// When using a MovementProvider, this scriptable object allows you to set rules for which surfaces qualify as valid teleport locations and which dont.
    /// </summary>
    public class AVR_MovementRestrictor : ScriptableObject
    {
        [Header("Limit by gameObject tag")]
        public bool limitTPLocation_byTag = false;

        [ConditionalHideInInspector("limitTPLocation_byTag", true)]
        public string validTPTag;

        [Header("Limit by gameObject layer")]
        public bool limitTPLocation_byLayer = false;

        [ConditionalHideInInspector("limitTPLocation_byLayer", true)]
        public LayerMask validTPLayers;

        [Header("Limit by maximum slope")]
        public bool limitTPLocation_bySlope = false;

        [ConditionalHideInInspector("limitTPLocation_bySlope", true)]
        public float validTPmaxSlope;

        public bool isValidTpLocation(RaycastHit loc)
        {
            if (limitTPLocation_bySlope && Vector3.Angle(Vector3.up, loc.normal) > validTPmaxSlope) return false;
            if (limitTPLocation_byTag && !loc.collider.gameObject.CompareTag(validTPTag)) return false;
            if (limitTPLocation_byLayer && (validTPLayers & 1 << loc.collider.gameObject.layer) < 1) return false;
            return true;
        }
    }
}
