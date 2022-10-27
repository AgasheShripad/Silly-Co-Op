using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ActiveRagdoll {


    /// <summary> Tells the ActiveRagdoll what it should do. Input can be external (like the
    /// one from the player or from another script) and internal (kind of like sensors, such as
    /// detecting if it's on floor). </summary>
    public class InputModule : Module {

        #region External Input
        public delegate void onMoveDelegate(Vector2 movement);
        public delegate void onLeftDelegate(float armWeight);
        public delegate void onRightDelegate(float armWeight);
        public delegate void onJumpDelegate();
        public delegate void onSprintDelegate(bool val);
        public delegate void onFloorChangedDelegate(bool onFloor);


        public onMoveDelegate OnMoveDelegates { get; set; }
        public onRightDelegate OnRightDelegates { get; set; }
        public onLeftDelegate OnLeftDelegates { get; set; }
        public onJumpDelegate OnJumpDelegates { get; set; }
        public onSprintDelegate OnSprintDelegates { get; set; }

        public onFloorChangedDelegate OnFloorChangedDelegates { get; set; }


        public void OnMove(InputValue value)
        {
            OnMoveDelegates?.Invoke(value.Get<Vector2>());
        }

        public void OnLeft(InputValue value)
        {
            OnLeftDelegates?.Invoke(value.Get<float>());
        }

        public void OnRight(InputValue value) {
            OnRightDelegates?.Invoke(value.Get<float>());
        }
        
        public void OnJump(InputValue value)
        {
           if(_isOnFloor) OnJumpDelegates?.Invoke();
        }

        public void OnSprintPressed(InputValue value)
        {
            Debug.Log("Spring Press Action Invoked" + value);
            if (_isOnFloor) OnSprintDelegates?.Invoke(true);
        }

        public void OnSprintReleased(InputValue value)
        {
            Debug.Log("Spring Release Action Invoked" + value);
            if (_isOnFloor) OnSprintDelegates?.Invoke(false);
        }

        #endregion
        // ---------- INTERNAL INPUT ----------
        #region Internal Input

        [Header("--- FLOOR ---")]
        public float floorDetectionDistance = 0.3f;
        public float maxFloorSlope = 60;

        private bool _isOnFloor = true;
        public bool IsOnFloor { get { return _isOnFloor; } }

        Rigidbody _rightFoot, _leftFoot;

        #endregion

        void Start() {
            _rightFoot = _activeRagdoll.GetPhysicalBone(HumanBodyBones.RightFoot).GetComponent<Rigidbody>();
            _leftFoot = _activeRagdoll.GetPhysicalBone(HumanBodyBones.LeftFoot).GetComponent<Rigidbody>();


        }

        void Update() {
            UpdateOnFloor();
        }

       
        


        private void UpdateOnFloor() {
            bool lastIsOnFloor = _isOnFloor;

            _isOnFloor = CheckRigidbodyOnFloor(_rightFoot, out Vector3 foo)
                         || CheckRigidbodyOnFloor(_leftFoot, out foo);

            if (_isOnFloor != lastIsOnFloor && OnFloorChangedDelegates != null)
                OnFloorChangedDelegates(_isOnFloor);
        }

        /// <summary>
        /// Checks whether the given rigidbody is on floor
        /// </summary>
        /// <param name="bodyPart">Part of the body to check</param
        /// <returns> True if the Rigidbody is on floor </returns>
        public bool CheckRigidbodyOnFloor(Rigidbody bodyPart, out Vector3 normal) {
            // Raycast
            Ray ray = new Ray(bodyPart.position, Vector3.down);
            bool onFloor = Physics.Raycast(ray, out RaycastHit info, floorDetectionDistance, ~(1 << bodyPart.gameObject.layer));

            // Additional checks
            onFloor = onFloor && Vector3.Angle(info.normal, Vector3.up) <= maxFloorSlope;

            if (onFloor && info.collider.gameObject.TryGetComponent<Floor>(out Floor floor))
                    onFloor = floor.isFloor;

            normal = info.normal;
            return onFloor;
        }
    }
} // namespace ActiveRagdoll