using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVR.Core {
    /// <summary>
    /// Will fade-in / fade-out the screen to a given color on command.
    /// NOTE: The color will *only* be faded for the main camera (Camera.main). If you want it to run for *all* cameras or different ones, inherit from this object and adjust the OnPostRenderCallback function.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___camera_fade.html")]
    public class AVR_CameraFade : AVR.Core.AVR_Effect
    {
        /// <summary>
        /// Material we fade to. Leave this blank for a uniform color.
        /// </summary>
        public Material fadeMaterial;

        /// <summary>
        /// Value of the "_Color" component of the materials shader.
        /// </summary>
        public Color overlayColor;

        /// <summary>
        /// Starting alpha value of the color
        /// </summary>
        public float start_value = 0.0f;

        /// <summary>
        /// Ending alpha value of the color
        /// </summary>
        public float end_value = 1.0f;

        /// <summary>
        /// Time for fade-in effect to conclude.
        /// </summary>
        public float fade_in_duration = 0.5f;
        
        /// <summary>
        /// Time for fade-out effect to conclude.
        /// </summary>
        public float fade_out_duration = 0.5f;

        // Private
        private float stime = 0.0f;
        private bool do_fade_in = false;
        private bool do_fade_out = false;

        protected virtual void Awake()
        {
            if (fadeMaterial == null) fadeMaterial = new Material(Shader.Find("AVR/CameraFade"));
            overlayColor.a = start_value;
            fadeMaterial.SetColor("_Color", overlayColor);
        }

        protected virtual void OnEnable() {
            Camera.onPostRender += OnPostRenderCallback;
        }

        protected virtual void OnDisable() {
            Camera.onPostRender -= OnPostRenderCallback;
        }

        public override void StartEffect()
        {
            do_fade_in = true;
            stime = Time.time;
        }

        public override void EndEffect()
        {
            do_fade_out = true;
            stime = Time.time;
        }

        public override void StartEndEffect()
        {
            do_fade_in = do_fade_out = true;
            stime = Time.time;
        }

        protected virtual void Update()
        {
            if (do_fade_in)
            {
                float t = Time.time - stime;
                overlayColor.a = Mathf.Lerp(start_value, end_value, t / fade_in_duration);
                fadeMaterial.SetColor("_Color", overlayColor);

                if (t > fade_in_duration)
                {
                    do_fade_in = false;
                    stime = Time.time;
                }
            }

            if (do_fade_out && !do_fade_in)
            {
                float t = Time.time - stime;
                overlayColor.a = Mathf.Lerp(end_value, start_value, t / fade_out_duration);
                fadeMaterial.SetColor("_Color", overlayColor);

                if (t > fade_out_duration)
                {
                    do_fade_out = false;
                }
            }
        }

        public override bool isBusy()
        {
            return do_fade_in || do_fade_out;
        }

        protected virtual void OnPostRenderCallback(Camera cam)
        {
            if (overlayColor.a > 0 && cam==Camera.main)
            {
                GL.PushMatrix();
                GL.LoadOrtho();
                fadeMaterial.SetPass(0);
                GL.Begin(GL.QUADS);
                GL.Color(overlayColor);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(1, 0, 0);
                GL.Vertex3(1, 1, 0);
                GL.Vertex3(0, 1, 0);
                GL.End();
                GL.PopMatrix();
            }
        }
    }
}
