@logo

# Step 9: Adding UI Interaction

We want the player to guess which color they'll draw *before* reaching into the urn. We'll do this by presenting the player with an in-game UI window, for which we'll need the [arc-vr-ui](@ref arc-vr-ui) package.

The installation procedure shoule be familiar by now: download the .tgz from github, import it via the package manager.

The subsequent steps should be familiar as well: select the controller you want to act as a UI pointer, click 'Add Module' and select 'UIInteractionProvider'.

If you now select the UIInteractionProvider, you might notice the warning it displays. As with any Unity-related UI system, an 'EventSystem' is required. If you click the 'Fix now' button, a new EventSystem object will be created (together with an attatched VRInput component).

The selected controller is now fully functional as a UI pointer, which will work with any world-space unity canvas.

## Adding UI elements

The arc-vr-ui package comes with several pre-fabricated UI elements, which we'll make use of in the next steps. There are seveal options to find what we're looking for:

- Use the Project explorer to navigate to `Packages > arc-vr-ui > Runtime > Prefabs` and find 'GenericWindow_Borderless'.
- Type 'window' into the project explorer search-bar, then filter only by prefabs and set the search to 'In Packages', rather than 'In Assets'. Find the GenericWindow_Borderless prefab.
- Run the command `prefabs ui window` in the developers console. This will set your project explorer search to look for the word 'window' inside the arc-vr-ui package. Find the GenericWindow_Borderless prefab.

Once you've found it, drag the 'GenericWindow_Borderless' into scene and place it in front of the table.

Next up we'll need some buttons. Use any of the methods above to search for button. You should find a simple 'Button_large' prefab. Drag three of them onto the GenericWindow, place them one above the other and change their text to "Guess Blue", "Guess Red" and "Start" respectively.

We now have all the elements of our scene ready. The buttons won't actually do anything as of right now, but they'll be clickable with a controller.

# Step 10: Setting up a TrialExperiment

For this next step we'll be using the @ref AVR.Core.AVR_TrialExperiment "TrialExperiment" class to create a trial-based structure to our experiment.
A 'trial' will follow the following procedure:

- Hide the urn + balls, display the UI buttons
- If the player clicks one of the guess buttons, hide the UI and show the urn
- After a couple of seconds, return to step 1.

The implementation is simple. Create a new script (call it something like `UrnExperiment`) and make it inherit from AVR.Core.AVR_TrialExperiment. The TrialExperiment class is a simple one that models trials as Coroutines.
We'll simply override the `trial()` function with the above listed procedure. Add this code inside your class:

    public GameObject urn;
    public GameObject guess_ui;

    public override IEnumerator trial() {
        // Disable urn, enable UI
        urn.SetActive(false);
        guess_ui.SetActive(true);

		// Wait for next step
        yield return new WaitUntil(can_proceed);

		// Enable urn, disable UI
        urn.SetActive(true);
        guess_ui.SetActive(false);

		// Wait a couple of seconds
        yield return new WaitForSeconds(5);
    }

Now attach this script to some fitting gameobject.

# Step 11: Finishing Up

As a last step, we'll just have to connect all the dots.

If you haven't already, parent the urn and all the balls under a single gameobject, so we can easily toggle them on and off.

Then select your 'UrnExperiment' script we created in the previous step and assign the 'urn' property to be the parent object of the urn and balls.
Assign the 'guess_ui' property to the GenericWindow that includes the three buttons.

Select each of the buttons, and add an 'OnClick' event. For the 'Guess Blue' / 'Guess Red' buttons set the OnClick event to be `UrnExperiment.proceed()`.
For the 'Start' button, set it to `UrnExperiment.commence`, with the amount of trials as the argument (eg. 10).

If you now click play, the project should be complete. Once you click the 'Start' button, the urn will be hidden. It will then only show once you've guessed a color.
You will then have 5 seconds to draw a sphere before the process begins again.

# Step 12: Data Collection

If we wish to record and collect data on the experiment, we can use the provided @ref AVR.Core.AVR_Logger "AVR_Logger" class.

Simply select any gameobject (for instance the same one that has the 'UrnExperiment' script) and add an AVR_Logger component to it.

We're going to keep things simple, and create a basic csv-table with two columns: time and the corresponding object the player is grabbing.

Add a new entry to the 'Columns' property, set the label to 'time' and select the type as 'TIME'.
Add a second entry, set the label to something like 'object' and leave the type as 'CUSTOM'.

The AVR_Logger uses C# reflection to extract properties from certain objects, including private or protected ones. Any value exposed in a property is thus simple to log with this.
In our case, we want to know what the player is holding on to, so it seems logical to look for an appropriate field in our BasicGrabProvider script. If you look at the docs for @ref AVR.Phys.AVR_BasicGrabProvider "BasicGrabProvider" you'll see it has a protected field called 'grabbedObject', which corresponds to the AVR_Grabbable the player is holding.

To log this field, simply drag and drop the BasicGrabProvider into the 'Target' field of the newly created column and set 'Field' to be `grabbedObject`. Leave read type as 'AUTO'.

If you now launch the scene again, there'll be a `logs/sample.log` file which includes a timestamp as well as the name of the object the player is holding at that time.
