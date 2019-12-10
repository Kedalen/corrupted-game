using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant_Raycasts : MonoBehaviour
{
    public CharacterController controller;
    public LayerMask groundMask;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {

        } else
        {
            RaycastHit hit_info;
            if (Physics.Raycast(transform.position, -transform.up,out hit_info,500f,groundMask)){
                float distance_to_ground = Vector3.Distance(transform.position, hit_info.point);
                if (distance_to_ground <= 40f)
                {
                    Debug.Log("NP");
                }
                else
                {
                    Debug.Log("Muere");
                }
            }
        }
    }
}
