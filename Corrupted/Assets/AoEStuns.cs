using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEStuns : MonoBehaviour
{
    Enemy_Improved enemy;
    private void Start()
    {
        Destroy(gameObject, 2.1f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            enemy = other.transform.GetComponent<Enemy_Improved>();
            enemy.Stunned = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
           

    }
}
