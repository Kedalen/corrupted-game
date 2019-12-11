using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    private Enemy_Improved enemy;
    public float myDamage;
    public GameObject destroyed_Prefab;
    private GameObject destroyed;
    public bool NoDestroy = false;
    public float LifeTime = 5f;
    public bool Undestructible = false;
    private void Start()
    {
        Destroy(gameObject, LifeTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag !="Player" && other.transform.tag !="Bullet")
        {
            if (!NoDestroy)
            {
                destroyed = (GameObject)Instantiate(destroyed_Prefab, transform.position, Quaternion.Lerp(destroyed_Prefab.transform.localRotation, transform.rotation, 0.1f));
                Destroy(destroyed, 0.4f);
                Destroy(gameObject);
            }else if (Undestructible)
            {
                Debug.Log("Will Live for Full Live Time");
            } else
            {
                Destroy(gameObject, 2f);
            }
        }
        if (other.transform.tag == "BurnIt" && transform.tag == "Basic")
        {

        }
        if(other.transform.tag == "Enemy")
        {
            enemy = other.transform.GetComponent<Enemy_Improved>();
            enemy.TakeDamage(myDamage);
            enemy.TargetFound = true;
        }

        
    }
}
