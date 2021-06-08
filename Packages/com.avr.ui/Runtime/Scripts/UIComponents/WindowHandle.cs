using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using AVR.Core;
using AVR;
using AVR.UI;

public class WindowHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public AVR_Canvas canvas;

    Transform og_parent = null;
    bool clicked = false;

    void Update() {
        // Prototype to resize windows, Unfinished:
        //if(clicked && AVR_Root.Instance.UIInteractionController.getEventStatus(AVR_ControllerInputManager.BoolEvent.PRIMARY2DAXIS_TOUCH)) {
        //    Vector3 scale = canvas.transform.localScale;
        //    scale *= (1.0f+AVR_Root.Instance.UIInteractionController.primary2DAxis.y);
        //    scale.z = 1.0f;
        //    canvas.transform.localScale = scale;
        //}
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if(clicked) return;
        clicked = true;
        og_parent = canvas.transform.parent;
        canvas.anchor_to_transform(AVR_UIInteractionProvider.currentActive.transform);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!clicked) return;
        clicked = false;
        canvas.anchor_to_transform(og_parent);
    }
}
