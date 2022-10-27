using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag+" "+ other.name);
        if (other.tag == "Player")
        {
            DefaultBehaviour var;
            if (other.transform.root.TryGetComponent<DefaultBehaviour>(out var))
            {
                var.OnOutofBounds();
            }
            else
            {
            }
        }
    }
}
