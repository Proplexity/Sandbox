using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AutoDoorClose : MonoBehaviour
{
    [SerializeField] bool _hasExitedTrigger = false;
    public Door _door;
    [SerializeField] bool doorIsAuto = false;





    private void Update()
    {
        AutomaticDoorClose();
    }


    private void OnTriggerExit(Collider other)
    {
        _hasExitedTrigger = true;
    }

    private void AutomaticDoorClose()
    {
        if ((!(_door._doorIsClosed)) && _hasExitedTrigger && doorIsAuto)
        {
            Task.Delay(10000);
            _door.DoorCloses();

        }
        _hasExitedTrigger = false;
    }
}
