using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy_Improved : MonoBehaviour
{
    [SerializeField] float detectionRange;
    [SerializeField] GameObject player;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float MaxRange;
    [SerializeField] float minRange;
    [SerializeField] NavMeshAgent myAgent;
    [SerializeField] Animator myAnimator;
    [SerializeField] Character_Behaviour_Improved char_behaviour;
    [SerializeField] Slider hp_slider;
    public GameObject deathparticles;
    public GameObject burningEffect;
    GameObject burning_effect;
    GameObject death;
    public float health;
    private float max_Health;
    private float distancetoplayer;
    private float distancetospawn;
    private float AttackTimer = 0;
    private float StunTimer = 0;
    private float DrTimer = 0;
    private int StunCounter = 0;
    private float AttackSpeed = 1f;
    private float myDamage;
    private float BurningDuration = 0;
    private float BurnTick = 0;
    private float crit_chance;
    public bool Stunned = false;
    private bool isStuned = false;
    public bool burning = false;
    private Vector3 spawn;
    public bool TargetFound = false;
    bool TooFar = false;
    bool givendestiny = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        char_behaviour = player.GetComponent<Character_Behaviour_Improved>();
        myAgent = gameObject.GetComponent<NavMeshAgent>();
        myAnimator = gameObject.GetComponentInChildren<Animator>();
        spawn = transform.position;
        max_Health = Random.Range(1000, 1800);
        hp_slider.maxValue = max_Health;
        hp_slider.value = max_Health;
        health = max_Health;
        myDamage = Random.Range(150,230);
        detectionRange = Random.Range(10, 25);
        crit_chance = Random.Range(5, 25);
        if (player != null)
        {
            StartCoroutine(GetDistanceToPlayer());
        }
        else
            Debug.Log("Falta Player");
    }
    void Update()
    {

        AttackTimer += Time.deltaTime;
        if (health > 0)
        {
            if(burning)
            {
                if (burning_effect == null)
                {
                    burning_effect = (GameObject)Instantiate(burningEffect, transform.position + transform.up, Quaternion.identity);
                }
                else
                {
                    burning_effect.transform.position = transform.position + transform.up;
                }
                BurningDuration += Time.deltaTime;
                BurnTick += Time.deltaTime;
                if(BurningDuration <= 7f)
                {
                    if(BurnTick >= 1f)
                    {
                        float nextDamage = Random.Range(max_Health / 90, max_Health / 75);
                        TakeDamage(nextDamage);
                        BurnTick = 0;
                    }
                } else
                {
                    BurningDuration = 0;
                    Destroy(burning_effect);
                    burning = false;
                }
            }
            if (StunCounter < 3)
            {
                if (!isStuned)
                {
                    if (Stunned)
                    {
                        StunTimer = 0;
                        myAnimator.SetBool("Stunner", true);
                        StunCounter++;
                        isStuned = true;
                    }
                }
            }
            else
            {
                DrTimer += Time.deltaTime;
                if (DrTimer >= 30f)
                {
                    StunCounter = 0;
                }
                if (Stunned)
                {
                    Stunned = false;
                }
            }
            if (!isStuned)
            {
                if (TargetFound)
                {
                    if (myAnimator.GetBool("Idle"))
                    {
                        myAnimator.SetBool("Idle", false);
                    }
                    SetDestination(player.transform.position);
                    StartCoroutine(GetDistanceToSpawn());
                    distancetoplayer = Vector3.Distance(transform.position, player.transform.position);
                    if (distancetoplayer <= myAgent.stoppingDistance)
                    {
                        if (myAnimator.GetBool("Chase") == true)
                        {
                            myAnimator.SetBool("Chase", false);
                        }
                        myAnimator.SetBool("Range", true);
                        if (AttackTimer >= AttackSpeed)
                        {
                            int crit_generator = Random.Range(1, 100);
                            if (crit_generator < crit_chance)
                            {
                                char_behaviour.TakeDamage(myDamage * 2);
                                AttackTimer = 0;
                            }
                            else
                            {
                                char_behaviour.TakeDamage(myDamage);
                                AttackTimer = 0;
                            }
                        }
                    }
                    else
                    {
                        if (myAnimator.GetBool("Range") == true)
                        {
                            myAnimator.SetBool("Range", false);
                        }
                        myAnimator.SetBool("Chase", true);
                        AttackTimer = 0;
                    }
                }
                else if (TooFar)
                {
                    if (!givendestiny)
                    {
                        SetDestination(spawn);
                    }
                }
                else
                {
                   //PatrolSystem
                }
            }
            else
            {

                StunTimer += Time.deltaTime;
                Stunned = false;
                if (StunCounter == 1)
                {
                    if (StunTimer >= 5f)
                    {
                        myAnimator.SetBool("Stunner", false);
                        isStuned = false;
                    }
                }
                if (StunCounter == 2)
                {
                    if (StunTimer >= 3f)
                    {
                        myAnimator.SetBool("Stunner", false);
                        isStuned = false;
                    }
                }
                if (StunCounter == 3)
                {
                    if (StunTimer >= 1f)
                    {
                        myAnimator.SetBool("Stunner", false);
                        isStuned = false;
                    }
                }
            }
        } else
        {
                Die();
        }
    }
    IEnumerator GetDistanceToPlayer()
    {
        while (!TargetFound)
        {
            yield return new WaitForSeconds(0.5f);
            distancetoplayer = Vector3.Distance(transform.position, player.transform.position);
            if(distancetoplayer <= detectionRange)
            {
                if (!Physics.Raycast(transform.position + transform.up, player.transform.position,distancetoplayer,obstacleMask))
                {
                    TargetFound = true;
                }
            }
        }
    }
    IEnumerator GetDistanceToSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.7f);
            distancetospawn = Vector3.Distance(transform.position, spawn);
            if (distancetospawn >= MaxRange)
            {
                TargetFound = false;
                TooFar = true;
            }
            if(TooFar)
            {
                if(distancetospawn <= minRange)
                {
                    myAnimator.SetBool("Idle", true);
                    myAnimator.SetBool("Chase", false);
                    myAnimator.SetBool("Range", false);
                    StartCoroutine(GetDistanceToPlayer());
                    TooFar = false;
                    break;
                }
            }
        }
    }
    void SetDestination(Vector3 destination)
    {
        myAgent.SetDestination(destination);
        if (TooFar) { givendestiny = true; }
    }
    public void TakeDamage(float Damage)
    {
        if(health != 0)
        {
            health -= Damage;
            hp_slider.value = health;
        }
    }
    void Die()
    {
        if (burning)
        {

            Destroy(burning_effect, 2f);
        }
        myAgent.speed = 0;
        myAnimator.SetBool("Death", true);
        if (death == null)
        {
            death = (GameObject)Instantiate(deathparticles, transform.position + transform.up, Quaternion.identity);
        }
        Destroy(death, 2.3f);
        Destroy(gameObject, 2.4f);
    }
}
