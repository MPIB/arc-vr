using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using AVR;
using AVR.Core;

namespace AVR.UI {
    [RequireComponent(typeof(BaseInputModule))]
    /// <summary>
    ///  This class overrides the base Input with Input from our VR components, thus enabling UI Interaction with a VR controller. The public attributes are automatically set by UIInteractionProvider.
    /// </summary>
    public class VRInput : BaseInput
    {
        /// <summary>
        /// Singleton instance of this class. There should only be one VRInput component.
        /// </summary>
        public static VRInput Instance { get; private set; }

        private Camera eventCamera = null;

        // We hide these components because they are set by UIInteractionProvider
        [HideInInspector]
        public AVR_ControllerInputManager inputManager;
        [HideInInspector]
        public AVR_ControllerInputManager.BoolEvent clickButton;
        [HideInInspector]
        public AVR_ControllerInputManager.BoolEvent clickButton_down;
        [HideInInspector]
        public AVR_ControllerInputManager.BoolEvent clickButton_up;

        /// <summary>
        /// Sets a given camera to the current eventcamera. Typically called from AVR_UIInteractionprovider.
        /// </summary>
        /// <param name="eventCamera"> newly set eventcamera </param>
        public void setEventCamera(Camera eventCamera) {
            this.eventCamera = eventCamera;
            foreach(AVR_Canvas c in AVR_Canvas.all_canvases) {
                c.canvas.worldCamera = eventCamera;
            }
        }

        protected override void Awake() 
        {
            if (Instance != null) GameObject.Destroy(Instance);
            Instance = this;

            DontDestroyOnLoad(this);

            GetComponent<BaseInputModule>().inputOverride = this;
        }

        public override bool GetMouseButton(int button)
        {
            return inputManager && inputManager.getEventStatus(clickButton);
        }

        public override bool GetMouseButtonDown(int button)
        {
            return inputManager && inputManager.getEventStatus(clickButton_down);
        }

        public override bool GetMouseButtonUp(int button)
        {
            return inputManager && inputManager.getEventStatus(clickButton_up);
        }

        public override Vector2 mousePosition
        {
            get
            {
                if(!eventCamera) return new Vector2(0,0);
                return new Vector2(eventCamera.pixelWidth/2, eventCamera.pixelHeight/2);
            }
        }
    }
}
