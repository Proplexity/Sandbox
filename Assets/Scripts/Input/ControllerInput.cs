using UnityEngine.InputSystem;
using UnityEngine;

public class ControllerInput : MonoBehaviour
{
    public Vector2 moveInput { get; private set; } = Vector2.zero;
    public Vector2 lookInput { get; private set; } = Vector2.zero;

    public bool moveIsPressed = false;
    public bool InvertMouseY { get; private set; } = true;
    public float ZoomCameraInput { get; private set; } = 0.0f;
    public bool InvertScroll { get; private set; } = true;

    public bool JumpIsPressed { get; private set; } = false;

    public bool runIsPressed { get; private set; } = false;

    public bool InteractionIsPressed { get; private set; } = false;
    public bool crouchIsPressed { get; private set; } = false;
    public bool CameraChangeWasPressedThisFrame { get; private set; } = false;

    public float fireIsPushed { get; private set; } = 0.0f;

    public bool reloadISPushed { get; private set; } = false;

    public bool toggleIsPushed { get; private set; } = false;

    public bool aimIsPushed { get; private set; } = false;

    public bool WeaponIsSwapped { get; private set; } = false;

    public bool OnOffWasPressedThisFrame { get; private set; } = false;

    public bool ModeWasPressedThisFrame { get; private set; } = false;

    InputActions _input;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void OnEnable()
    {
        _input = new InputActions();
        _input.playerLand.Enable();
        
        _input.playerLand.Move.performed += SetMove;
        _input.playerLand.Move.canceled += SetMove;
        
        _input.playerLand.Look.performed += SetLook;
        _input.playerLand.Look.canceled += SetLook;

        _input.playerLand.Run.started += SetRun;
        _input.playerLand.Run.canceled += SetRun;

        _input.playerLand.Crouch.started += SetCrouch;
        _input.playerLand.Crouch.canceled += SetCrouch;

        _input.playerLand.Jump.started += SetJump;
        _input.playerLand.Jump.canceled += SetJump;

        _input.playerLand.Interaction.started += SetInteraction;
        _input.playerLand.Interaction.canceled += SetInteraction;

        _input.playerLand.Aim.started += SetAim;
        _input.playerLand.Aim.canceled += SetAim;

        _input.playerLand.FireAction.performed += SetFireAction;
        _input.playerLand.FireAction.canceled += SetFireAction;

        _input.playerLand.Reload.started += SetReload;
        _input.playerLand.Reload.canceled += SetReload;

        _input.playerLand.FireActionToggle.started += SetToggle;
        _input.playerLand.FireActionToggle.canceled += SetToggle;

        _input.playerLand.WeaponSwap.started += SetWeaponChange;
        _input.playerLand.WeaponSwap.canceled += SetWeaponChange;

        _input.playerLand.ZoomCamera.started += SetZoomCamera;
        _input.playerLand.ZoomCamera.canceled += SetZoomCamera;
    }

    private void OnDisable()
    {

        _input.playerLand.Move.performed -= SetMove;
        _input.playerLand.Move.canceled -= SetMove;

        _input.playerLand.Look.performed -= SetLook;
        _input.playerLand.Look.canceled -= SetLook;

        _input.playerLand.Run.started -= SetRun;
        _input.playerLand.Run.canceled -= SetRun;

        _input.playerLand.Crouch.started -= SetCrouch;
        _input.playerLand.Crouch.canceled -= SetCrouch;

        _input.playerLand.Jump.started -= SetJump;
        _input.playerLand.Jump.canceled -= SetJump;

        _input.playerLand.Interaction.started -= SetInteraction;
        _input.playerLand.Interaction.canceled -= SetInteraction;

        _input.playerLand.Aim.started -= SetAim;
        _input.playerLand.Aim.canceled -= SetAim;

        _input.playerLand.FireAction.performed -= SetFireAction;
        _input.playerLand.FireAction.canceled -= SetFireAction;

        _input.playerLand.Reload.started -= SetReload;
        _input.playerLand.Reload.canceled -= SetReload;

        _input.playerLand.FireActionToggle.started -= SetToggle;
        _input.playerLand.FireActionToggle.canceled -= SetToggle;

        _input.playerLand.WeaponSwap.started -= SetWeaponChange;
        _input.playerLand.WeaponSwap.canceled -= SetWeaponChange;

        _input.playerLand.ZoomCamera.started -= SetZoomCamera;
        _input.playerLand.ZoomCamera.canceled -= SetZoomCamera;

        _input.playerLand.Disable();
    }

    private void Update()
    {
        CameraChangeWasPressedThisFrame = _input.playerLand.ChangeCam.WasPressedThisFrame();
        OnOffWasPressedThisFrame = _input.playerLand.OnOff.WasPressedThisFrame();
        ModeWasPressedThisFrame = _input.playerLand.Mode.WasPressedThisFrame();
    }

    private void SetMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        moveIsPressed = !(moveInput == Vector2.zero);
    }

    private void SetLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }
    private void SetZoomCamera(InputAction.CallbackContext ctx)
    {
        ZoomCameraInput = ctx.ReadValue<float>();
        
    }

    private void SetRun(InputAction.CallbackContext ctx)
    {
        runIsPressed = ctx.started;
    }

    private void SetCrouch(InputAction.CallbackContext ctx)
    {
        crouchIsPressed = ctx.started;
    }

    private void SetJump(InputAction.CallbackContext ctx)
    {
        JumpIsPressed = ctx.started;
    }

    private void SetInteraction(InputAction.CallbackContext ctx)
    {
        InteractionIsPressed = ctx.started;
    }

    private void SetAim(InputAction.CallbackContext ctx)
    {
        aimIsPushed = ctx.started;
    }

    private void SetFireAction (InputAction.CallbackContext ctx)
    {
        fireIsPushed = ctx.ReadValue<float>();
        
    }

    private void SetReload(InputAction.CallbackContext ctx)
    {
        reloadISPushed = ctx.started;
    }

    private void SetToggle(InputAction.CallbackContext ctx)
    {
        toggleIsPushed = ctx.started;
    }

    private void SetWeaponChange (InputAction.CallbackContext cts)
    {
        WeaponIsSwapped = cts.started;
    }
}
