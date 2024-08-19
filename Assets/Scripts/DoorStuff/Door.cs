using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Door : MonoBehaviour
{

    // Attempt 1: HardCode Door transform {https://www.youtube.com/watch?v=cPltQK5LlGE&ab_channel=LlamAcademy} error ~ door wasn't rotating on correcti pivot point
    // Attempt 2: Code with animation of door {https://www.youtube.com/watch?v=iIBvTPC610M&t=176s&ab_channel=TwinGamingStudios} error ~ door wasnt recieving transform logic
    // Attempt 3: Re-attempt attempt 1 with new ~knowledge~


    public Animator door;
   // public GameObject openText;
    [SerializeField] playerLandController _controllerInput;
    [SerializeField] ControllerInput _input;
    
    public bool _doorIsClosed { get; private set; } = true;
    

    
    // public AudioSource doorSound;

  

    void Update()
    {
        DoorFunction();
    }

    void DoorOpens()
    {
        door.SetBool("DoorOpen", true);
        door.SetBool("DoorClosed", false);
       // doorSound.Play();

    }

    public void DoorCloses()
    {
        door.SetBool("DoorOpen", false);
        door.SetBool("DoorClosed", true);
        
       
    }

    private void DoorFunction()
    {
        if (_controllerInput._isLookingAtDoor && _doorIsClosed && _input.InteractionIsPressed && (!(_controllerInput._interactWasPressedLastFrame)))
        {
            DoorOpens();
            _doorIsClosed = false;
        }

        else if (_controllerInput._isLookingAtDoor && (!(_doorIsClosed)) && _input.InteractionIsPressed && (!(_controllerInput._interactWasPressedLastFrame)))
        {
            DoorCloses();
            _doorIsClosed = true;

        }
        _controllerInput._interactWasPressedLastFrame = _input.InteractionIsPressed;
    }
    // use the raycast to see which collider is being touched to decide which way to open door
}

