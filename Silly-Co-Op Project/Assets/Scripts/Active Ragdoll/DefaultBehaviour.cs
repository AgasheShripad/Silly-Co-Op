using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActiveRagdoll;

/// <summary> Default behaviour of an Active Ragdoll </summary>
public class DefaultBehaviour : MonoBehaviour {
    

    [Header("Movement")]
    [SerializeField] public int CharStatus; // 0 = normal , 1 = legs, 2 = upper_body


    [Header("Modules")]
    [SerializeField] private ActiveRagdoll.ActiveRagdoll _activeRagdoll;
    [SerializeField] private PhysicsModule _physicsModule;
    [SerializeField] private AnimationModule _animationModule;
    [SerializeField] private GripModule _gripModule;
    [SerializeField] private CameraModule _cameraModule;

    [Header("Movement")]
    [SerializeField] private bool _enableMovement = true;
    [SerializeField] private bool _upRight = true;
    private Vector2 _movement;

    private Vector3 _aimDirection;



    private void OnValidate() {
        if (_activeRagdoll == null) _activeRagdoll = GetComponent<ActiveRagdoll.ActiveRagdoll>();
        if (_physicsModule == null) _physicsModule = GetComponent<PhysicsModule>();
        if (_animationModule == null) _animationModule = GetComponent<AnimationModule>();
        if (_gripModule == null) _gripModule = GetComponent<GripModule>();
        if (_cameraModule == null) _cameraModule = GetComponent<CameraModule>();
    }

    private void Start() {
        // Link all the functions to its input to define how the ActiveRagdoll will behave.
        // This is a default implementation, where the input player is binded directly to
        // the ActiveRagdoll actions in a very simple way. But any implementation is
        // possible, such as assigning those same actions to the output of an AI system.
        if (CharStatus==1)
        {
            _activeRagdoll.Input.OnMoveDelegates += MovementInput;
            _activeRagdoll.Input.OnMoveDelegates += _physicsModule.ManualTorqueInput;
            _activeRagdoll.Input.OnFloorChangedDelegates += ProcessFloorChanged;

            _activeRagdoll.Input.OnLeftDelegates += _animationModule.UseLeftArm;
            _activeRagdoll.Input.OnLeftDelegates += _gripModule.UseLeftGrip;
            _activeRagdoll.Input.OnRightDelegates += _animationModule.UseRightArm;
            _activeRagdoll.Input.OnRightDelegates += _gripModule.UseRightGrip;

            _activeRagdoll.Input.OnJumpDelegates += _animationModule.Jump;
            _activeRagdoll.Input.OnJumpDelegates += _physicsModule.ManualUpTorqueInput;
            _gripModule.SetBones(true);
            _physicsModule.AirControl = false;

        }
        else if(CharStatus==2)
        {
            _activeRagdoll.Input.OnMoveDelegates += MovementInput;
            _activeRagdoll.Input.OnMoveDelegates += _physicsModule.ManualTorqueInput;
            _activeRagdoll.Input.OnFloorChangedDelegates += ProcessFloorChanged;

            _activeRagdoll.Input.OnLeftDelegates += _animationModule.UseLeftArm;
            _activeRagdoll.Input.OnLeftDelegates += _gripModule.UseLeftGrip;
            _activeRagdoll.Input.OnRightDelegates += _animationModule.UseRightArm;
            _activeRagdoll.Input.OnRightDelegates += _gripModule.UseRightGrip;

            _activeRagdoll.Input.OnJumpDelegates += _animationModule.Jump;
            _activeRagdoll.Input.OnJumpDelegates += _physicsModule.ManualUpTorqueInput;

            _gripModule.SetBones(false);
            _physicsModule.AirControl = true;
        }
        else
        {
            _activeRagdoll.Input.OnMoveDelegates += MovementInput;
            _activeRagdoll.Input.OnMoveDelegates += _physicsModule.ManualTorqueInput;
            _activeRagdoll.Input.OnFloorChangedDelegates += ProcessFloorChanged;

            _activeRagdoll.Input.OnLeftDelegates += _animationModule.UseLeftArm;
            _activeRagdoll.Input.OnLeftDelegates += _gripModule.UseLeftGrip;
            _activeRagdoll.Input.OnRightDelegates += _animationModule.UseRightArm;
            _activeRagdoll.Input.OnRightDelegates += _gripModule.UseRightGrip;

            _activeRagdoll.Input.OnJumpDelegates += _animationModule.Jump;
            _activeRagdoll.Input.OnJumpDelegates += _physicsModule.ManualUpTorqueInput;

            _gripModule.SetBones(false);
        }
    }

