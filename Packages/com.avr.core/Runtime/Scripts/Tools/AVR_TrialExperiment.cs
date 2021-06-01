using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Need better documentation

namespace AVR.Core {
    /// <summary>
    /// Base class to inherit from in order to create trial-based experiments. (One experiment consists of one or more trials.)
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___trial_experiment.html")]
    public abstract class AVR_TrialExperiment : AVR_Behaviour
    {
        /// <summary>
        /// IEnumerator-coroutine that represents a single tiral
        /// </summary>
        public abstract IEnumerator trial();

        /// <summary>
        /// Function that gets called when the experiment starts
        /// </summary>
        public virtual void on_start() { }

        /// <summary>
        /// Function that gets called when the experiment ends
        /// </summary>
        public virtual void on_end() { }

        /// <summary>
        /// Function that gets called when a trial starts
        /// </summary>
        public virtual void on_start_trial() { }

        /// <summary>
        /// Function that gets called when a trial ends
        /// </summary>
        public virtual void on_end_trial() { }

        /// <summary>
        /// This function returns true *once* after the can_proceed function has been called. Use this within the trial Coroutine like so: yield return new WaitUntil(can_proceed()). Then call the proceed() function on certain events, such as button presses.
        /// </summary>
        public bool can_proceed() {
            if (_p)
            {
                _p = false;
                return true;
            }
            return false;
        }
        private bool _p = false;

        /// <summary>
        /// Call this function externally to allow the trial to proceed.
        /// </summary>
        public void proceed() {
            _p = true;
        }

        private IEnumerator experiment(int trial_amount) {
            on_start();

            for(int i=0; i<trial_amount; i++) {
                on_start_trial();
                yield return StartCoroutine(this.trial());
                on_end_trial();
            }

            on_end();
        }
        
        /// <summary>
        /// Call this function to commence an experiment with a given amount of trials.
        /// </summary>
        /// <param name="trial_amount"> Amount of trials to run. </param>
        public void commence(int trial_amount=1) {
            StartCoroutine(experiment(trial_amount));
        }
    }
}
