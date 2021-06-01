using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// Represents a generic effect, such as fading a color on a camera.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___effect.html")]
    public abstract class AVR_Effect : AVR_Behaviour
    {
        /// <summary>
        /// Start the effect
        /// </summary>
        public abstract void StartEffect();

        /// <summary>
        /// End the effect
        /// </summary>
        public abstract void EndEffect();

        /// <summary>
        /// Start the effect and automatically end it right after
        /// </summary>
        public abstract void StartEndEffect();

        /// <summary>
        /// True if the effect is starting/started or ending but not yet ended.
        /// </summary>
        public abstract bool isBusy();
    }
}
