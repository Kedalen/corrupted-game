using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnFromWithIn : MonoBehaviour
{
    Enemy_Improved burning_target;
    private void OnTriggerEnter(Collider other)   
    {
        if(other.tag == "Enemy")
        {
            burning_target = other.GetComponent<Enemy_Improved>();
            burning_target.burning = true;
        }
    }
}
