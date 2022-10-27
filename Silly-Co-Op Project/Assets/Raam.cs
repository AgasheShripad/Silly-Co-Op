using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raam : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Breakable")
        {
            if (collision.transform.TryGetComponent<HingeJoint>(out var hingeJoint))
            {
                if(hingeJoint != null)
                {
                    Debug.Log("Door Broken");
                    hingeJoint.breakForce = 1;
                }
            }
        }
    }
}
