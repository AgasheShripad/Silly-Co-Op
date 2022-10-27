using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActiveRagdoll {
    

    public class Grippable : MonoBehaviour {

        public bool InActiveTillTouched = false;


        public JointMotionsConfig jointMotionsConfig;
        private Rigidbody Rb;

        public void Start()
        {
            if (InActiveTillTouched)
            {
                Rb = this.GetComponent<Rigidbody>();
                Rb.isKinematic = true;
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (InActiveTillTouched)
            {
                if (collision.transform.tag == "Player1" || collision.transform.tag == "Player2")
                {
                    Rb.isKinematic = false;
                }
            }
        }
    }
} 