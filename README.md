# GazeControllerTutorial
tutorial for controlling motion in VR with the player's gaze

https://youtu.be/nTPLH2-lAig

A key component in VR control-schemes will be gaze-based controls. You're always going to want to know where the user's face is pointing. 
In this tutorial, I'll demonstrate how to raycast the user's gaze. We'll draw a pointer object where the user is looking and you'll be able to use that to select objects. 

All the code for this tutorial is hosted on my Github for your reference: https://github.com/Rosstin/GazeControllerTutorial

Here's also a great article about Unity Git Source Control if you need that: http://gamasutra.com/blogs/AlistairDoulin/20150304/237814/Git_for_Unity_Developers.php

This tutorial demonstrates my implementation of this feature using Unity3D and C#. I'm going to assume you've already done the Roll-A-Ball VR tutorial or similar: https://developer.oculus.com/documentation/game-engines/latest/concepts/unity-tutorial-rollaball-intro/

STARTING THE PROJECT AND BUILDING A ROOM

Make a new 3D Project without any modules called GazeControllerTutorial.

Save your scene as MinorityReportRoom, make a folder called _Scenes, and put it in there.

Make a quick little room for your player so we have something to look at. My long-term project is to create a crazy giant screen interface like the one in Minority Report, so I made an object called MasterMRScreen as a giant viewscreen. Then I made a Floor, and a couple Pillars just to add some color to the room, and positioned the camera in the center of our room. I made a few materials to give the objects some color too.

Now go to File - Build Settings - Player Settings... - Virtual Reality Supported

Assuming you've already setup your Oculus Rift and its connected, you should be set to control your scene in VR.

If you want to download the project from this state, you can download from the commit: https://github.com/Rosstin/GazeControllerTutorial/commit/b6ec452af23be709871adf3206de3b447e3207a0

RAYCASTING FROM THE PLAYER'S FACE

Now it's time to write some code. What we need is to build a cursor object that follows the user's gaze.

I make a new script called "VisionRaycast" and attach it to the main camera, and put the script in a _Scripts folder.

What we're going to do, is we're going to draw a Raycast from the player camera (right between the eyes of the human wearing the headset). That raycast is going to hit whatever object the player is looking at.

Make a "public Camera playerCamera" to hold a reference to the camera object.

In Update(), declare Raycast hit = new RaycastHit(). This object will grab important data about what the raycast is colliding with each frame.

<code><pre>using UnityEngine;
using System.Collections;

public class VisionRaycast : MonoBehaviour {

    public Camera playerCamera; // holds a reference to the player camera

	void Start () {
	    
	}
	
	void Update () {
        RaycastHit hit = new RaycastHit(); // this object will collect data about the collision each frame

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit)) // do the raycast, based on the camera's position and orientation, and store the hit for our reference
        {
            Debug.Log("We are hitting something!"); // the raycast collided with an object
        }
        else // there wasn't any collision
        {
            Debug.Log("Not hitting anything.");
        }
    }
}
</pre></code>

Now you should be able to run the project and, if you're looking at the big purple screen, you'll get a message "We are hitting something!" If you look away, you'll get the message "Not hitting anything."

https://opensourcevrblog.files.wordpress.com/2016/01/002hitting.png

From here, you can grab https://github.com/Rosstin/GazeControllerTutorial/commit/0f7b3fa02b6e2c07dab861842a346c455a2476ba to get up to speed.

DRAWING THE RAYCAST HIT

Now, let's put a marker in the position where the raycast hit is colliding with an object. Make a small sphere "Cursor" GameObject.

Now put a reference to the cursor in your VisionRaycast script, and populate that script field with your cursor.

Rewrite your script as so, cursor.transform.position = hit.point, to draw the cursor at the location of the player's.

<code><pre>using UnityEngine;
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
</pre></code>

Woah! Warning, if you run the game now, the cursor will continuously fly at the screen like crazy! Very disorienting. It's doing this because VisionRaycast is registering the cursor itself as a collidable object. Let's deactivate our cursor's Sphere Collider.

Now you can see that the cursor is able to collide with some objects and demonstrate to you where the player is looking.

https://opensourcevrblog.files.wordpress.com/2016/01/003pointer.png

Let's put a few boxes on the MasterMRScreen so there's something we can point at and select.

https://opensourcevrblog.files.wordpress.com/2016/01/005windows.png

You can grab the code up to this point from here https://github.com/Rosstin/GazeControllerTutorial/commit/5999ec4b0b8ea5cce5db6475afe2e5235881e22a

BUILDING A DYNAMIC CURSOR

