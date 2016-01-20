using UnityEngine;
using System.Collections;

// this class controls the Window objects and allows them to be selectable
public class MRWindowController : MonoBehaviour {

    Vector3 defaultPosition; // the default and original position of this object
    public GameObject selectedPositionMarker; // a gameobject used to mark the position the object will move to
    Vector3 selectedPosition; // the position Vector3 that we pull out of selectedPositionMarker

    // STATE //
    int state;

    int STATE_DESELECTED = 0; // we aren't currently selected
    int STATE_SELECTED = 1; // we are currently selectted
    // //

    public bool justSelected = false; // we just received a message that we have been selected this frame

    const float FORWARD_TIME = 2.5f; // the time our window will spend in the "selected position" once it is selected
    const float MOVEMENT_SPEED = 2.0f; // the speed at which the window moves at
    const float DISTANCE_FUZZ = 0.01f; // a fuzz constant for when we are "close enough" to the selected position

    float forwardTimer = 0.0f; // the variable used to record how much time we have been in the "selected position"

	// Use this for initialization
	void Start () {
        defaultPosition = transform.position; // populate defaultPosition with our current position
        selectedPosition = selectedPositionMarker.transform.position; // populate selectedPosition with the selectedPositionMarker object
    }
	
	void Update () {

        // state management
        if (justSelected) // if we got selected this frame, enter the STATE_SELECTED state
        {
            forwardTimer = 0.0f;
            justSelected = false;
            state = STATE_SELECTED;
        }

        if (forwardTimer >= FORWARD_TIME) // if we've been in the STATE_SELECTED state long enough, go back to the STATE_DESELECTED state
        {
            state = STATE_DESELECTED;
        }

        // state execution
        if (state == STATE_SELECTED)
        {
            if (Vector3.Distance(transform.position, selectedPosition) >= DISTANCE_FUZZ) // if we haven't reached our destination, keep going there
            {
                transform.position = Vector3.Lerp(transform.position, selectedPosition, MOVEMENT_SPEED * Time.deltaTime); // lerp towards our destination
            }
            else
            {
                forwardTimer += Time.deltaTime;
            }
        }
        else if( state == STATE_DESELECTED)
        {
            if (Vector3.Distance(transform.position, defaultPosition) >= DISTANCE_FUZZ)
            {
                transform.position = Vector3.Lerp(transform.position, defaultPosition, MOVEMENT_SPEED * Time.deltaTime); // lerp back towards the default position
            }
        }


    }
}
