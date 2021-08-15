@logo

# Step 9: Adding UI Interaction

import arc-vr-ui

add UIInteractionProvider to left controller
	- select UIInteractionprovider
	- do the FIXME thing

use console to find window
	- add window
	- find button prefab
	- add 3 buttons
	- "Start", "Guess Blue", "Guess Red"


# Step 10: Setting up a TrialExperiment

DIRECTOR

Create new Script "UrnExperiment"
	- Have it inherit from AVR.Core.AVR_TrialExperiment

Add following code:
```
    public GameObject urn;
    public GameObject guess_ui;

    public override IEnumerator trial() {
        urn.SetActive(false);
        guess_ui.SetActive(true);

        yield return new WaitUntil(can_proceed);

        urn.SetActive(true);
        guess_ui.SetActive(false);

        yield return new WaitForSeconds(5);
    }
```

Put this into scene

# Step 11: Finishing Up

Put urn and balls under one object

Put Green and Blue buttons under one object

Assign both to the UrnExperiment script

Assign button actions:
	- Start -> UrnExperiment.commence
	- Green and Blue -> UrnExperiment.proceed()

# Step 12: Data Collection

Add AVR_Logger with 2 columns
	- Time
	- Target: BGrabProvider, Field: grabbedObject