So, now the cursor is being drawn where the player is looking, but we want the cursor to be more dynamic. Let's make the cursor more reactive, and build a clock-face interface around the cursor. The gist of this, is the clock face we build is going to demonstrate the completion percentage of the selection action. Once the clock hits 12oclock, the selection action will be complete.

https://opensourcevrblog.files.wordpress.com/2016/01/009clock.png

I built a clock face around the cursor with 12 long cubes. Unless you want to skip ahead and grab the next repo snapshot, you're going to have to do some sine/cosine geometry... I make a set of white clock hands and a set of red clock hands for the script to turn on and off. Don't forget to turn off the colliders for these "clock hands".

For the PointerSelectionController code, I built a very simple state machine that flips between 2 modes.

STATE_NOSELECTION is for when the user hasn't selected any object.

STATE_SELECTED indicates that the user has selected an object.

In the update loop... when you're in STATE_NOSELECTION and told you're hitting an object, fill up the timer. If the player stops looking at an object, empty the timer. When the timer fills up completely, change to STATE_SELECTED.

When you're in STATE_SELECTED, do the opposite for deselecting the object.

Then we have a "setClockFace()" function to flip our clock hands on and off based on the timer.

<code><pre>
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

            if (t &gt;= 1.00)
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

            if (t &lt;= 0.00)             {                 state = STATE_NOSELECTION; // if the timer empties completely, switch states             }         }         setClockFace(); // keep the clock face updated     }     public void setVisible(bool visibility) // make your components invisible (but keep running your own update loop)     {         componentsFolder.SetActive(visibility);     }     void setClockFace() // turn on/off the clock tick marks based on time      {         float t = elapsedTime / SELECTION_TIME;         if (t &gt;= 12.0 / 12.0) { tickOclockWhite[11].SetActive(false); tickOclockRed[11].SetActive(true); }
        else { tickOclockWhite[11].SetActive(true); tickOclockRed[11].SetActive(false); }

        if (t &gt;= 11.0 / 12.0) { tickOclockWhite[10].SetActive(false); tickOclockRed[10].SetActive(true); }
        else { tickOclockWhite[10].SetActive(true); tickOclockRed[10].SetActive(false); }

        if (t &gt;= 10.0 / 12.0) { tickOclockWhite[9].SetActive(false); tickOclockRed[9].SetActive(true); }
        else { tickOclockWhite[9].SetActive(true); tickOclockRed[9].SetActive(false); }

        if (t &gt;= 9.0 / 12.0) { tickOclockWhite[8].SetActive(false); tickOclockRed[8].SetActive(true); }
        else { tickOclockWhite[8].SetActive(true); tickOclockRed[8].SetActive(false); }

        if (t &gt;= 8.0 / 12.0) { tickOclockWhite[7].SetActive(false); tickOclockRed[7].SetActive(true); }
        else { tickOclockWhite[7].SetActive(true); tickOclockRed[7].SetActive(false); }

        if (t &gt;= 7.0 / 12.0) { tickOclockWhite[6].SetActive(false); tickOclockRed[6].SetActive(true); }
        else { tickOclockWhite[6].SetActive(true); tickOclockRed[6].SetActive(false); }

        if (t &gt;= 6.0 / 12.0) { tickOclockWhite[5].SetActive(false); tickOclockRed[5].SetActive(true); }
        else { tickOclockWhite[5].SetActive(true); tickOclockRed[5].SetActive(false); }

        if (t &gt;= 5.0 / 12.0) { tickOclockWhite[4].SetActive(false); tickOclockRed[4].SetActive(true); }
        else { tickOclockWhite[4].SetActive(true); tickOclockRed[4].SetActive(false); }

        if (t &gt;= 4.0 / 12.0) { tickOclockWhite[3].SetActive(false); tickOclockRed[3].SetActive(true); }
        else { tickOclockWhite[3].SetActive(true); tickOclockRed[3].SetActive(false); }

        if (t &gt;= 3.0 / 12.0) { tickOclockWhite[2].SetActive(false); tickOclockRed[2].SetActive(true); }
        else { tickOclockWhite[2].SetActive(true); tickOclockRed[2].SetActive(false); }

        if (t &gt;= 2.0 / 12.0) { tickOclockWhite[1].SetActive(false); tickOclockRed[1].SetActive(true); }
        else { tickOclockWhite[1].SetActive(true); tickOclockRed[1].SetActive(false); }

        if (t &gt;= 1.0 / 12.0) { tickOclockWhite[0].SetActive(false); tickOclockRed[0].SetActive(true); }
        else { tickOclockWhite[0].SetActive(true); tickOclockRed[0].SetActive(false); }
    }

}
</pre></code>

Now in VisionRaycast.cs, let's tell the cursor when we're hitting an object. We'll grab a reference to the script and use that to set the "hittingSomething" boolean.

