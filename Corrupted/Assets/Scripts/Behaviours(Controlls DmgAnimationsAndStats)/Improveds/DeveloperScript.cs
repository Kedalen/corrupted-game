using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveloperScript : MonoBehaviour
{
    public GameObject enemy;
    GameObject instance;
    public GameObject PLAYER;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit_info;
            if (Physics.Raycast(ray, out hit_info, Mathf.Infinity)) {
                instance = (GameObject)Instantiate(enemy, hit_info.point, Quaternion.identity);
                Enemy_Behaviour_Improved eby = instance.GetComponent<Enemy_Behaviour_Improved>();
                eby.player_not_found = PLAYER.transform;
                    }
        }
    }
}
