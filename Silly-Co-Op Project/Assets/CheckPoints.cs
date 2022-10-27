using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    public GameObject SpawnPoint;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Player")
        {
            Debug.Log("Check Point Reached");
            SpawnPoint.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y + 10f, collision.transform.position.z);
            Destroy(this.gameObject);
        }
    }
}
