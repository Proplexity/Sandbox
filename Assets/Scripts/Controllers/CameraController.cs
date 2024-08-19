using Cinemachine;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class CameraController : MonoBehaviour
{
    public bool UsingOrbitalCamera { get; private set; } = false;

    [SerializeField] ControllerInput _input;
    [SerializeField] float _cameraZoomModifier = 32.0f;
    CinemachineVirtualCamera _activeCamera;
    int _activeCameraPriorityModifier = 31337;

    float _minCameraZoomDistance = 1.5f;
    float _minOrbitCameraZoomDistance = 1.0f;
    float _maxCameraZoomDistance = 12.0f;
    float _maxOrbitCameraZoomDistance = 36.0f;

    public Camera MainCamera;
    public CinemachineVirtualCamera cinemachine1stPerson;
    CinemachineFramingTransposer _cinemachineFramingTransposer3rdPerson;
    public CinemachineVirtualCamera cinemachine3rdPerson;
    CinemachineFramingTransposer _cinemachineFramingTransposerOrbital;
    public CinemachineVirtualCamera cinemachineOrbital;

    private void Awake()
    {
        _cinemachineFramingTransposer3rdPerson = cinemachine3rdPerson.GetCinemachineComponent<CinemachineFramingTransposer>();
        _cinemachineFramingTransposerOrbital = cinemachineOrbital.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    private void Start()
    {
        ChangeCamera();
    }

    private void Update()
    {
        if (_input.CameraChangeWasPressedThisFrame) { ChangeCamera(); }
        if (!(_input.ZoomCameraInput == 0.0f)) { ZoomCamera(); }
        
    }

    private  async void ChangeCamera()
    {
        if (cinemachine3rdPerson == _activeCamera)
        {
            SetCameraPriorities(cinemachine3rdPerson, cinemachine1stPerson);
            UsingOrbitalCamera = false;
            
            await Task.Delay(800);
            MainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("PlayerSelf"));
        }
        else if (cinemachine1stPerson == _activeCamera)
        {
            SetCameraPriorities(cinemachine1stPerson, cinemachineOrbital);
            UsingOrbitalCamera = true;
            MainCamera.cullingMask |= 1 << LayerMask.NameToLayer("PlayerSelf");
        }
        else if (cinemachineOrbital == _activeCamera)
        {
            SetCameraPriorities(cinemachineOrbital, cinemachine3rdPerson);
            _activeCamera = cinemachine3rdPerson;
            UsingOrbitalCamera = false;
        }
        else
        {
            cinemachine3rdPerson.Priority += _activeCameraPriorityModifier;
            _activeCamera = cinemachine3rdPerson;
        }
    }

    private void ZoomCamera()
    {
        if(_activeCamera == cinemachine3rdPerson)
        {
            _cinemachineFramingTransposer3rdPerson.m_CameraDistance = Mathf.Clamp(_cinemachineFramingTransposer3rdPerson.m_CameraDistance + 
                (!_input.InvertScroll ? _input.ZoomCameraInput : -_input.ZoomCameraInput) / _cameraZoomModifier,
                _minCameraZoomDistance, 
                _maxCameraZoomDistance);
           
        }
        else if (_activeCamera == cinemachineOrbital)
        {
            _cinemachineFramingTransposerOrbital.m_CameraDistance = Mathf.Clamp(_cinemachineFramingTransposerOrbital.m_CameraDistance +
               (!_input.InvertScroll ? _input.ZoomCameraInput : -_input.ZoomCameraInput) / _cameraZoomModifier,
               _minCameraZoomDistance,
               _maxCameraZoomDistance);
        }
    }

    private void SetCameraPriorities( CinemachineVirtualCamera CurrentCameraMode, CinemachineVirtualCamera NewCameraMode)
    {
        CurrentCameraMode.Priority -= _activeCameraPriorityModifier;
        NewCameraMode.Priority += _activeCameraPriorityModifier;
        _activeCamera = NewCameraMode;
    }

}
