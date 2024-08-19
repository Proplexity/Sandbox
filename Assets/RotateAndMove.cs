using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndMove : MonoBehaviour
{
    Rigidbody _rb = null;

    [SerializeField] bool _rotateEnabled;
    [SerializeField] float _rotationSpeed;

    [SerializeField] bool _moveEnabled;
    [SerializeField] float _speed;
    Vector3 _startPosotion = Vector3.zero;
    Vector3 _endPositoin = Vector3.zero;

    Vector3 _platformPositionLastFrame = Vector3.zero;
    float _timeScale = 0.0f;

    Dictionary<Rigidbody, float> RBsOnPlatformAndTime = new Dictionary<Rigidbody, float>();
    [SerializeField] List<Rigidbody> RbsOnPlatform = new List<Rigidbody>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _startPosotion = _rb.position;
        _endPositoin = new Vector3(_startPosotion.x + 3.0f, _startPosotion.y + 3.0f, _startPosotion.z);
    }

    private void FixedUpdate()
    {
        if (_rotateEnabled)
        {
            _rb.rotation = Quaternion.Euler(_rb.rotation.eulerAngles.x,
                                            _rb.rotation.eulerAngles.y + _rotationSpeed * Time.fixedDeltaTime,
                                            _rb.rotation.eulerAngles.z);
        }
        if (RbsOnPlatform.Count != RBsOnPlatformAndTime.Count)
        {
            RBsOnPlatformAndTime.Clear();
            foreach(Rigidbody rb in RbsOnPlatform)
            {
                RBsOnPlatformAndTime.Add(rb, 1.0f);
            }
        }
        if (_moveEnabled)
        {
            _platformPositionLastFrame = _rb.position;
            _timeScale = _speed / Vector3.Distance(_startPosotion, _endPositoin);
            _rb.position = Vector3.Lerp(_endPositoin, _startPosotion, Mathf.Abs(Time.time * _timeScale % 2 - 1));
        }
        foreach (Rigidbody rb in RbsOnPlatform)
        {
            RBsOnPlatformAndTime.TryGetValue(rb, out float timer);
            if (timer < 1.0f)
            {
                RBsOnPlatformAndTime[rb] += Time.deltaTime * 4.0f;
            }
            else if (timer > 1.0f)
            {
                RBsOnPlatformAndTime[rb] = 1.0f;
            }
            RotateAndMoveRBsOnPlatform(rb, timer);
        }

    }

    private void RotateAndMoveRBsOnPlatform(Rigidbody rb, float timer)
    {
        if (_rotateEnabled)
        {
            float rotationAmount = _rotationSpeed * timer * Time.deltaTime;

            Quaternion localAngleAxis = Quaternion.AngleAxis(rotationAmount, _rb.transform.up);
            rb.position = (localAngleAxis * (rb.position - _rb.position)) + _rb.position;

            Quaternion globalAngleAxis = Quaternion.AngleAxis(rotationAmount, rb.transform.InverseTransformDirection(_rb.transform.up));
            rb.rotation *= globalAngleAxis;
        }

        if (_moveEnabled)
        {
            rb.position += (_rb.position - _platformPositionLastFrame) * timer;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        if (other.attachedRigidbody != null && (!(other.attachedRigidbody.isKinematic)))
        {
            if(!(RbsOnPlatform.Contains(other.attachedRigidbody)))
            {
                RbsOnPlatform.Add(other.attachedRigidbody);
                RBsOnPlatformAndTime.Add(other.attachedRigidbody, 0.0f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        if (!(other.attachedRigidbody == null))
        {
            if(RbsOnPlatform.Contains(other.attachedRigidbody))
            {
                RbsOnPlatform.Remove(other.attachedRigidbody);
                RBsOnPlatformAndTime.Remove(other.attachedRigidbody);
            }
        }
    }
}
