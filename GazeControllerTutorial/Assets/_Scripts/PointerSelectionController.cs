using UnityEngine;
using System.Collections;

public class PointerSelectionController : MonoBehaviour
{

    public GameObject[] tickOclockWhite; // holds the 12 tick marks representing empty
    public GameObject[] tickOclockRed; // holds the 12 tick marks representing full

    public GameObject componentsFolder; // holds the visible components of the cursor so we can activate/deactivate them easily

    public bool hittingSomething = false; // set by VisionRaycast every frame so that we know if we're hitting something

    // STATE STUFF //
    public int state; // simple state

    public const int STATE_NOSELECTION = 0; // nothing is selected
    public const int STATE_SELECTED = 2; // something is selected
    //             //

    public const float SELECTION_TIME = 2.0f; // the time it takes to select/deselect an object

    public float elapsedTime = 0.0f; // holds the fullness of the timer

    public float fractionComplete = 0.0f; // other scripts can see how complete the timer is

    public void initializePointerSelectionController()
    {
        hittingSomething = false;
        elapsedTime = 0.0f;
        state = STATE_NOSELECTION;
    }

    void Start()
    {
        initializePointerSelectionController();
    }

    void Update()
    {
        float t = elapsedTime / SELECTION_TIME; // holds the fraction of timer completeness
        fractionComplete = t; // for reference by outside objects/scripts

        if (state == STATE_NOSELECTION) // if nothing is selected
        {
            if (hittingSomething)
                elapsedTime += Time.deltaTime; // if you're hitting something, slowly fill the timer
            else
                elapsedTime = 0.0f; // if you're not hitting anything, zero the timer

            if (t >= 1.00)
            {
                state = STATE_SELECTED; // switch state to STATE_SELECTED if the timer fills up
            }
        }
        else if (state == STATE_SELECTED) // if something is selected
        {
            if (hittingSomething)
                elapsedTime = SELECTION_TIME; // if you're hitting something, set timer to full
            else
                elapsedTime -= Time.deltaTime; // if you're not hitting anything, slowly empty the timer

            if (t <= 0.00)
            {
                state = STATE_NOSELECTION; // if the timer empties completely, switch states
            }
        }

        setClockFace(); // keep the clock face updated
    }

    public void setVisible(bool visibility) // make your components invisible (but keep running your own update loop)
    {
        componentsFolder.SetActive(visibility);
    }

    void setClockFace() // turn on/off the clock tick marks based on time 
    {
        float t = elapsedTime / SELECTION_TIME;

        if (t >= 12.0 / 12.0) { tickOclockWhite[11].SetActive(false); tickOclockRed[11].SetActive(true); }
        else { tickOclockWhite[11].SetActive(true); tickOclockRed[11].SetActive(false); }

        if (t >= 11.0 / 12.0) { tickOclockWhite[10].SetActive(false); tickOclockRed[10].SetActive(true); }
        else { tickOclockWhite[10].SetActive(true); tickOclockRed[10].SetActive(false); }

        if (t >= 10.0 / 12.0) { tickOclockWhite[9].SetActive(false); tickOclockRed[9].SetActive(true); }
        else { tickOclockWhite[9].SetActive(true); tickOclockRed[9].SetActive(false); }

        if (t >= 9.0 / 12.0) { tickOclockWhite[8].SetActive(false); tickOclockRed[8].SetActive(true); }
        else { tickOclockWhite[8].SetActive(true); tickOclockRed[8].SetActive(false); }

        if (t >= 8.0 / 12.0) { tickOclockWhite[7].SetActive(false); tickOclockRed[7].SetActive(true); }
        else { tickOclockWhite[7].SetActive(true); tickOclockRed[7].SetActive(false); }

        if (t >= 7.0 / 12.0) { tickOclockWhite[6].SetActive(false); tickOclockRed[6].SetActive(true); }
        else { tickOclockWhite[6].SetActive(true); tickOclockRed[6].SetActive(false); }

        if (t >= 6.0 / 12.0) { tickOclockWhite[5].SetActive(false); tickOclockRed[5].SetActive(true); }
        else { tickOclockWhite[5].SetActive(true); tickOclockRed[5].SetActive(false); }

        if (t >= 5.0 / 12.0) { tickOclockWhite[4].SetActive(false); tickOclockRed[4].SetActive(true); }
        else { tickOclockWhite[4].SetActive(true); tickOclockRed[4].SetActive(false); }

        if (t >= 4.0 / 12.0) { tickOclockWhite[3].SetActive(false); tickOclockRed[3].SetActive(true); }
        else { tickOclockWhite[3].SetActive(true); tickOclockRed[3].SetActive(false); }

        if (t >= 3.0 / 12.0) { tickOclockWhite[2].SetActive(false); tickOclockRed[2].SetActive(true); }
        else { tickOclockWhite[2].SetActive(true); tickOclockRed[2].SetActive(false); }

        if (t >= 2.0 / 12.0) { tickOclockWhite[1].SetActive(false); tickOclockRed[1].SetActive(true); }
        else { tickOclockWhite[1].SetActive(true); tickOclockRed[1].SetActive(false); }

        if (t >= 1.0 / 12.0) { tickOclockWhite[0].SetActive(false); tickOclockRed[0].SetActive(true); }
        else { tickOclockWhite[0].SetActive(true); tickOclockRed[0].SetActive(false); }
    }

}