        private void Update() {
        _aimDirection = _cameraModule.Camera.transform.forward;
        _animationModule.AimDirection = _aimDirection;

        UpdateMovement();

#if UNITY_EDITOR
        // TEST
        if (Input.GetKeyDown(KeyCode.F1))
            Debug.Break();
#endif

    }
    
    private void UpdateMovement() {
        if (_movement == Vector2.zero || !_enableMovement ) {
            if(_animationModule.Animator!=null) _animationModule.Animator.SetBool("moving", false);
            return;
        }

        _animationModule.Animator.SetBool("moving", true);
        _animationModule.Animator.SetFloat("speed", _movement.magnitude);        

        float angleOffset = Vector2.SignedAngle(_movement, Vector2.up);
        Vector3 targetForward = Quaternion.AngleAxis(angleOffset, Vector3.up) * Auxiliary.GetFloorProjection(_aimDirection);
        _physicsModule.TargetDirection = targetForward;
    }

    private void ProcessFloorChanged(bool onFloor) {
        if (onFloor) {
            _upRight = false;
            //StartCoroutine(WaitBeforeDetectingFloor(5f));
            ProcessOnFloor();
        }
        else {
            _physicsModule.SetBalanceMode(PhysicsModule.BALANCE_MODE.MANUAL_TORQUE);
            _enableMovement = false;
            _activeRagdoll.GetBodyPart("Head Neck")?.SetStrengthScale(0.1f);
            _activeRagdoll.GetBodyPart("Right Leg")?.SetStrengthScale(0.05f);
            _activeRagdoll.GetBodyPart("Left Leg")?.SetStrengthScale(0.05f);
            _animationModule.PlayAnimation("InTheAir");
            _upRight = false;
        }
    }


    void ProcessOnFloor()
    {
        _physicsModule.SetBalanceMode(PhysicsModule.BALANCE_MODE.STABILIZER_JOINT);
        _activeRagdoll.GetBodyPart("Head Neck")?.SetStrengthScale(1f);
        _activeRagdoll.GetBodyPart("Right Leg")?.SetStrengthScale(1f);
        _activeRagdoll.GetBodyPart("Left Leg")?.SetStrengthScale(1f);
        _animationModule.PlayAnimation("Idle");
        _enableMovement = true;
        _upRight = true;
    }

    IEnumerator WaitBeforeDetectingFloor(float waitTime)
    {
        yield return wait(waitTime);
        /*
        _activeRagdoll.GetBodyPart("Head Neck")?.SetStrengthScale(Mathf.Lerp(_activeRagdoll.GetBodyPart("Head Neck").StrengthScale,1,waitTime));
        _activeRagdoll.GetBodyPart("Right Leg")?.SetStrengthScale(Mathf.Lerp(_activeRagdoll.GetBodyPart("Right Leg").StrengthScale, 1, waitTime)); ;
        _activeRagdoll.GetBodyPart("Left Leg")?.SetStrengthScale(Mathf.Lerp(_activeRagdoll.GetBodyPart("Left Leg").StrengthScale, 1, waitTime)); ;*/
        _activeRagdoll.GetBodyPart("Head Neck")?.SetStrengthScale(1f);
        _activeRagdoll.GetBodyPart("Right Leg")?.SetStrengthScale(1f);
        _activeRagdoll.GetBodyPart("Left Leg")?.SetStrengthScale(1f);
        //_animationModule.PlayAnimation("Idle");
        _physicsModule.SetBalanceMode(PhysicsModule.BALANCE_MODE.STABILIZER_JOINT);
        _enableMovement = true;
        _upRight = true;
    }

    IEnumerator wait(float waitTime)
    {
        float counter = 0;
        while (counter < waitTime)
        {
            //Increment Timer until counter >= waitTime
            counter += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary> Make the player move and rotate </summary>
    private void MovementInput(Vector2 movement)
    {
       _movement = movement;
    }
}
