using UnityEngine;
using System.Collections;

public class VisionRaycast : MonoBehaviour {

    public Camera playerCamera; // holds a reference to the player camera

    public GameObject cursor; // holds a reference to a cursor object to be drawn where the player's gaze points

	void Start () {
    }

    void Update () {
        RaycastHit hit = new RaycastHit(); // this object will collect data about the collision each frame

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit)) // do the raycast, based on the camera's position and orientation, and store the hit for our reference
        {
            cursor.SetActive(true); // activate the cursor if it was inactive
            cursor.transform.position = hit.point; 
        }
        else // there wasn't any collision
        {
            cursor.SetActive(false); // deactivate the cursor if it was active
        }
    }
}
