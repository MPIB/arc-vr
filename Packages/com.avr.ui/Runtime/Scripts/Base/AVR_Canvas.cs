using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using AVR.Core;

/// <summary>
/// Namespace of the arc-vr-ui package
/// </summary>
namespace AVR.UI {
    [RequireComponent(typeof(Canvas))]
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_u_i_1_1_a_v_r___canvas.html")]
    /// <summary>
    /// Represents a canvas for VR interaction purposes. Allows for interaction with the AVR_UIInteractionProvider.
    /// </summary>
    public class AVR_Canvas : AVR_Behaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// A list of all AVR_Canvases that have at some point been active and not destroyed since
        /// </summary>
        /// <value></value>
        public static List<AVR_Canvas> all_canvases { get; private set; }

        /// <summary>
        /// A list of all AVR_Canvases that the user is currently interacting with. NOTE: This is *not* the same as all canvases that are active in the scene.
        /// </summary>
        /// <value></value>
        public static List<AVR_Canvas> active_canvases { get; private set; }

        /// <summary>
        /// The UnityEngine.Canvas Component this object is attatched to.
        /// </summary>
        public Canvas canvas { get; private set; }

        void Awake() {
            all_canvases = new List<AVR_Canvas>();
            active_canvases = new List<AVR_Canvas>();

            all_canvases.Add(this);

            canvas = GetComponent<Canvas>();

            if(!canvas) {
                AVR_DevConsole.cerror("AVR_Canvas does not have a Canvas component!", this);
                Destroy(this);
            }
        }

        void OnDisable() {
            if (active_canvases.Contains(this)) active_canvases.Remove(this);
        }

        public override void OnDestroy()
        {
            if (all_canvases.Contains(this)) all_canvases.Remove(this);
            if (active_canvases.Contains(this)) active_canvases.Remove(this);
        }

        /// <summary>
        /// Anchors this canvas to the world. (Sets parent to null)
        /// </summary>
        public void anchor_to_world()
        {
            anchor_to_transform(null);
        }

        /// <summary>
        /// Anchors this canvas to the players HMD. Intended for HUD UI elements.
        /// </summary>
        public void anchor_to_player()
        {
            this.transform.SetParent(AVR_PlayerRig.Instance.MainCamera.transform, true);
            AVR_DevConsole.print(gameObject.name + " anchored to player");
        }

        /// <summary>
        /// Anchors the canvas to a given vr controller.
        /// </summary>
        /// <param name="controller"> Controller the canvas will be parented to </param>
        public void anchor_to_controller(AVR_ControllerInputManager controller)
        {
            this.transform.SetParent(controller.transform, true);
            AVR_DevConsole.print(gameObject.name + " anchored to controller");
        }

        /// <summary>
        /// Anchor this canvas to a given transform. Effectively the same as calling transform.SetParent(...)
        /// </summary>
        /// <param name="t">Transform the canvas will be parented to</param>
        public void anchor_to_transform(Transform t)
        {
            if(t==null) {
                // We need to do this circus each time we parent to world because something doesnt work. I seriously can't figure out what, but either way, performance impact should be close to nothing.
                Vector3 tmp = this.transform.position;
                this.transform.SetParent(null, true);
                transform.position = tmp;
            }
            else{
                this.transform.SetParent(t, true);
            }
        }

        /// <summary>
        /// Returns the 3d plane this canvas lies in
        /// </summary>
        /// <returns>3d plane this canvas lies in</returns>
        public Plane GetPlane() {
            Plane canvasPlane = new Plane();
            canvasPlane.Set3Points(
                transform.position,
                transform.position + transform.right,
                transform.position + transform.up
            );
            return canvasPlane;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!active_canvases.Contains(this)) active_canvases.Add(this);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (active_canvases.Contains(this)) active_canvases.Remove(this);
        }
    }
}
