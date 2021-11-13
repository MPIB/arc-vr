using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AVR.Core;
using AVR.Phys;

public class UrnExperiment : AVR_TrialExperiment
{
    public GameObject guess_ui;
    public GameObject urn;
    public TMPro.TMP_Text instruction_text;

    public GameObject start_ui;
    public GameObject end_ui;
    public AVR_BasicGrabProvider grabProv;

    private bool drew_red_sphere = false;

    public override IEnumerator trial()
    {
        // Guess a color
        showGuessUI();
        yield return new WaitUntil(can_proceed);

        // Draw a Ball from the Urn, then wait until it is released.
        showUrn();
        yield return new WaitUntil(ball_has_been_drawn);
        yield return new WaitForSeconds(2); // A small delay
        yield return new WaitUntil(() => !ball_has_been_drawn());
        yield return new WaitForSeconds(1); // A small delay to let the ball roll off
        urn.SetActive(false);

        // Display a confirmation message
        showConfirmationMessage();
        yield return new WaitForSeconds(3);
    }

    public override void on_start()
    {
        base.on_start();
        start_ui.SetActive(false);
    }

    public override void on_end()
    {
        base.on_end();
        end_ui.SetActive(true);
        instruction_text.text = "You've reached the end of the experiment.";
    }

    private bool ball_has_been_drawn()
    {
        if (grabProv.getGrabbedObject() != null)
        {
            if(grabProv.getGrabbedObject().GetComponent<MeshRenderer>().material.color.r > 0.1f)
            {
                drew_red_sphere=true;
            }
            else
            {
                drew_red_sphere=false;
            }
            return true;
        }
        return false;
    }

    private void showGuessUI()
    {
        urn.SetActive(false);
        guess_ui.SetActive(true);
        instruction_text.text = "Guess the color you will draw.";
    }

    private void showUrn()
    {
        urn.SetActive(true);
        guess_ui.SetActive(false);
        instruction_text.text = "Draw an sphere from the urn.";
    }

    private void showConfirmationMessage()
    {
        urn.SetActive(false);
        guess_ui.SetActive(false);
        instruction_text.text = "You drew a " + (drew_red_sphere ? "red" : "blue") + " sphere.";
    }
}
