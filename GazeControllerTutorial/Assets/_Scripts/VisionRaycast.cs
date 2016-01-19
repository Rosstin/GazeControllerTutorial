using UnityEngine;
using System.Collections;

public class VisionRaycast : MonoBehaviour {

    public Camera playerCamera; // holds a reference to the player camera

    public GameObject cursor; // holds a reference to a cursor object to be drawn where the player's gaze points
    public PointerSelectionController cursorScript; // holds a reference to the cursor's script

	void Start () {
        cursorScript = cursor.GetComponent<PointerSelectionController>();
    }

    void Update () {
        RaycastHit hit = new RaycastHit(); // this object will collect data about the collision each frame

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit)) // do the raycast, based on the camera's position and orientation, and store the hit for our reference
        {
            cursorScript.setVisible(true); // the cursor has a script that depends on update, we want that to run regardless of whether or not it's invisible
            cursor.transform.position = hit.point; // place the cursor where the collision is happening
            cursorScript.hittingSomething = true; // tell the cursor that it's hitting something
        }
        else // there wasn't any collision
        {
            cursorScript.setVisible(false); // deactivate the cursor if it was active
            cursorScript.hittingSomething = false; // tell the cursor it's not hitting anything
        }
    }
}
