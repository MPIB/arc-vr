using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace AVR.Core {
    /// <summary>
    /// Manages inputs of a controller such as button presses and track-pad inputs.
    /// </summary>
    [AVR.Core.Attributes.DocumentationUrl("class_a_v_r_1_1_core_1_1_a_v_r___controller_input_manager.html")]
    public class AVR_ControllerInputManager : AVR_ControllerComponent
    {
        /// <summary>
        /// Boolean-returning events of a controller.
        /// </summary>
        public enum BoolEvent {
            PRIMARY2DAXIS_TOUCH,                // Primary Axis touched
            PRIMARY2DAXIS_TOUCH_LEFT,           // Primary Axis touched on the left side
            PRIMARY2DAXIS_TOUCH_RIGHT,          // Primary Axis touched on the right side
            PRIMARY2DAXIS_TOUCH_LEFTORRIGHT,    // Primary Axis touched on the left or right side
            PRIMARY2DAXIS_TOUCH_MIDDLE,         // Primary Axis touched in the middle
            PRIMARY2DAXIS_CLICK,                // Primary Axis clicked
            PRIMARY2DAXIS_CLICK_LEFT,           // Primary Axis clicked on the left side
            PRIMARY2DAXIS_CLICK_RIGHT,          // Primary Axis clicked on the right side
            PRIMARY2DAXIS_CLICK_LEFTORRIGHT,    // Primary Axis clicked on the left or right side
            PRIMARY2DAXIS_CLICK_MIDDLE,         // Primary Axis clicked in the middle
            PRIMARY2DAXIS_CLICKDOWN,            // Primary Axis clicked
            PRIMARY2DAXIS_CLICKDOWN_LEFT,       // Primary Axis clicked down on the left side
            PRIMARY2DAXIS_CLICKDOWN_RIGHT,      // Primary Axis clicked down on the right side
            PRIMARY2DAXIS_CLICKDOWN_LEFTORRIGHT,// Primary Axis clicked down on the left or right side
            PRIMARY2DAXIS_CLICKDOWN_MIDDLE,     // Primary Axis clicked down in the middle
            TRIGGER_ONTRIGGERDOWN,              // Trigger pressed
            TRIGGER_TRIGGER,                    // Trigger status
            TRIGGER_ONTRIGGERUP,                // Trigger released
            MENUBUTTON,                         // Menubutton status
            MENUBUTTON_DOWN,                    // Menubutton pressed
            MENUBUTTON_UP,                      // Menubutton released
            GRIP_GRIP,                          // Grip status
            GRIP_ONGRIPDOWN,                    // Grip pressed
            GRIP_ONGRIPUP,                      // Grip released
            ANY_CANCEL,                         // Any event that might cancel something (triggerdown, menubuttondown, primaryaxis click)
            ALWAYS_TRUE,                        // Is always true
            ALWAYS_FALSE,                       // Is always false
            PRIMARY_BUTTON,                     // Primary button (typically 'A')
            SECONDARY_BUTTON,                   // Secondary button (typically 'B')
            PRIMARY_BUTTON_DOWN,                // Primary button (typically 'A')
            SECONDARY_BUTTON_DOWN,              // Secondary button (typically 'B')
            PRIMARY_BUTTON_UP,                  // Primary button (typically 'A')
            SECONDARY_BUTTON_UP                 // Secondary button (typically 'B')
            //TODO etc.
        }

        /// <summary>
        /// Floating-Point-number-returning events of a controller.
        /// </summary>
        public enum FloatEvent
        {
            PRIMARY2DAXIS_X,
            PRIMARY2DAXIS_Y
        }

        /// <summary>
        /// 2D vector returning events of a controller.
        /// </summary>
        public enum Vec2Event
        {
            PRIMARY2DAXIS
        }

        /// <summary>
        /// True if the primary2D axis is being touched by the user
        /// </summary>
        public bool primary2DAxisTouch { 
            get {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool o);
                return o;
            }
        }

        /// <summary>
        /// True if the primary2D axis is being clicked
        /// </summary>
        public bool primary2DAxisClick {
            get {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool o);
                return o;
            }
        }

        /// <summary>
        /// True on the frame that the primary 2D axis is pressed down
        /// </summary>
        public bool primary2DAxisClickDown {
            get {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool o);
                return o && !lastAxisClick;
            }
        }

        /// <summary>
        /// True on the frame that the primary 2D axis is released
        /// </summary>
        public bool primary2DAxisClickUp {
            get {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool o);
                return !o && lastAxisClick;
            }
        }

        /// <summary>
        /// 2D value returned by the primary 2D axis.
        /// </summary>
        public Vector2 primary2DAxis {
            get {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 o);
                return o;
            }
        }

        /// <summary>
        /// True if the trigger button is being pressed
        /// </summary>
        public bool triggerButton {
            get {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool o);
                return o;
            }
        }

        /// <summary>
        /// True on the frame the trigger-button is pressed down
        /// </summary>
        public bool triggerDown
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool o);
                return o && !lastTrigger;
            }
        }

        /// <summary>
        /// True on the frame the trigger-button is released
        /// </summary>
        public bool triggerUp
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool o);
                return !o && lastTrigger;
            }
        }

        /// <summary>
        /// True if the menu-button is pressed
        /// </summary>
        public bool menuButton
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool o);
                return o;
            }
        }

        /// <summary>
        /// True on the frame the menubutton is pressed down
        /// </summary>
        public bool menuButtonDown
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool o);
                return o && ! lastMenuButton;
            }
        }

        /// <summary>
        /// True on the frame the menubutton is released
        /// </summary>
        public bool menuButtonUp
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool o);
                return !o && lastMenuButton;
            }
        }

        /// <summary>
        /// True if the grip is pressed
        /// </summary>
        public bool grip
        {
            get {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool o);
                return o;
            }
        }

        /// <summary>
        /// True on the frame the grip is pressed down
        /// </summary>
        public bool gripDown
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool o);
                return o && !lastGrip;
            }
        }

        /// <summary>
        /// True on the frame the grip is released
        /// </summary>
        public bool gripUp
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool o);
                return !o && lastGrip;
            }
        }

        /// <summary>
        /// True if the primary button is pressed
        /// </summary>
        public bool primaryButton
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool o);
                return o;
            }
        }

        /// <summary>
        /// True on the frame the primary button is pressed
        /// </summary>
        public bool primaryButtonDown
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool o);
                return o && !lastPrimaryButton;
            }
        }

        /// <summary>
        /// True omn the frame the primary button is released
        /// </summary>
        public bool primaryButtonUp
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool o);
                return !o && lastPrimaryButton;
            }
        }

        /// <summary>
        /// True if the primary button is pressed
        /// </summary>
        public bool secondaryButton
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool o);
                return o;
            }
        }

        /// <summary>
        /// True on the frame the primary button is pressed
        /// </summary>
        public bool secondaryButtonDown
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool o);
                return o && !lastSecondaryButton;
            }
        }

        /// <summary>
        /// True omn the frame the primary button is released
        /// </summary>
        public bool secondaryButtonUp
        {
            get
            {
                controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool o);
                return !o && lastSecondaryButton;
            }
        }

        private bool lastTrigger = false;
        private bool lastMenuButton = false;
        private bool lastAxisClick = false;
        private bool lastGrip = false;
        private bool lastPrimaryButton = false;
        private bool lastSecondaryButton = false;

        protected void LateUpdate()
        {
            controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out lastTrigger);
            controller.inputDevice.TryGetFeatureValue(CommonUsages.menuButton, out lastMenuButton);
            controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out lastAxisClick);
            controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out lastGrip);
            controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out lastPrimaryButton);
            controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out lastSecondaryButton);
        }

        /// <summary>
        /// Returns the status of a given BoolEvent
        /// </summary>
        public bool getEventStatus(BoolEvent type)
        {
#if AVR_NET
            if(!IsOwner) return false;
#endif
            switch (type)
            {
                case BoolEvent.PRIMARY2DAXIS_CLICK : { return primary2DAxisClick; }
                case BoolEvent.PRIMARY2DAXIS_CLICK_LEFT: { return primary2DAxisClick && primary2DAxis.x < -0.5; }
                case BoolEvent.PRIMARY2DAXIS_CLICK_RIGHT: { return primary2DAxisClick && primary2DAxis.x > 0.5; }
                case BoolEvent.PRIMARY2DAXIS_CLICK_LEFTORRIGHT: { return primary2DAxisClick && Mathf.Abs(primary2DAxis.x) > 0.5; }
                case BoolEvent.PRIMARY2DAXIS_CLICK_MIDDLE: { return primary2DAxisClick && Mathf.Abs(primary2DAxis.x) < 0.5; }
                case BoolEvent.PRIMARY2DAXIS_CLICKDOWN: { return primary2DAxisClickDown; }
                case BoolEvent.PRIMARY2DAXIS_CLICKDOWN_LEFT: { return primary2DAxisClickDown && primary2DAxis.x < -0.5; }
                case BoolEvent.PRIMARY2DAXIS_CLICKDOWN_RIGHT: { return primary2DAxisClickDown && primary2DAxis.x > 0.5; }
                case BoolEvent.PRIMARY2DAXIS_CLICKDOWN_LEFTORRIGHT: { return primary2DAxisClickDown && Mathf.Abs(primary2DAxis.x) > 0.5; }
                case BoolEvent.PRIMARY2DAXIS_CLICKDOWN_MIDDLE: { return primary2DAxisClickDown && Mathf.Abs(primary2DAxis.x) < 0.5; }
                case BoolEvent.PRIMARY2DAXIS_TOUCH: { return primary2DAxisTouch; }
                case BoolEvent.PRIMARY2DAXIS_TOUCH_LEFT: { return primary2DAxisTouch && primary2DAxis.x < -0.5; }
                case BoolEvent.PRIMARY2DAXIS_TOUCH_RIGHT: { return primary2DAxisTouch && primary2DAxis.x > 0.5; }
                case BoolEvent.PRIMARY2DAXIS_TOUCH_LEFTORRIGHT: { return primary2DAxisTouch && Mathf.Abs(primary2DAxis.x) > 0.5; }
                case BoolEvent.PRIMARY2DAXIS_TOUCH_MIDDLE: { return primary2DAxisTouch && Mathf.Abs(primary2DAxis.x) < 0.5; }
                case BoolEvent.TRIGGER_TRIGGER: { return triggerButton; }
                case BoolEvent.TRIGGER_ONTRIGGERDOWN: { return triggerDown; }
                case BoolEvent.TRIGGER_ONTRIGGERUP: { return triggerUp; }
                case BoolEvent.MENUBUTTON: { return menuButton; }
                case BoolEvent.MENUBUTTON_DOWN: { return menuButtonDown; }
                case BoolEvent.MENUBUTTON_UP: { return menuButtonUp; }
                case BoolEvent.GRIP_GRIP: { return grip; }
                case BoolEvent.GRIP_ONGRIPDOWN: { return gripDown; }
                case BoolEvent.GRIP_ONGRIPUP: { return gripUp; }
                case BoolEvent.ANY_CANCEL: { return menuButtonDown || triggerDown || primary2DAxisClick; }
                case BoolEvent.ALWAYS_TRUE: { return true; }
                case BoolEvent.ALWAYS_FALSE: { return false; }
                case BoolEvent.PRIMARY_BUTTON: { return primaryButton; }
                case BoolEvent.SECONDARY_BUTTON: { return secondaryButton; }
                case BoolEvent.PRIMARY_BUTTON_DOWN: { return primaryButtonDown; }
                case BoolEvent.SECONDARY_BUTTON_DOWN: { return secondaryButtonDown; }
                case BoolEvent.PRIMARY_BUTTON_UP: { return primaryButtonUp; }
                case BoolEvent.SECONDARY_BUTTON_UP: { return secondaryButtonUp; }
                default : { AVR_DevConsole.cwarn("getEventStatus does not recoginze value "+type, this); break; }
            }
            return false;
        }

        /// <summary>
        /// Returns the status of a given FloatEvent
        /// </summary>
        public float getEventStatus(FloatEvent type)
        {
            switch (type)
            {
                case FloatEvent.PRIMARY2DAXIS_X : { return primary2DAxis.x; }
                case FloatEvent.PRIMARY2DAXIS_Y: { return primary2DAxis.y; }
                default: { AVR_DevConsole.cwarn("getEventStatus does not recoginze value " + type, this); break; }
            }
            return -1.0f;
        }

        /// <summary>
        /// Returns the status of a given Vec2Event
        /// </summary>
        public Vector2 getEventStatus(Vec2Event type)
        {
            switch (type)
            {
                case Vec2Event.PRIMARY2DAXIS : { return primary2DAxis; }
                default: { AVR_DevConsole.cwarn("getEventStatus does not recoginze value " + type, this); break; }
            }
            return Vector2.zero;
        }
    }
}
