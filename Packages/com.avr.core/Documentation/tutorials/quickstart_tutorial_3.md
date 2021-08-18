@logo

# Step 6: Importing additional packages

We now have a rudimentary VR rig with HMD and both controllers. As a next step we'll allow the player to pick up and interact with physical objects.

We're going to import two additional packages for this: [arc-vr-motion](@ref arc-vr-motion) and [arc-vr-phys](@ref arc-vr-phys).

Just as with arc-vr-core, download the packages from github and install them with the package manager.

# Step 7: Adding Modules

## Letting the player move around

Theoretically, we could simply position the VR Rig in front of the table, which would make moving around unnecessary.

However, to keep things simple and for demonstration purposes, we'll let the player move around the scene.

Select the controller you want to handle movement, I'd suggest the left hand controller. Click on 'Add Module' and select 'MovementProvider' as well as 'TurnProvider'. These two modules essentially 'provide' movement and turning functionality to the controller they are attatched to.

You can take a gander at the newly created child-objects and play around with the settings. If you're looking for more details on these modules, look at the @ref AVR.Motion.AVR_MovementProvider "MovementProvider" and @ref AVR.Motion.AVR_TurnProvider "TurnProvider" docs.

## Letting the player interact with objects

The goal is to let the player draw colored spheres from an urn. For this we want to allow the player to grab/interact with rigidbody objects.

Just as with the step before, select the controller that will handle physics interactions (I'd suggest the right-handed one) and add a 'GrabProvider' module. There are several types of grab providers. For this project the 'Basic' type will suffice.

If you take a quick look at the newly created 'BGrabProvider' object, you'll see it comes with two children:

- A 'GrabZone', that defines a volume from which the GrabProvider will select the nearest object when a grab is provided.
- As well as a 'GrabSpot'. A grabbed object's center will be pulled towards this GrabSpot. If you want to grab an object at another location than its center (example: grabbing a stick by either end) you'll need to employ @ref AVR.Phys.AVR_OffsetGrabProvider "a different type of grabprovider".

# Step 8: Creating the Scene

Our player is now able to grab and interact with objects, but we haven't given them any objects to interact with yet.

As a next step we'll expand our scene to include a table with an urn on top of it. You can find the 3d model of the urn [here](www.TODO.com) for download. (For the table you can just use a re-scaled cube if necessary).

## Filling the urn

Now we've got an urn we next need to fill it with colored balls.

Create a 3d sphere and resize it to an adequate size (I've used 0.1 for all three axes). Place it so that it hovers above the urn's cavity.

To turn the sphere into a phyiscs object, add a rigidbody component to it. Set collision detection to 'continuous speculative'*. Additionally, whilst not actually necessary, it's generally good practice to use realistic mass values for rigidbodies, so set its' mass to about 0.1 (100g).

\note * Any type collision detection will do, but in this particular case the balls tend to leak through the urn's walls on different collision detection methods.

Finally, we'll have to turn the rigidbody into a grabbable object. Simply select the gameobject that has the rigidbody attatched and add an 'AVR_Grabbable' component. You don't need to bother with any of its' properties except for 'Object Type'. If you try to set 'Object Type, you'll find that there are several pre-configured scriptable objects available, which determine how the object behaves when grabbed. For a small sphere, 'VeryLightProp' works the best. If you don't set this field, the AVR_Grabbable will default to a preset type.

Only step left is to give the sphere an adequate color. Create two new materials, one red the other blue and apply one of them to the sphere. Duplicate the sphere and apply the other material to that.

Now you'll only have to duplicate both spheres multiple times and place them above the urn.

## Test it out

If you now launch your scene, you should be able to move up to the table using the left controller, then grab a sphere out of the urn using the right controller.

TODO: video
