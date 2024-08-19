using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulator : MonoBehaviour
{
    const float GravitationalConstant = 0.667408f;

    [SerializeField] ControllerInput _input;
    [SerializeField] OnScreenCounter _onScreenCounter;

    [SerializeField] GameObject laser;

    MeshRenderer MeshRenderer;

    private List<Rigidbody> Attractees = new List<Rigidbody>();

    [SerializeField] bool _manipulatorIsEnabled = false;
    [SerializeField] bool _manipulatorToggledOn = false;
    [SerializeField] bool _manipulatorModeToggled = false;

    [SerializeField] float JoystickForceVariable;

    private void Awake()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (_input.OnOffWasPressedThisFrame) { _manipulatorIsEnabled = !_manipulatorIsEnabled; }
        if (_input.ModeWasPressedThisFrame) { _manipulatorModeToggled = !_manipulatorModeToggled; }
        _manipulatorToggledOn = _input.fireIsPushed > 0.0f;
        SetColor();
        ToggleLaser();

        Debug.Log(_input.fireIsPushed);
    }

   

    private void ToggleLaser()
    {
        if (_input.aimIsPushed)
        {
            laser.SetActive(true);
        }
        else
        {
            laser.SetActive(false);
        }
    }

    private void SetColor()
    {
        if (_manipulatorIsEnabled || _manipulatorToggledOn)
        {
            if (_manipulatorModeToggled)
            {
                MeshRenderer.material.color = Color.red;
            }
            else
            {
                MeshRenderer.material.color = Color.blue;
            }
        }
        else
        {
            if (_manipulatorModeToggled)
            {
                MeshRenderer.material.color = Color.white + Color.red;
            }
            else
            {
                MeshRenderer.material.color = Color.white + Color.blue;
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (Rigidbody attractee in Attractees)
        {
            if (attractee != this)
            {
                Atract(attractee);
            }
        }
    }

    private void Atract(Rigidbody rbToAttack)
    {
        if (_manipulatorIsEnabled || _manipulatorToggledOn)
        {
            Vector3 direction = transform.position - rbToAttack.position;
            float distance = direction.magnitude;

            if (distance <= 0.0f) { return; }

            float forceMagnitude = 0.0f;
            if (_input.fireIsPushed >= 0.0f && _input.fireIsPushed < 1.0f)
            {
                forceMagnitude = (GravitationalConstant * (750.0f * rbToAttack.mass) / distance * _input.fireIsPushed ) * (_input.fireIsPushed * JoystickForceVariable);
            }
            else if (_input.fireIsPushed == 1)
            {
                forceMagnitude = GravitationalConstant * (750.0f * rbToAttack.mass) / distance;
            }
            Vector3 force = direction.normalized * forceMagnitude;

            if (_manipulatorModeToggled)
            {
                rbToAttack.AddForce(-force);
            }
            else
            {
                rbToAttack.AddForce(force);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.attachedRigidbody == null) && !(other.attachedRigidbody.isKinematic))
        {
            if (!(Attractees.Contains(other.attachedRigidbody)))
            {
                Attractees.Add(other.attachedRigidbody);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Attractees.Contains(other.attachedRigidbody))
        {
            if (_manipulatorIsEnabled || _manipulatorToggledOn)
            {
                other.attachedRigidbody.useGravity = false;
            }
            else
            {
                if (!(other.gameObject.CompareTag("Player")))
                {
                    other.attachedRigidbody.useGravity = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!(other.attachedRigidbody == null))
        {
            if (Attractees.Contains(other.attachedRigidbody))
            {
                Attractees.Remove(other.attachedRigidbody);
                if (!(other.gameObject.CompareTag("Player")))
                {
                    other.attachedRigidbody.useGravity = true;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_manipulatorIsEnabled || _manipulatorToggledOn)
        {
            if (Attractees.Contains(collision.rigidbody))
            {
                if (!(collision.gameObject.CompareTag("Player")))
                {
                    Attractees.Remove(collision.rigidbody);
                    Destroy(collision.gameObject);
                    _onScreenCounter.Counter++;
                }
            }
        }
    }

}
