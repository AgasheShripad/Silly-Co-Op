using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAnvel : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Anvel")
        {
            other.GetComponentInParent<Rigidbody>().isKinematic = true;
            this.GetComponentInParent<Rigidbody>().isKinematic = true;
        }
    }
}
