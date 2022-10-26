using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActiveRagdoll {

    public class GripModule : Module {
        [Tooltip("What's the minimum weight the arm IK should have over the whole " +
        "animation to activate the grip")]
        public float leftArmWeightThreshold = 0.5f, rightArmWeightThreshold = 0.5f;
        public JointMotionsConfig defaultMotionsConfig;

        [Tooltip("Whether to only use Colliders marked as triggers to detect grip collisions.")]
        public bool onlyUseTriggers = false;
        public bool canGripYourself = false;
        public bool OnlyLegs = false;

        private Gripper _leftGrip, _rightGrip;



        private void Start() {
            
        }

        public void SetBones(bool OnlyLegs)
        {
            this.OnlyLegs = OnlyLegs;
            GameObject left;
            GameObject right;

            if (OnlyLegs)
            {
                left = _activeRagdoll.GetPhysicalBone(HumanBodyBones.LeftFoot).gameObject;
                right = _activeRagdoll.GetPhysicalBone(HumanBodyBones.RightFoot).gameObject;  

            }
            else
            {
                left = _activeRagdoll.GetPhysicalBone(HumanBodyBones.LeftHand).gameObject;
                right = _activeRagdoll.GetPhysicalBone(HumanBodyBones.RightHand).gameObject;
            }

            (_leftGrip = left.AddComponent<Gripper>()).GripMod = this;
            (_rightGrip = right.AddComponent<Gripper>()).GripMod = this;
        }

        public void UseLeftGrip(float weight) {
            _leftGrip.enabled = weight > leftArmWeightThreshold;
        }

        public void UseRightGrip(float weight) {
            _rightGrip.enabled = weight > rightArmWeightThreshold;
        }
    }
} // namespace ActiveRagdoll