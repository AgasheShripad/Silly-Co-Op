using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActiveRagdoll;
using UnityEngine.InputSystem;

/// <summary> Default behaviour of an Active Ragdoll </summary>
public class DefaultBehaviour : MonoBehaviour {
    

    [Header("Movement")]
    [SerializeField] public int CharStatus; // 0 = normal , 1 = legs, 2 = upper_body
    [SerializeField] public int PlayerNumber = 0;
    [SerializeField] public float Kill_Height=-10f;
    [SerializeField] public float SprintMultiplier = 2f;
    [SerializeField] private GameObject PysicalBody;


    [Header("Modules")]
    [SerializeField] private ActiveRagdoll.ActiveRagdoll _activeRagdoll;
    [SerializeField] private PhysicsModule _physicsModule;
    [SerializeField] private AnimationModule _animationModule;
    [SerializeField] private GripModule _gripModule;
    [SerializeField] private CameraModule _cameraModule;

    [Header("Movement")]
    [SerializeField] private bool _enableFloorMovement = true;
    [SerializeField] private bool _inAir = true;
    private Vector2 _movement;
    private float _speedMultiplyer = 3f;
    private float _speed = 1f;
    private Vector3 _aimDirection;



    private void OnValidate() {
        if (_activeRagdoll == null) _activeRagdoll = GetComponent<ActiveRagdoll.ActiveRagdoll>();
        if (_physicsModule == null) _physicsModule = GetComponent<PhysicsModule>();
        if (_animationModule == null) _animationModule = GetComponent<AnimationModule>();
        if (_gripModule == null) _gripModule = GetComponent<GripModule>();
        if (_cameraModule == null) _cameraModule = GetComponent<CameraModule>();
    }

    private void Start() {

        
            _activeRagdoll.Input.OnMoveDelegates += MovementInput;
            _activeRagdoll.Input.OnMoveDelegates += _physicsModule.ManualTorqueInput;

            _activeRagdoll.Input.OnFloorChangedDelegates += ProcessFloorChanged;

            _activeRagdoll.Input.OnLeftDelegates += _animationModule.UseLeftArm;
            _activeRagdoll.Input.OnLeftDelegates += _gripModule.UseLeftGrip;
            _activeRagdoll.Input.OnRightDelegates += _animationModule.UseRightArm;
            _activeRagdoll.Input.OnRightDelegates += _gripModule.UseRightGrip;

            _activeRagdoll.Input.OnJumpDelegates += _animationModule.Jump;
            _activeRagdoll.Input.OnJumpDelegates += _physicsModule.ManualUpForce;

            _activeRagdoll.Input.OnSprintDelegates += Sprint;

        _gripModule.SetBones(false);
        
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
        
        //if (PysicalBody.transform.position.y < Kill_Height) OnOutofBounds();

    }

    public void OnOutofBounds()
    {
        Debug.Log("Out of Bounds");
        if (PlayerManager.startingPoints[PlayerNumber-1] != null)
        {
            PlayerManager.PlayerDead(this.GetComponent<PlayerInput>(), PlayerNumber);
            Destroy(this.gameObject);
        }

    }

    public void Sprint(bool val)
    {
        if (val)
            _speed = _speedMultiplyer;
        else
            _speed = 1f;
    }

    private void UpdateMovement() {
        if (_movement == Vector2.zero && _enableFloorMovement)
        {
            if (_animationModule.Animator != null)
            {
                _animationModule.Animator.SetBool("moving", false);
                _animationModule.Animator.SetFloat("speed", 1);
            }
            return;
        }
        else if (_inAir==true && _movement != Vector2.zero)
        {
            _physicsModule.ManualAirForce(_movement);
            return;
        }
        else if(_animationModule.Animator == null)
        {
           return;
        }
        
        _animationModule.Animator.SetBool("moving", true);
        _animationModule.Animator.SetFloat("speed", _speed * _movement.magnitude);        

        float angleOffset = Vector2.SignedAngle(_movement, Vector2.up);
        Vector3 targetForward = Quaternion.AngleAxis(angleOffset, Vector3.up) * Auxiliary.GetFloorProjection(_aimDirection);
        _physicsModule.TargetDirection = targetForward;
    }

    private void ProcessFloorChanged(bool onFloor) {
        if (onFloor) {
            _inAir = false;
            //StartCoroutine(WaitBeforeDetectingFloor(5f));
            ProcessOnFloor();
        }
        else {
            
            _physicsModule.SetBalanceMode(PhysicsModule.BALANCE_MODE.MANUAL_TORQUE);
            _enableFloorMovement = false;
            _inAir = true;
            _activeRagdoll.GetBodyPart("Head Neck")?.SetStrengthScale(0.1f);
            _activeRagdoll.GetBodyPart("Right Leg")?.SetStrengthScale(0.05f);
            _activeRagdoll.GetBodyPart("Left Leg")?.SetStrengthScale(0.05f);
            _animationModule.PlayAnimation("InTheAir");
        }
    }


    void ProcessOnFloor()
    {
        _physicsModule.SetBalanceMode(PhysicsModule.BALANCE_MODE.STABILIZER_JOINT);
        _activeRagdoll.GetBodyPart("Head Neck")?.SetStrengthScale(1f);
        _activeRagdoll.GetBodyPart("Right Leg")?.SetStrengthScale(1f);
        _activeRagdoll.GetBodyPart("Left Leg")?.SetStrengthScale(1f);
        _animationModule.PlayAnimation("Idle");
        _enableFloorMovement = true;
    }

    IEnumerator WaitBeforeDetectingFloor(float waitTime)
    {
        yield return wait(waitTime);
        //
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