<code><pre>public class VisionRaycast : MonoBehaviour {

    public Camera playerCamera; // holds a reference to the player camera

    public GameObject cursor; // holds a reference to a cursor object to be drawn where the player's gaze points
    public PointerSelectionController cursorScript; // holds a reference to the cursor's script

	void Start () {
        cursorScript = cursor.GetComponent();
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
</pre></code>

To catch up to this point, pull from the commit https://github.com/Rosstin/GazeControllerTutorial/commit/2da3ccffd29a8ddeb99b44c09fe33f398ed10879

SELECTABLE AND NON-SELECTABLE OBJECTS

https://youtu.be/M6GBEZtWpkg

Now the cursor is behaving almost the way we want, but it's a bit annoying that it considers everything a selectable object, including the big purple screen. We want the cursor to recognize only the little screens as selectable, like in this video. Let's modify our VisionRaycast code so that only recognizes objects tagged as 'Selectable' trigger it.

Tag the 'screens' we built earlier as 'Selectable' objects by making a new tag, 'Selectable', and setting it on them.

Now, in VisionRaycast.cs, we'll make it so that it only cares about objects that are tagged 'Selectable'.

Take the Update statement for VisionRaycast and rewrite it like so:
<code><pre>    void Update () {
        RaycastHit hit = new RaycastHit(); // this object will collect data about the collision each frame

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit)) // do the raycast, based on the camera's position and orientation, and store the hit for our reference
        {
            cursorScript.setVisible(true); // the cursor has a script that depends on update, we want that to run regardless of whether or not it's invisible
            cursor.transform.position = hit.point; // place the cursor where the collision is happening
            if(hit.transform.gameObject.tag == "Selectable") // also, check if the thing you hit was tagged as "Selectable"
            { 
                cursorScript.hittingSomething = true; // tell the cursor that it's hitting something

                if (cursorScript.state == PointerSelectionController.STATE_SELECTING) // if the cursor is in the process of selecting...
                {
                    hit.transform.gameObject.GetComponent().justSelected = true; // grab the window object and give it a message, that it has been selected
                    cursorScript.state = PointerSelectionController.STATE_SELECTED; 
                }
                else if (cursorScript.state == PointerSelectionController.STATE_DESELECTING)
                {
                    cursorScript.state = PointerSelectionController.STATE_DESELECTED;
                }
            }
            else
            {
                cursorScript.hittingSomething = false; // tell the cursor it's not hitting anything
            }
        }
        else // there wasn't any collision
        {
            cursorScript.setVisible(false); // deactivate the cursor if it was active
            cursorScript.hittingSomething = false; // tell the cursor it's not hitting anything
        }

    }
</pre></code>

Now it will only trigger if it hits something, AND the thing it hit was 'Selectable'.

Now we have selectable and non-selectable objects, but selecting the objects doesn't DO anything... let's fix that.

SELECTED OBJECTS DO SOMETHING

I was building a virtual control room like the one in Minority Report, so my "do something" is going to be: the object you're looking at is brought in front of you.

I make a script called MRWindowController to attach to the Window objects.

<code><pre>using UnityEngine;
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

        if (forwardTimer &gt;= FORWARD_TIME) // if we've been in the STATE_SELECTED state long enough, go back to the STATE_DESELECTED state
        {
            state = STATE_DESELECTED;
        }

        // state execution
        if (state == STATE_SELECTED)
        {
            if (Vector3.Distance(transform.position, selectedPosition) &gt;= DISTANCE_FUZZ) // if we haven't reached our destination, keep going there
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
            if (Vector3.Distance(transform.position, defaultPosition) &gt;= DISTANCE_FUZZ)
            {
                transform.position = Vector3.Lerp(transform.position, defaultPosition, MOVEMENT_SPEED * Time.deltaTime); // lerp back towards the default position
            }
        }


    }
}
</pre></code>

Now we need a way to send that message to the Window. I modify the PointerSelection code to have 2 additional states: STATE_SELECTING and STATE_DESELECTING. When the timer fills up from the STATE_DESELECTED state, we switch to STATE_SELECTING. This way, VisionRaycast can recognize when the Pointer is in STATE_SELECTING, and use that info to know that it should send a "hey you got selected" message to the object it's looking at. Then it can switch the state to STATE_SELECTED, and it won't continue to send that message over and over again.

