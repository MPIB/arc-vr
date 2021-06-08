using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using AVR;
using AVR.Core;
using AVR.UI;

[RequireComponent(typeof(Canvas))]
public class AVR_Canvas : AVR_Behaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static List<AVR_Canvas> all_canvases = new List<AVR_Canvas>();

    private Plane canvasPlane;
    private Canvas canvas;

    void Awake() {
        all_canvases.Add(this);
        canvasPlane = new Plane();
        canvasPlane.Set3Points(
            transform.position,
            transform.position + transform.right,
            transform.position + transform.up
        );

        canvas = GetComponent<Canvas>();
    }

    public Canvas GetCanvas() {
        return canvas;
    }

    void OnDisable() {
        AVR_UIInteractionProvider.unregister_UIInteraction(this);
    }

    public void OnDestroy()
    {
        all_canvases.Remove(this);
    }

    public void setUIInteractionProvider(AVR.UI.AVR_UIInteractionProvider p) {
        canvas.worldCamera = p.UIRay.UICamera;
    }

    public void anchor_to_world()
    {
        anchor_to_transform(null);
    }

    public void anchor_to_player()
    {
        this.transform.SetParent(Camera.main.transform, true);
        AVR_DevConsole.print(gameObject.name + " anchored to player");
    }

    public void anchor_to_controller(AVR_ControllerInputManager controller)
    {
        this.transform.SetParent(controller.transform, true);
        AVR_DevConsole.print(gameObject.name + " anchored to controller");
    }

    public void anchor_to_transform(Transform t)
    {
        if(t==null) {
            // We need to do this circus each time we parent to world because something doesnt work
            Vector3 tmp = this.transform.position;
            this.transform.SetParent(null, true);
            transform.position = tmp;
            AVR_DevConsole.print(gameObject.name + " anchored to world");
        }
        else{
            this.transform.SetParent(t, true);
            AVR_DevConsole.print(gameObject.name + " anchored to "+t.gameObject.name);
        }
    }

    public Plane GetPlane() {
        canvasPlane = new Plane();
        canvasPlane.Set3Points(
            transform.position,
            transform.position + transform.right,
            transform.position + transform.up
        );
        return canvasPlane;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AVR_UIInteractionProvider.register_UIInteraction(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        AVR_UIInteractionProvider.unregister_UIInteraction(this);
    }
    
}
