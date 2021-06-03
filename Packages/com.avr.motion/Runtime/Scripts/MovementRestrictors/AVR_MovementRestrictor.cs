using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR;
using AVR.Core;

namespace AVR.Motion {
    [CreateAssetMenu(fileName = "Data", menuName = "arc-vr/motion/MovementRestrictor", order = 1)]
    public class AVR_MovementRestrictor : ScriptableObject
    {
        public bool limitTPLocation_byTag = false;
        public bool limitTPLocation_byLayer = false;
        public bool limitTPLocation_bySlope = false;

        public string validTPTag;
        public LayerMask validTPLayers;
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