<code><pre>public class PointerSelectionController : MonoBehaviour
{

    public GameObject[] tickOclockWhite; // holds the 12 tick marks representing empty
    public GameObject[] tickOclockRed; // holds the 12 tick marks representing full

    public GameObject componentsFolder; // holds the visible components of the cursor so we can activate/deactivate them easily

    public bool hittingSomething = false; // set by VisionRaycast every frame so that we know if we're hitting something

    // STATE STUFF //
    public int state; // simple state

    public const int STATE_DESELECTED = 0; // nothing is selected
    public const int STATE_SELECTING = 1; // in the process of selecting something
    public const int STATE_SELECTED = 2; // something is selected
    public const int STATE_DESELECTING = 3; // in the process of deselecting something
    //             //

    public const float SELECTION_TIME = 2.0f; // the time it takes to select/deselect an object

    public float elapsedTime = 0.0f; // holds the fullness of the timer

    public float fractionComplete = 0.0f; // other scripts can see how complete the timer is

    public void initializePointerSelectionController()
    {
        hittingSomething = false;
        elapsedTime = 0.0f;
        state = STATE_DESELECTED;
    }

    void Start()
    {
        initializePointerSelectionController();
    }

    void Update()
    {
        float t = elapsedTime / SELECTION_TIME; // holds the fraction of timer completeness
        fractionComplete = t; // for reference by outside objects/scripts

        if (state == STATE_DESELECTED) // if nothing is selected
        {
            if (hittingSomething)
                elapsedTime += Time.deltaTime; // if you're hitting something, slowly fill the timer
            else
                elapsedTime = 0.0f; // if you're not hitting anything, zero the timer

            if (t &gt;= 1.00)
            {
                state = STATE_SELECTING; // switch state to STATE_SELECTING if the timer fills up
            }
        }
        else if (state == STATE_SELECTED) // if something is selected
        {
            if (hittingSomething)
                elapsedTime = SELECTION_TIME; // if you're hitting something, set timer to full
            else
                elapsedTime -= Time.deltaTime; // if you're not hitting anything, slowly empty the timer

            if (t &lt;= 0.00)
            {
                state = STATE_DESELECTING; // if the timer empties completely, switch states
            }
        }

        setClockFace(); // keep the clock face updated
    }
</pre></code>

Now in VisionRaycast.cs, we can intercept that state-changing event, and send the message to the object we are hitting.

<code><pre>public class VisionRaycast : MonoBehaviour {

    public Camera playerCamera; // holds a reference to the player camera

    public GameObject cursor; // holds a reference to a cursor object to be drawn where the player's gaze points
    public PointerSelectionController cursorScript; // holds a reference to the cursor's script

	void Start () {
        cursorScript = cursor.GetComponent();
    }

    void Update () {
        RaycastHit hit = new RaycastHit(); // this object will collect data about the collision each frame

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit)) // do the raycast, based on the camera's position and orientation, and store the hit for our reference
        {
            cursorScript.setVisible(true); // the cursor has a script that depends on update, we want that to run regardless of whether or not it's invisible
            cursor.transform.position = hit.point; // place the cursor where the collision is happening
            if(hit.transform.gameObject.tag == "Selectable") // also, check if the thing you hit was tagged as "Selectable"
            { 
                cursorScript.hittingSomething = true; // tell the cursor that it's hitting something

                if (cursorScript.state == PointerSelectionController.STATE_SELECTING) // if the cursor is in the process of selecting...
                {
                    hit.transform.gameObject.GetComponent().justSelected = true; // grab the window object and give it a message, that it has been selected
                    cursorScript.state = PointerSelectionController.STATE_SELECTED; 
                }
                else if (cursorScript.state == PointerSelectionController.STATE_DESELECTING)
                {
                    cursorScript.state = PointerSelectionController.STATE_DESELECTED;
                }
            }
            else
            {
                cursorScript.hittingSomething = false; // tell the cursor it's not hitting anything
            }
        }
        else // there wasn't any collision
        {
            cursorScript.setVisible(false); // deactivate the cursor if it was active
            cursorScript.hittingSomething = false; // tell the cursor it's not hitting anything
        }

    }
}
</pre></code>

And voila! Here you are. The cursor tracks your position, it fills up when you're looking at an object, and when it completely fills up, you tell the object to "do something".

https://youtu.be/nTPLH2-lAig

So, this is my first crack at implementing a cursor that selects objects. Gaze-detection in VR is always going to be important because the place where the player is looking is a persistent piece of information that is always useful... you can use it to grossly give the user control over a cursor-object as we've done here, but you can also be more subtle, and have the knowledge that the player is looking at an object influence its behavior. This kind of code and behavior is going to be essential throughout the lifetime of VR.

If you want to grab a copy of this completed tutorial project, just pull the project's final commit and try it out for yourself: https://github.com/Rosstin/GazeControllerTutorial/commit/9a0841af3d85df9870a9ff0e4fbf266179a3331c
