using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;


namespace AVR.Core {
    /// <summary>
    /// Root object of the arc-vr-core library. Functions as a Singleton.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___root.html")]
    public class AVR_Root : Singleton<AVR_Root>
    {
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            if(Instance==null) {
                AVR_DevConsole.cwarn("No AVR_Root component is present. Creating one at runtime.", "AVR_Root");
                var root = AVR.Core.Utils.Misc.CreateEmptyGameObject("Root");
                root.gameObject.AddComponent<AVR_Root>();
            }
        }

        public void ReEnableAtEndOfFrame(GameObject obj) {
            StartCoroutine(_ReEnableAtEndOfFrame(obj));
        }

        private IEnumerator _ReEnableAtEndOfFrame(GameObject obj) {
            yield return new WaitForEndOfFrame();
            obj.SetActive(true);
        }
    }
}
