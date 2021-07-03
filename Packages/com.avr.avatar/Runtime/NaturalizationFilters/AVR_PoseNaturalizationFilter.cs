using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;

using AVR.Core;

namespace AVR.Avatar {
    [CreateAssetMenu(fileName = "PoseNaturalizationFilter", menuName = "arc-vr/avatar/PoseNaturalizationFilter", order = 1)]
    public class AVR_PoseNaturalizationFilter : ScriptableObject
    {
        public int reference_amount = 30;

        public int take_k = 4;

        public float smoothing_speed = 10.0f;

        public float interp = 1.0f;

        public List<Vector3> refpoints;

        private List<Vector3> _refpoints;

        protected Vector3 outpos = Vector3.zero;

        void OnEnable() {
            if(refpoints==null) _refpoints = new List<Vector3>();
            else _refpoints = refpoints.Take(Mathf.Min(refpoints.Count, reference_amount)).ToList();
        }

        public Vector3 naturalize_point(Vector3 wpos) {
            try {
                // Transform worldspace -> local space (relative to the PlayerRigs camera)
                Vector3 cpos = AVR.Core.AVR_PlayerRig.Instance.MainCamera.transform.InverseTransformPoint(wpos);

                // Get a weighted average of reference points that are closest to this location
                Vector3 closest = get_weighted_average_reference(cpos);

                // Smooth over time
                outpos = Vector3.Lerp(outpos, closest, Time.deltaTime * smoothing_speed);

                // Interpolate by given value
                return AVR.Core.AVR_PlayerRig.Instance.MainCamera.transform.TransformPoint(Vector3.Lerp(wpos, outpos, interp));
            }
            catch(System.Exception) {
                AVR_DevConsole.cerror("PoseNaturalizationFilter failed! Is AVR_PlayerRig.Instance set correctly?", "AVR_PoseNaturalizationFilter");
                return wpos;
            }
        }

        Vector3 get_weighted_average_reference(Vector3 p)
        {
            // TODO: we are sorting the whole list here but only want the first k items. -> Needless performance cost.
            List<Vector3> kn = _refpoints.OrderBy(item => Vector3.Distance(item, p)).Take(Mathf.Min(take_k, _refpoints.Count)).ToList();

            Vector3 sum = Vector3.zero;
            float wsum = 0;
            foreach (var v in kn)
            {
                float w = 1.0f / (0.0001f + Vector3.Distance(v, p));
                wsum += w;
                sum += w * v;
            }

            sum /= wsum;

            return sum;
        }
    }
}
