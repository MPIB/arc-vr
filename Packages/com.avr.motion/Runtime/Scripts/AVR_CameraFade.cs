using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVR_CameraFade : AVR.Core.AVR_Effect
{
    public Material fadeMaterial;

    public Color overlayColor;
    public float start_value = 0.0f;
    public float end_value = 1.0f;
    public float fade_in_duration = 0.5f;
    public float fade_out_duration = 0.5f;

    private float stime = 0.0f;
    private bool do_fade_in = false;
    private bool do_fade_out = false;

    void Awake()
    {
        if (fadeMaterial == null) fadeMaterial = new Material(Shader.Find("AVR/CameraFade"));
        overlayColor.a = start_value;
        fadeMaterial.SetColor("_Color", overlayColor);
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

    void Update()
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

    void OnPostRender()
    {
        if (overlayColor.a > 0)
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
