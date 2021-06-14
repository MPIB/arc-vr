using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using AVR.Core;

public class NaturalizedController : AVR.Core.AVR_ControllerComponent
{
    List<Vector3> refpoints = new List<Vector3> {
        new Vector3(0.3130672f, -0.8555371f, -0.05349071f),
        new Vector3(0.1875403f, -0.1843239f, 0.5226004f),
        new Vector3(0.1297842f, 0.3305759f, 0.1609094f),
        new Vector3(0.6633397f, -0.188998f, 0.2596239f),
        new Vector3(-0.1863071f, -0.2506216f, 0.2888376f),
        new Vector3(0.3763097f, 0.2223641f, 0.3407036f),
        new Vector3(0.4919417f, -0.6919423f, 0.1794233f),
        new Vector3(-0.06197758f, -0.6310289f, 0.3017172f),
        new Vector3(-0.1150279f, 0.1570742f, 0.3689314f),
        new Vector3(0.06075469f, -0.5881292f, 0.4081661f),
        new Vector3(-0.1293586f, -0.4548941f, 0.3687209f),
        new Vector3(-0.2344741f, -0.2135523f, 0.3372273f),
        new Vector3(-0.1965994f, 0.04493928f, 0.3594324f),
        new Vector3(-0.05899038f, 0.2399873f, 0.357431f),
        new Vector3(0.1459467f, 0.273103f, 0.3742191f),
        new Vector3(0.3480873f, 0.2285503f, 0.3722088f),
        new Vector3(0.4762831f, 0.05860758f, 0.3898822f),
        new Vector3(0.5396073f, -0.1755121f, 0.3975164f),
        new Vector3(0.5242664f, -0.3851136f, 0.3863374f),
        new Vector3(0.4510424f, -0.6661255f, 0.3000682f),
        new Vector3(0.2893045f, -0.7522406f, 0.2654127f),
        new Vector3(0.2396608f, -0.4571418f, 0.4997253f),
        new Vector3(0.009911999f, -0.3112085f, 0.4998798f),
        new Vector3(0.02352711f, 0.00599493f, 0.4862583f),
        new Vector3(0.2817126f, 0.0698675f, 0.4846259f),
        new Vector3(0.3737018f, -0.2748309f, 0.4511977f),
        new Vector3(0.1789444f, -0.2061029f, 0.5697523f),
        new Vector3(0.5598639f, -0.1432514f, 0.05959378f),
        new Vector3(0.3332149f, 0.253341f, 0.1296776f),
        new Vector3(0.1210886f, 0.4511154f, 0.1342202f),
        new Vector3(0.1417553f, -0.1538361f, 0.6045761f),
        new Vector3(-0.1915717f, -0.255244f, 0.2346186f),
        new Vector3(0.6717821f, -0.6318416f, 0.1174976f),
        new Vector3(0.6645142f, -0.6774831f, -0.01035453f),
        new Vector3(0.8222181f, -0.3584935f, -0.04209535f),
        new Vector3(0.7579225f, 0.03175234f, 0.0615076f),
        new Vector3(0.07855002f, -0.2243347f, 0.2829997f),
        new Vector3(0.07432391f, -0.1835844f, 0.2390581f),
        new Vector3(0.0604629f, -0.1005674f, 0.2226095f),
        new Vector3(0.1114236f, -0.2708174f, 0.1091078f),
        new Vector3(-0.1355885f, -0.2771785f, 0.07465832f),
        new Vector3(-0.1897227f, -0.4594658f, 0.2602356f),
        new Vector3(0.06223706f, -0.1716432f, 0.2982705f),
        new Vector3(0.009867656f, -0.0385427f, 0.4952148f),
        new Vector3(-0.007234326f, 0.1460291f, 0.3986637f),
        new Vector3(0.01339677f, -0.2846705f, 0.5164974f),
        new Vector3(0.2728412f, -0.0150883f, 0.3643675f),
        new Vector3(-0.2064285f, -0.04086648f, 0.3411852f),
        new Vector3(0.1071309f, 0.2709712f, 0.2849442f),
        new Vector3(-0.1139413f, 0.2537714f, 0.2704613f),
        new Vector3(-0.1047982f, -0.4392074f, 0.4727466f),
        new Vector3(0.1481976f, -0.4856894f, 0.5008735f),
        new Vector3(0.4463902f, 0.07595506f, 0.4507528f),
        new Vector3(0.263638f, 0.3154329f, 0.3439315f),
        new Vector3(0.3473695f, -0.3119796f, 0.4499293f),
        new Vector3(0.05217796f, -0.1100652f, 0.6295177f),
        new Vector3(0.06441771f, -0.4936383f, 0.4601623f),
        new Vector3(-0.1461361f, -0.5006831f, 0.3856009f),
        new Vector3(0.2252315f, -0.5019503f, 0.4179451f),
        new Vector3(0.3439449f, 0.3608203f, 0.2712157f),
        new Vector3(-0.04797429f, 0.2481471f, 0.2394861f),
        new Vector3(0.1578359f, 0.3546618f, 0.2871732f),
        new Vector3(-0.2828721f, -0.3350565f, 0.210703f),
        new Vector3(-0.299469f, -0.1014449f, 0.2097515f),
        new Vector3(-0.1493744f, 0.2246452f, 0.2612446f)
    };

    protected override void Start()
    {
        base.Start();

        refpoints = refpoints.Take(30).ToList();
    }

    private Vector3 outpos = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        Vector3 wpos = controller.transform.position;

        Vector3 cpos = AVR_PlayerRig.Instance.MainCamera.transform.InverseTransformPoint(wpos);

        Vector3 closest = find_closest(cpos);

        Vector3 targetPos = AVR_PlayerRig.Instance.MainCamera.transform.TransformPoint(closest);

        outpos = Vector3.Lerp(outpos, targetPos, Time.deltaTime*10.0f);

        transform.position = outpos;
    }

    Vector3 find_closest(Vector3 p) {
        int k = 3;
        List<Vector3> kn = refpoints.OrderBy(item => Vector3.Distance(item, p)).Take(k).ToList();

        Vector3 sum = Vector3.zero;
        float wsum = 0;
        foreach(var v in kn) {
            float w = 1.0f / (0.0001f+Vector3.Distance(v, p));
            wsum += w;
            sum += w*v;
        }

        sum /= wsum;

        return sum;
    }
}
