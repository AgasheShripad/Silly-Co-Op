using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raam : MonoBehaviour
{
    [SerializeField]
    public float RamForce = 5;
    [SerializeField]
    private bool heldByBoth = false;
    [SerializeField]
    private int PlayerHeldcount = 0;


    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
         if (collision.transform.tag == "Breakable" && heldByBoth)
        {
            if (collision.transform.TryGetComponent<HingeJoint>(out var hingeJoint) && collision.transform.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                if(hingeJoint != null && rigidbody !=null)
                {
                    Debug.Log("Door Broken");
                    hingeJoint.breakForce = 1;
                    rigidbody.isKinematic = false;
                    rigidbody.AddForce(collision.contacts[0].point*RamForce);
                }
            }
        }


        if (collision.transform.tag == "Player1" || collision.transform.tag == "Player2")
        {
            PlayerHeldcount++;
        }
        if (PlayerHeldcount >= 2) heldByBoth = true;
        else heldByBoth = false;
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Player1" || collision.transform.tag == "Player2")
        {
            PlayerHeldcount--;
        }
    }
}
