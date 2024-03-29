﻿using System.Collections;
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

        public AVR_ControllerInputManager inputManager => AVR_UIInteractionProvider.currentActive?.controller?.inputManager;

        public AVR_ControllerInputManager.BoolEvent mouseButton0Click => AVR_UIInteractionProvider.currentActive ? AVR_UIInteractionProvider.currentActive.mouseButton0Click : AVR_ControllerInputManager.BoolEvent.ALWAYS_FALSE;
        public AVR_ControllerInputManager.BoolEvent mouseButton0Down => AVR_UIInteractionProvider.currentActive ? AVR_UIInteractionProvider.currentActive.mouseButton0Down : AVR_ControllerInputManager.BoolEvent.ALWAYS_FALSE;
        public AVR_ControllerInputManager.BoolEvent mouseButton0Up => AVR_UIInteractionProvider.currentActive ? AVR_UIInteractionProvider.currentActive.mouseButton0Up : AVR_ControllerInputManager.BoolEvent.ALWAYS_FALSE;
        public AVR_ControllerInputManager.BoolEvent mouseButton1Click => AVR_UIInteractionProvider.currentActive ? AVR_UIInteractionProvider.currentActive.mouseButton1Click : AVR_ControllerInputManager.BoolEvent.ALWAYS_FALSE;
        public AVR_ControllerInputManager.BoolEvent mouseButton1Down => AVR_UIInteractionProvider.currentActive ? AVR_UIInteractionProvider.currentActive.mouseButton1Down : AVR_ControllerInputManager.BoolEvent.ALWAYS_FALSE;
        public AVR_ControllerInputManager.BoolEvent mouseButton1Up => AVR_UIInteractionProvider.currentActive ? AVR_UIInteractionProvider.currentActive.mouseButton1Up : AVR_ControllerInputManager.BoolEvent.ALWAYS_FALSE;

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

        public override bool mousePresent { get { return inputManager!=null; } }

        public override bool GetMouseButton(int button)
        {
            return button == 1 ?
                inputManager && inputManager.getEventStatus(mouseButton1Click)
                :
                inputManager && inputManager.getEventStatus(mouseButton0Click)
            ;
        }

        public override bool GetMouseButtonDown(int button)
        {
            return button == 1 ?
                inputManager && inputManager.getEventStatus(mouseButton1Down)
                :
                inputManager && inputManager.getEventStatus(mouseButton0Down)
            ;
        }

        public override bool GetMouseButtonUp(int button)
        {
            return button == 1 ?
                inputManager && inputManager.getEventStatus(mouseButton1Up)
                :
                inputManager && inputManager.getEventStatus(mouseButton0Up)
            ;
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
