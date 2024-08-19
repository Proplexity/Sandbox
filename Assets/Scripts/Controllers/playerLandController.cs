

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerLandController : MonoBehaviour
{

    public Vector3 _playerMoveInput = Vector3.zero;

    Vector3 _playerLookInput = Vector3.zero;
    Vector3 _previousPlayerLookInput;

    public Transform cameraFollow;

    private Rigidbody rb;
    CapsuleCollider _capsuleCollider = null;

    float _cameraPitch;

    [SerializeField] ControllerInput _input;
    [SerializeField] CameraController _cameraController;

    [SerializeField] float _playerLookInputLerpTime = 0.35f;

    [Header("Movement")]
    [SerializeField] float _movementMultiplier = 30.0f;
    [SerializeField] float _notGroundedMovementMultiplier = 1.25f;
    [SerializeField] float _rotationSpeedMultiplier = 180.0f;
    [SerializeField] float _pitchSpeedMultiplier = 180.0f;
    [SerializeField] float crouchSpeedMultiplier = 0.5f;
    [SerializeField] float _crouchSpeedMovementMultiplier = 0.5f;
    [SerializeField] float _runMultiplier = 2.5f;

    [Header("GroundCheck")]
    [SerializeField] bool _playerIsGrounded = true;
    // [Range()] creates a slider with the gicven parameters
    [SerializeField][Range(0.0f, 1.8f)] float _groundCheckRadiusMultiplier = 0.9f;
    [SerializeField][Range(-0.95f, 1.05f)] float _groundCheckDistanceTolerance = 0.05f;
    [SerializeField] float _playerCenterToGroundDistance = 0.0f;
    RaycastHit _groundCheckHit = new RaycastHit();

    [Header("Gravity")]
    [SerializeField] float _gravityFallCurrent = -100.0f;
    [SerializeField] float _gravityFallMin = -100.0f;
    [SerializeField] float _gravityFallMax = -500.0f;
    [SerializeField][Range(-5.0f, -35.0f)] float _gravityFallIncrementAmount = -20.0f;
    [SerializeField] float _gravityFallIncrementTime = 0.05f;
    [SerializeField] float _playerFallTimer = 0.0f;
    [SerializeField] float _gravityGrounded = -1.0f;
    [SerializeField] float _maxSlopeAngle = 47.5f;

    [Header("Stairs")]
    [SerializeField][Range(0.0f, 5.0f)] float _maxStepHeight = 0.5f;
    [SerializeField][Range(0.0f, 5.0f)] float _minStepDepth = 0.5f;
    [SerializeField] float _stairHeightPaddingMultiplier = 1.5f;
    [SerializeField] bool _isFirstStep = true;
    [SerializeField] float _firstStepVelocityDistanceMultiplier = 0.1f;
    [SerializeField] bool _playerIsAscendingStairs = false;
    [SerializeField] bool _playerIsDescendingStairs = false;
    [SerializeField] float AscendingStairsMovementeMultiplier = 0.35f;
    [SerializeField] float DescendingStairsMovementeMultiplier = 0.7f;
    [SerializeField] float _maximunAngleOfApproachToAscend = 45.0f;
    float _playerHalfHeightToGround = 0.0f;
    float maxAscendRayDistance = 0.0f;
    float maxDescendRayDistance = 0.0f;
    int _numberOfStepDetectRays = 0;
    float _rayIncrementAmount = 0.0f;


    [Header("Jumping")]
    [SerializeField] float _initialJumpForceMultiplier = 750.0f;
    [SerializeField] float _continualJumpForceMultiplier = 0.1f;
    [SerializeField] float _jumpTime = 0.175f;
    [SerializeField] float _jumpTimeCounter = 0.0f;
    [SerializeField] float _coyoteTime = 0.15f;
    [SerializeField] float _coyoteTimeCounter = 0.0f;
    [SerializeField] float _jumpBufferTime = 0.2f;
    [SerializeField] float _jumpBufferTimeCounter = 0.0f;
    [SerializeField] bool _playerIsJumping = false;
    [SerializeField] bool _jumpWasPressedLastFrame = false;

    [Header("Crouching")]
    [SerializeField] bool _playerIsCrouching = false;
    [SerializeField][Range(0.0f, 1.0f)] float _headCheckRadiusMultiplier = 0.9f;
    [SerializeField] float _crouchTimeMultiplier = 10.0f;
    [SerializeField] float _playerCrouchHeightTolerance = 0.05f;
    float _crouchAmount = 1.0f;
    float _playerFullHeight = 0.0f; // set in awake
    float _playerCrouchHeight = 0.0f; // set in awake
    Vector3 _playerCenterPoint = Vector3.zero;



    [Header("Just For Fun")]
    [SerializeField] float _jumpReactionForceMultiplier = 0.75f;
    RaycastHit _lastGroundCheckHit = new RaycastHit();
    Vector3 _playerMoveInputAtLastKnownGroundCheckHit = Vector3.zero;

    [Header("Interaction")]

    public Camera _camera;
    public LayerMask _door;
    public TextMeshPro _doorInteractText;
    public bool _isLookingAtDoor { get; private set; } = false;
    public bool _interactWasPressedLastFrame;






    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        maxAscendRayDistance = _maxStepHeight / Mathf.Cos(_maximunAngleOfApproachToAscend * Mathf.Deg2Rad);
        maxDescendRayDistance = _maxStepHeight / Mathf.Cos(80.0f * Mathf.Deg2Rad);

        _numberOfStepDetectRays = Mathf.RoundToInt(((_maxStepHeight * 100.0f) * 0.5f) + 1.0f);
        _rayIncrementAmount = _maxStepHeight / _numberOfStepDetectRays;

        _playerFullHeight = _capsuleCollider.height;
        _playerCrouchHeight = _playerFullHeight - _crouchAmount;
    }

   
    private void FixedUpdate()
    {

        if (!_cameraController.UsingOrbitalCamera)
        {
            _playerLookInput = GetLookInput();
            PlayerLook();
            PitchCamera();
        }

        _playerMoveInput = GetMoveInput();
        PlayerVariables();
        _playerIsGrounded = PlayerGroundCheck();

        _playerMoveInput = playerMove();
        _playerMoveInput = PlayerStairs();
        _playerMoveInput = PlayerSlope();
        _playerMoveInput = PlayerCrouch();
        _playerMoveInput = PlayerRun();

        PlayerInfoCapture();

        _playerMoveInput.y = PlayerFallGravity();
        _playerMoveInput.y = PlayerJump();

        Interact();

        // Debug.DrawRay(_playerCenterPoint, rb.transform.TransformDirection(_playerMoveInput), Color.red, 0.5f);
        //   Debug.DrawRay(this.transform.position, rb.transform.TransformDirection(_playerMoveInput), Color.blue, 5.0f);

        _playerMoveInput *= rb.mass; //for dev purposes

        rb.AddRelativeForce(_playerMoveInput, ForceMode.Force);
    }

    public Vector3 GetLookInput()
    {
        _previousPlayerLookInput = _playerLookInput;
        _playerLookInput = new Vector3(_input.lookInput.x, (_input.InvertMouseY ? -_input.lookInput.y : _input.lookInput.y), 0.0f);
        return Vector3.Lerp(_previousPlayerLookInput, _playerLookInput * Time.deltaTime, _playerLookInputLerpTime);
    }

    private void PlayerLook()
    {
        rb.rotation = Quaternion.Euler(0.0f, rb.rotation.eulerAngles.y + (_playerLookInput.x * _rotationSpeedMultiplier), 0.0f);
    }

    private void PitchCamera()
    {
        /*same effect
        Vector3 rotationValues = cameraFollow.rotation.eulerAngles; */
        _cameraPitch += _playerLookInput.y * _pitchSpeedMultiplier;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -89.9f, 89.9f);

        cameraFollow.rotation = Quaternion.Euler(_cameraPitch,
                                                  cameraFollow.rotation.eulerAngles.y,
                                                  cameraFollow.rotation.eulerAngles.z);
    }

    private Vector3 GetMoveInput()
    {
        return new Vector3(_input.moveInput.x, 0.0f, _input.moveInput.y);
    }

    private void PlayerVariables()
    {
        _playerCenterPoint = rb.position + _capsuleCollider.center;
    }
    private Vector3 playerMove()
    {
        /*  return new Vector3(_playerMoveInput.x * _movementMultiplier,
                             _playerMoveInput.y,
                             _playerMoveInput.z * _movementMultiplier);
        */


        return ((_playerIsGrounded) ? _playerMoveInput * _movementMultiplier : (_playerMoveInput * _movementMultiplier * _notGroundedMovementMultiplier));        // This is an if else statement
    }

    private bool PlayerGroundCheck()
    {
        float sphereCastRadius = _capsuleCollider.radius * _groundCheckRadiusMultiplier;
        Physics.SphereCast(_playerCenterPoint, sphereCastRadius, Vector3.down, out _groundCheckHit);
        _playerCenterToGroundDistance = _groundCheckHit.distance + sphereCastRadius;
        return ((_playerCenterToGroundDistance >= _capsuleCollider.bounds.extents.y - _groundCheckDistanceTolerance) &&
                (_playerCenterToGroundDistance <= _capsuleCollider.bounds.extents.y + _groundCheckDistanceTolerance));
    }

    private Vector3 PlayerStairs()
    {
        Vector3 calculatedStepInput = _playerMoveInput;

        _playerHalfHeightToGround = _capsuleCollider.bounds.extents.y;
        if(_playerCenterToGroundDistance < _capsuleCollider.bounds.extents.y)
        {
            _playerHalfHeightToGround = _playerCenterToGroundDistance;
        }

        calculatedStepInput = AscendStairs(calculatedStepInput);
        if(!(_playerIsAscendingStairs))
        {
            calculatedStepInput = DescendStairs(calculatedStepInput);
        }
        return calculatedStepInput;
    }

    private Vector3 AscendStairs(Vector3 calculatedStepInput)
    {
        if(_input.moveIsPressed)
        {
            float calculatedVelDistance = _isFirstStep ? (rb.velocity.magnitude * _firstStepVelocityDistanceMultiplier) + _capsuleCollider.radius : _capsuleCollider.radius;

            float ray = 0.0f;
            List<RaycastHit> raysThatHit = new List<RaycastHit>();
            for(int x = 1; x <= _rayIncrementAmount; x++, ray += _rayIncrementAmount)
            {
                Vector3 rayLower = new Vector3(_playerCenterPoint.x, ((_playerCenterPoint.y - _playerHalfHeightToGround) + ray), _playerCenterPoint.z);
                RaycastHit hitLower;
                if (Physics.Raycast(rayLower, rb.transform.TransformDirection(_playerMoveInput), out hitLower, calculatedVelDistance + maxAscendRayDistance))
                {
                    float stairSlopeAngle = Vector3.Angle(hitLower.normal, rb.transform.up);
                    if(stairSlopeAngle == 90.0f)
                    {
                        raysThatHit.Add(hitLower);
                    }
                }
            }
            if (raysThatHit.Count > 0)
            {
                Vector3 rayUpper = new Vector3(_playerCenterPoint.x, (((_playerCenterPoint.y - _playerHalfHeightToGround) + _maxStepHeight) + _rayIncrementAmount), _playerCenterPoint.z);
                RaycastHit hitUpper;
                Physics.Raycast(rayUpper, rb.transform.TransformDirection(_playerMoveInput), out hitUpper, calculatedVelDistance + (maxAscendRayDistance * 2.0f));
                if(!(hitUpper.collider) || (hitUpper.distance - raysThatHit[0].distance) > _minStepDepth)
                {
                    if(Vector3.Angle(raysThatHit[0].normal, rb.transform.TransformDirection(-_playerMoveInput)) <= _maximunAngleOfApproachToAscend)
                    {
                        Debug.DrawRay(rayUpper, rb.transform.TransformDirection(_playerMoveInput), Color.yellow, 5.0f);

                        _playerIsAscendingStairs = true;
                        Vector3 playerRelX = Vector3.Cross(_playerMoveInput, Vector3.up);

                        if(_isFirstStep)
                        {
                            calculatedStepInput = Quaternion.AngleAxis(45.0f, playerRelX) * calculatedStepInput;
                        }
                        else
                        {
                            float stairHeight = raysThatHit.Count * _rayIncrementAmount * _stairHeightPaddingMultiplier;

                            float avgDistance = 0.0f;
                            foreach (RaycastHit r in raysThatHit)
                            {
                                avgDistance += r.distance;
                            }
                            avgDistance /= raysThatHit.Count;

                            float tanAngle = Mathf.Atan2(stairHeight, avgDistance) * Mathf.Rad2Deg;
                            calculatedStepInput = Quaternion.AngleAxis(tanAngle, playerRelX) * calculatedStepInput;
                            calculatedStepInput *= AscendingStairsMovementeMultiplier;
                        }
                     
                    }
                    else
                    { // more than 45deg approach
                        _playerIsAscendingStairs = false;
                        _isFirstStep = true;
                    }
                }
                else
                { // top ray hit something
                    _playerIsAscendingStairs = false;
                    _isFirstStep = true;
                }
            }
            else
            { // no rays hit
                _playerIsAscendingStairs = false;
                _isFirstStep = true;
            }
        }
        else
        { // move is not pressed
            _playerIsAscendingStairs = false;
            _isFirstStep = true;
        }
        return calculatedStepInput;
    }

    private Vector3 DescendStairs(Vector3 calculatedStepInput)
    {
        if (_input.moveIsPressed)
        {

            float ray = 0.0f;
            List<RaycastHit> raysThatHit = new List<RaycastHit>();
            for (int x = 1; x <= _rayIncrementAmount; x++, ray += _rayIncrementAmount)
            {
                Vector3 rayLower = new Vector3(_playerCenterPoint.x, ((_playerCenterPoint.y - _playerHalfHeightToGround) + ray), _playerCenterPoint.z);
                RaycastHit hitLower;
                if (Physics.Raycast(rayLower, rb.transform.TransformDirection(-_playerMoveInput), out hitLower, _capsuleCollider.radius + maxDescendRayDistance))
                {
                    float stairSlopeAngle = Vector3.Angle(hitLower.normal, rb.transform.up);
                    if (stairSlopeAngle == 90.0f)
                    {
                        raysThatHit.Add(hitLower);
                    }
                }
            }
            if (raysThatHit.Count > 0)
            {
                Vector3 rayUpper = new Vector3(_playerCenterPoint.x, (((_playerCenterPoint.y - _playerHalfHeightToGround) + _maxStepHeight) + _rayIncrementAmount), _playerCenterPoint.z);
                RaycastHit hitUpper;
                Physics.Raycast(rayUpper, rb.transform.TransformDirection(-_playerMoveInput), out hitUpper, _capsuleCollider.radius + (maxDescendRayDistance * 2.0f));
                if (!(_playerIsGrounded) && hitUpper.distance < _capsuleCollider.radius + (maxDescendRayDistance * 2.0f))
                {
                    if (Vector3.Angle(raysThatHit[0].normal, rb.transform.TransformDirection(-_playerMoveInput)) <= _maximunAngleOfApproachToAscend)
                    {
                        Debug.DrawRay(rayUpper, rb.transform.TransformDirection(-_playerMoveInput), Color.yellow, 5.0f);

                        _playerIsDescendingStairs = true;
                        Vector3 playerRelX = Vector3.Cross(_playerMoveInput, Vector3.up);

                        float stairHeight = raysThatHit.Count * _rayIncrementAmount * _stairHeightPaddingMultiplier;

                        float avgDistance = 0.0f;
                        foreach (RaycastHit r in raysThatHit)
                        {
                                avgDistance += r.distance;
                        }
                        avgDistance /= raysThatHit.Count;

                        float tanAngle = Mathf.Atan2(stairHeight, avgDistance) * Mathf.Rad2Deg;
                        calculatedStepInput = Quaternion.AngleAxis(tanAngle - 90.0f, playerRelX) * calculatedStepInput;
                        calculatedStepInput *= AscendingStairsMovementeMultiplier;
                        

                    }
                    else
                    { // more than 45deg approach
                        _playerIsDescendingStairs = false;
                        
                    }
                }
                else
                { // top ray hit something
                    _playerIsDescendingStairs = false;
                    
                }
            }
            else
            { // no rays hit
                _playerIsDescendingStairs = false;
               
            }
        }
        else
        { // move is not pressed
            _playerIsDescendingStairs = false;
           
        }
        return calculatedStepInput;
    }

    private Vector3 PlayerSlope()
    {
        Vector3 calculatedPlayerMovement = _playerMoveInput;

        if (_playerIsGrounded && !_playerIsAscendingStairs && !_playerIsDescendingStairs)
        {
            Vector3 localGroundCheckHitNormal = rb.transform.InverseTransformDirection(_groundCheckHit.normal);
            float groundSlopeAngle = Vector3.Angle(localGroundCheckHitNormal, rb.transform.up);

            if (groundSlopeAngle == 0.0f)
            {
                if (_input.moveIsPressed)
                {
                    RaycastHit rayHit;
                    float rayCalculatedRayHeight = _playerCenterPoint.y - _playerCenterToGroundDistance + _groundCheckDistanceTolerance;
                    Vector3 rayOrigin = new Vector3(_playerCenterPoint.x, rayCalculatedRayHeight, _playerCenterPoint.z);
                    if (Physics.Raycast(rayOrigin, rb.transform.TransformDirection(calculatedPlayerMovement), out rayHit, 0.75f))
                    {
                        if (Vector3.Angle(rayHit.normal, rb.transform.up) > _maxSlopeAngle)
                        {
                            calculatedPlayerMovement.y = -_movementMultiplier;
                        }
                    }
                    // Debug.DrawRay(rayOrigin, rb.transform.TransformDirection(calculatedPlayerMovement), Color.green, 1.0f);
                }
                if (calculatedPlayerMovement.y == 0.0f)
                {
                    calculatedPlayerMovement.y = _gravityGrounded;
                }
            }
            else
            {
                Quaternion slopeAngleRotation = Quaternion.FromToRotation(rb.transform.up, localGroundCheckHitNormal);
                calculatedPlayerMovement = slopeAngleRotation * calculatedPlayerMovement;

                float relativeSlopeAngle = Vector3.Angle(calculatedPlayerMovement, rb.transform.up) - 90.0f;
                calculatedPlayerMovement += calculatedPlayerMovement * (relativeSlopeAngle / 90.0f);

                if (groundSlopeAngle < _maxSlopeAngle)
                {
                    if (_input.moveIsPressed)
                    {
                        calculatedPlayerMovement.y += _gravityGrounded;
                    }
                }
                else
                {
                    float calculatedSlopeGravity = groundSlopeAngle * -0.3f;
                    if (calculatedSlopeGravity < calculatedPlayerMovement.y)  // When your going uphill this statement is true, but when you go down hill your _calculatedPlayerMovement.y becomes a negative value
                    {
                        calculatedPlayerMovement.y = calculatedSlopeGravity;
                    }
                }
            }
// #if UNITY_EDITOR
//            Debug.DrawRay(rb.position, rb.transform.TransformDirection(calculatedPlayerMovement), Color.red, 0.5f);
// #endif
        }
        return calculatedPlayerMovement;
    }

    private Vector3 PlayerCrouch()
    {
        Vector3 calculatedPlayerCrouchSpeed = _playerMoveInput;
        if(_input.crouchIsPressed)
        {
            Crouch();
        }
        else if (_playerIsCrouching)
        {
            Uncrouch();
        }
        if(_playerIsCrouching)
        {
            calculatedPlayerCrouchSpeed *= _crouchSpeedMovementMultiplier;
        }
        return calculatedPlayerCrouchSpeed;
    }

    private void Crouch()
    {
        if (_capsuleCollider.height >= _playerCrouchHeight + _playerCrouchHeightTolerance)
        {
            float time = Time.fixedDeltaTime * crouchSpeedMultiplier;
            float amount = Mathf.Lerp(0.0f, _crouchAmount, time);

            _capsuleCollider.height -= amount;
            _capsuleCollider.center = new Vector3(_capsuleCollider.center.x, _capsuleCollider.center.y + (amount * 0.5f), _capsuleCollider.center.z);
            rb.position = new Vector3(rb.position.x, rb.position.y - amount, rb.position.z);

            _playerIsCrouching = true;
        }
        else
        {
            EnforceExactCharHeight();
        }
    }

    private void Uncrouch()
    {
        if(_capsuleCollider.height < _playerFullHeight - _playerCrouchHeightTolerance)
        {
            float sphereCastRadius = _capsuleCollider.radius * _headCheckRadiusMultiplier;
            float headRoomBufferDistance = 0.05f;
            float sphereCastTravelDistance = (_capsuleCollider.bounds.extents.y + headRoomBufferDistance) - sphereCastRadius;
           if(!(Physics.SphereCast(_playerCenterPoint, sphereCastRadius, rb.transform.up, out _, sphereCastTravelDistance)))
            {
                float time = Time.fixedDeltaTime * crouchSpeedMultiplier;
                float amount = Mathf.Lerp(0.0f, _crouchAmount, time);

                _capsuleCollider.height += amount;
                _capsuleCollider.center = new Vector3(_capsuleCollider.center.x, _capsuleCollider.center.y - (amount * 0.5f), _capsuleCollider.center.z);
                rb.position = new Vector3(rb.position.x, rb.position.y + amount, rb.position.z);
            } 
        }
        else
        {
            _playerIsCrouching = false;
            EnforceExactCharHeight();
        }
    }

    private void EnforceExactCharHeight()
    {
        if (_playerIsCrouching)
        {
            _capsuleCollider.height = _playerFullHeight - _crouchAmount;
            _capsuleCollider.center = new Vector3(0.0f, _crouchAmount*0.5f, 0.0f);
        }
        else
        {
            _capsuleCollider.height = _playerFullHeight;
            _capsuleCollider.center = Vector3.zero;
        }
    }

    private Vector3 PlayerRun()
    {
        Vector3 calculatePlayerRunSpeed = _playerMoveInput;

        if (_input.moveIsPressed && _input.runIsPressed && !_playerIsCrouching)
        {
            calculatePlayerRunSpeed *= _runMultiplier;
        }
        return calculatePlayerRunSpeed;

    }

    private void PlayerInfoCapture()
    {
        if (_playerIsGrounded && _groundCheckHit.collider)
        {
            _lastGroundCheckHit = _groundCheckHit;
            _playerMoveInputAtLastKnownGroundCheckHit = _playerMoveInput;
        }
    }

    private float PlayerFallGravity()
    {
        float gravity = _playerMoveInput.y;
        if (_playerIsGrounded || _playerIsAscendingStairs || _playerIsDescendingStairs)
        {
            _gravityFallCurrent = _gravityFallMin;
        }
        else
        {
            _playerFallTimer -= Time.fixedDeltaTime;
            if (_playerFallTimer < 0.0f)
            {
                if (_gravityFallCurrent > _gravityFallMax)
                {
                    _gravityFallCurrent += _gravityFallIncrementAmount;
                }
                _playerFallTimer = _gravityFallIncrementTime;

            }
            gravity = _gravityFallCurrent;
        }
        return gravity;
    }

    private float PlayerJump()
    {
        float calculatedJumpInput = _playerMoveInput.y;

        SetJumptTimeCounter();
        SetCoyoteTimeCounter();
        SetJumpBufferTimeCounter();


        if (_jumpBufferTimeCounter > 0.0f && !_playerIsJumping && _coyoteTimeCounter > 0.0f)
        {

            /* if (Vector3.Angle(rb.transform.up, _groundCheckHit.normal) < _maxSlopeAngle)
             {
                 calculatedJumpInput = _initialJumpForce;
                 _playerIsJumping = true;
                 _jumpBufferTimeCounter = 0.0f;
                 _coyoteTimeCounter = 0.0f;
             } */ //if player attempts to jump up >maxSlope disable jump

            KickOutFromUnderneath(); //Just for fun


            calculatedJumpInput = _initialJumpForceMultiplier;
            _playerIsJumping = true;
            _jumpBufferTimeCounter = 0.0f;
            _coyoteTimeCounter = 0.0f;
        }
        else if (_input.JumpIsPressed && _playerIsJumping && !_playerIsGrounded && _jumpTimeCounter > 0)
        {
            calculatedJumpInput = _initialJumpForceMultiplier * _continualJumpForceMultiplier;
        }
        else if (_playerIsJumping && _playerIsGrounded)
        {
            _playerIsJumping = false;

        }
        return calculatedJumpInput;
    }

    private void SetJumptTimeCounter()
    {
        if (_playerIsJumping && !_playerIsGrounded)
        {
            _jumpTimeCounter -= Time.fixedDeltaTime;
        }
        else
        {
            _jumpTimeCounter = _jumpTime;
        }
    }

    private void SetCoyoteTimeCounter()
    {
        if (_playerIsGrounded)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void SetJumpBufferTimeCounter()
    {
        if (!_jumpWasPressedLastFrame && _input.JumpIsPressed)
        {
            _jumpBufferTimeCounter = _jumpBufferTime;
        }
        else if (_jumpBufferTimeCounter > 0.0f)
        {
            _jumpBufferTimeCounter -= Time.fixedDeltaTime;
        }
        _jumpWasPressedLastFrame = _input.JumpIsPressed;
    }

    private void KickOutFromUnderneath()
    {
        if (_lastGroundCheckHit.collider.attachedRigidbody)
        {
            Vector3 force = rb.transform.TransformDirection(_playerMoveInputAtLastKnownGroundCheckHit) *
                                                            _lastGroundCheckHit.collider.attachedRigidbody.mass *
                                                            _jumpReactionForceMultiplier;
            _lastGroundCheckHit.collider.attachedRigidbody.AddForceAtPosition(-force, _lastGroundCheckHit.point, ForceMode.Impulse); //both rb.AddForce and rb.AddForceToPosition use world space coordinates which is why we convert to world space with rb.transform.TransformDirection
        }
    }


    private void Interact() //if raycast hits door collider and "interaction" is pushed play animation
    {
        
        float playerHeight = _capsuleCollider.bounds.extents.y * 3.0f  ;
        Vector3 RaycastOrigin = new Vector3(_playerCenterPoint.x,_playerCenterPoint.y + (_capsuleCollider.bounds.extents.y / 2), _playerCenterPoint.z );
        Debug.DrawRay(RaycastOrigin, _camera.transform.forward, Color.green, playerHeight);
        if (Physics.Raycast(RaycastOrigin, _camera.transform.forward , out RaycastHit Hit, playerHeight, _door))
        {
            Debug.Log("HitDoor");
            _doorInteractText.SetText("Open \"F\"");
            _doorInteractText.gameObject.SetActive(true);
            _isLookingAtDoor = true;

        }
        else
        {
            _doorInteractText.gameObject.SetActive(false);
            _isLookingAtDoor = false;
        }
        
        
    }
}



