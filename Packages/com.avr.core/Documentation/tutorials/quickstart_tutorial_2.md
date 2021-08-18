@logo

# Step 4: Base VR Rig

Next up, we'll want to create a VR PlayerRig that represents the player (HMD + controllers) in our virtual space.

Creating a PlayeRig with arc-vr is easy. Simply click on `AVR > Create Player Rig` in the top menu bar. Alternatively, you can navigate to `Packages/arc-vr-core/Editor/DefaultPrefabs` and drag-&-drop a PlayerRig object into your scene.

The PlayerRig includes a Camera object tagged as MainCamera. In order to not get Unity confused, its best to only have one single Camera object tagged as the main camera. Thus, delete the default "MainCamera" GameObject from your scene.

If you now click on 'Play', you should be able to view your scene through your VR headset.

\note Make sure 'Initialize XR on Startup' is enabled, under `Project Settings > XR Plugin Management`, or the scene won't launch in VR mode.

# Step 5: VR Controllers

We are now able to view our scene through our tracked HMD. As a next step, we ought to include the Vive controllers, representing the left and right hands of the player.

## Adding the controllers

Adding left/right controllers to our PlayerRig is simple.

- Select the 'PlayerRig' gameobject.
- In the inspector, click on the 'Add Module' button.
- In the opened dialogue window, tick 'Left Controller' and 'Right Controller'.
- Click on 'Update'.

Your PlayerRig gameobject should now have three child objects: 'Camera Offset' (represents the HMD), as well as 'LeftHandController' and 'RightHandController' (represent the left/right controllers).

\note This workflow pattern is common in arc-vr. An object will have a selection-wizard for a set of modules, and those modules will be added as child-objects. You can insert your own modules into these dialogue-windows. For more info, check the docs on \ref AVR.UEditor.Core.AVR_HookableWizard<T> "AVR_HookableWizard" and \ref AVR.UEditor.Core.AVR_WizardHook<Wiz> "AVR_WizardHook<Wiz>".

## Adding a controller visual and inputmanager

We now have two gameobjects that represent the tracked VR controllers. However, we can't see them in our virtual space, as they simply are empty transforms.

To add a rendered mesh to make our controller visible, individually select each controller gameobject and click on 'Add Module', tick 'ControllerVisual' and 'InputManager', then click Update.

If you launch the scene now, you should be able to see both controllers through your headset.

\note The "InputManager" module is *not* required to make the controller visible. You only need an InputManager if you are planning to use inputs (ie the buttons on the controller). We'll be using these in future steps, so may as well add them now.

\htmlonly\endhtmlonly

\note More selectable modules will appear in these dialogue windows once you import more arc-vr packages.

\htmlonly\endhtmlonly

\note Each \ref AVR.Core.AVR_Component "AVR_Component" script displays 2-3 buttons within the editor:
![img](../../res/images/core_editorButtons.jpg)
- *Documentation Link*: Will open up the respective documentation page in your browser.
- *Custom Monobehaviour Events*: Will let you set UnityEvents that are executed when the Monobehaviour Awake, Start, OnEnable or OnDisable are called. You can use this, for instance, to maintain two modules mutually exclusive. For example, you might not want to let the player teleport/move whilst interacting with UI. Then you would simple add an OnEnable event onto your UIInteractionProvider that disables the MovementProvider, and vice-versa for OnDisable.
- *Network Behaviour*: This button is only present if the [arc-vr-net](@ref arc-vr-net) package is included in the project. It gives you some useful tools for how components ought to behave when on a network.

Continue in [tutorial 3](tutorials/quickstart_tutorial_3.md).
