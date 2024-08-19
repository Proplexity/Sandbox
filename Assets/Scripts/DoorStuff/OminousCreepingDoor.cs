using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OminousCreepingDoor : MonoBehaviour
{
    Rigidbody _rb;
    [SerializeField] [Range(0, 10)] float _speed;
    [SerializeField] Door door;
    private Vector3 _startLocation = Vector3.zero;
    [SerializeField] float _multipliedSpeed;
    [SerializeField] float _amount;
    [SerializeField] GameObject OminousWall;
    

    private void Awake()
    {
        _startLocation = transform.position;
    }

    private void moving()
    {
            if ((!(door._doorIsClosed)))
            {
                transform.position += (transform.right * _speed) * -1 * Time.fixedDeltaTime;
            }
            if (door.door.GetBool("DoorClosed"))
            {
                transform.position += ((transform.right * _speed) * _multipliedSpeed) * -1 * Time.fixedDeltaTime;
            }
    }

    private void reset()
    {
        if (transform.position.x <= _startLocation.x + _amount)
        {
            Debug.Log("Warning");
            Destroy(gameObject);
            Instantiate(OminousWall, _startLocation, Quaternion.identity);
            
        }
    }


    private void FixedUpdate()
    {
        moving();
        reset();  
    }
}
