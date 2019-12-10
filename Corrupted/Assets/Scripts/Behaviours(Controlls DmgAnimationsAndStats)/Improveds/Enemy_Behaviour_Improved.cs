using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;



public class Enemy_Behaviour_Improved : MonoBehaviour
{
    //LineOfSight
    [HideInInspector]
    public float viewRadius;
    [HideInInspector]
    public float viewAngle;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public Transform target;
    //Range 
    private float maxRange;
    private Vector3 spawn;
    //Movement
    private NavMeshAgent myAgent;
    public Transform player_not_found;
    public Animator myAnimator;
    float DistanceToPlayer;
    //StateController
    public enum State { combat, nocombat, chasing, maxrange,stuned};
    public State state = State.nocombat;
    public Character_Behaviour_Improved char_behaviour;
    //Timers
    private float AttackTimer = 0.0f;
    private float Dr_Timer = 0.0f;
    private float Burning_Timer = 0.0f;
    private float healing_timer = 0;
    private float Burning_Tick_Timer = 0.0f;
    //Stats
    private float crit_chance;
    public bool Stun = false;
    private float max_hp;
    private float current_hp;
    private float minRange = 5f;
    private float DamageIDeal;
    private float Speed;
    int negative_decider;
    private int stuncount = 0;
    float stuntimer = 0;
    float dodge_chance = 0;
    //Effects
    public GameObject deathparticles;
    GameObject death;
    public GameObject burningEffect;
    GameObject burning_effect;
    //HP BAR
    public Slider hp_slider;
    public bool isburning = false;
    float rotation_direction;

    private void Start()
    {
        myAgent = gameObject.GetComponent<NavMeshAgent>();
        Speed = Random.Range(3, 8);
        myAgent.speed = Speed;
        spawn = transform.position;
        //RandomStats
        crit_chance = Random.Range(1, 100);
        max_hp = Random.Range(1000, 1350);
        DamageIDeal = Random.Range(75, 150);
        dodge_chance = Random.Range(1, 100);
        maxRange = Random.Range(15, 40);
        viewRadius = Random.Range(10, maxRange);
        viewAngle = Random.Range(35, 60);
        current_hp = max_hp;
        rotation_direction = Random.Range(10,40);
        negative_decider = Random.Range(1, 10);
        if (negative_decider == 1)
        {
            rotation_direction *= -1;
        }
        hp_slider.maxValue = max_hp;
        hp_slider.value = max_hp;
    }
    private void Update()
    {
        Debug.Log(myAnimator);

        if (current_hp <= 0)
        {
            myAgent.speed = 0;
            myAnimator.SetBool("Stunner", false);
            myAnimator.SetBool("Death", true);
            hp_slider.value = 0;
            Die();
        }
        else
        {
            if (isburning)
            {
                Burning_Timer += Time.deltaTime;
                Burning_Tick_Timer += Time.deltaTime;
                if (burning_effect == null)
                {
                    burning_effect = (GameObject)Instantiate(burningEffect, transform.position + transform.up, Quaternion.identity);
                }else
                {
                    burning_effect.transform.position = transform.position + transform.up;
                }

                if (Burning_Timer <= 7f)
                {
                    if (Burning_Tick_Timer >= 1f)
                    {
                        float NextDamage = Random.Range(20, 75);
                        ReciveDmg(NextDamage);
                        Burning_Tick_Timer = 0;
                    }
                }
                else
                {
                    Destroy(burning_effect);
                    burning_effect = null;
                    isburning = false;
                }
            }
            else
            {
                Burning_Tick_Timer = 0;
                Burning_Timer = 0;
            }
            if (stuncount <= 3)
            {
                stuntimer += Time.deltaTime;
                Dr_Timer = 0;
                if (Stun)
                {
                    if (target == null)
                    {
                        target = player_not_found;
                    }
                    state = State.stuned;
                    stuncount++;
                    stuntimer = 0;
                    Stun = false;
                }
            }
            else
            {
                Dr_Timer += Time.deltaTime;
                if (Dr_Timer >= 30)
                    stuncount = 0;
            }
            hp_slider.value = current_hp;
            float distancetospawn = Vector3.Distance(transform.position, spawn);

            if (state != State.stuned)
            {
                if (distancetospawn >= maxRange)
                {
                    myAgent.SetDestination(spawn);
                    state = State.maxrange;
                }
            }
            if (target != null)
            {
                DistanceToPlayer = Vector3.Distance(transform.position, target.position);
            }
            switch (state)
            {
                case State.nocombat: //Looking for players
                    StartCoroutine("FindPlayer", 0.2f);
                    transform.RotateAround(transform.position, Vector3.up, rotation_direction * Time.deltaTime);
                    if (current_hp != max_hp)
                    {
                        healing_timer += Time.deltaTime;
                        if(healing_timer >= 1)
                        {
                            current_hp += max_hp / 20;
                            healing_timer = 0;
                        }
                    }
                    break;
                case State.combat: //In Range To Attack.
                    myAnimator.SetBool("Stunner", false);
                    FacePlayer();
                    myAnimator.SetBool("Range", true);
                    AttackTimer += Time.deltaTime;
                    if (AttackTimer >= 1f)
                    {
                        int Crited = Random.Range(1, 100);
                        if (Crited <= crit_chance)
                        {

                            float Damage = DamageIDeal * 2;
                            char_behaviour.TakeDamage(Damage);
                            Debug.Log("DmgDealt");
                        }
                        else
                        {
                            float Damage = DamageIDeal * 2;
                            char_behaviour.TakeDamage(Damage);
                            Debug.Log("DmgDealt");

                        }
                        AttackTimer = 0f;
                    }
                    if (DistanceToPlayer > myAgent.stoppingDistance)
                    {
                        myAnimator.SetBool("Range", false);
                        state = State.chasing;
                    }
                    break;
                case State.chasing: //Player is out of range
                    myAnimator.SetBool("Stunner", false);
                    StopCoroutine("findPlayer");
                    target = player_not_found;
                    myAnimator.SetBool("Chase", true);
                    myAgent.SetDestination(target.position);
                    if (DistanceToPlayer <= myAgent.stoppingDistance)
                    {
                        myAnimator.SetBool("Chase", false);
                        state = State.combat;
                    }
                    break;
                case State.maxrange: //Reached max range
                    myAnimator.SetBool("Stunner", false);

                    myAnimator.SetBool("Chase", true);
                    if (distancetospawn <= minRange)
                    {
                        myAnimator.SetBool("Chase", false);
                        transform.LookAt(target);
                        state = State.nocombat;
                    }
                    break;
                case State.stuned: //Enemy got stunned
                    myAgent.speed = 0;
                    myAnimator.SetBool("Stunner", true);
                    if (stuncount == 1)
                    {
                        if (stuntimer >= 3f)
                        {
                            myAgent.speed = Speed;
                            state = State.chasing;
                        }
                    }
                    if (stuncount == 2)
                    {
                        if (stuntimer >= 2f)
                        {
                            myAgent.speed = Speed;
                            state = State.chasing;
                        }
                    }
                    if (stuncount == 3)
                    {
                        if (stuntimer >= 1f)
                        {
                            myAgent.speed = Speed;
                            state = State.chasing;
                        }
                    }
                    break;
            }
        }
    }
    IEnumerator FindPlayer(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    void FindVisibleTargets()
    {
        Collider[] playersInView = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for(int i = 0; i<playersInView.Length; i++)
        {
             target = playersInView[i].transform;
            Vector3 dirToTarget = (target.position - transform.position);
            if (Vector3.Angle (transform.forward,dirToTarget)< viewAngle /2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position + transform.up * 2, dirToTarget, distToTarget, obstacleMask))
                {
                    char_behaviour = target.GetComponent<Character_Behaviour_Improved>();
                    state = State.chasing;
                }
            }
        }
    }
    public Vector3 DirFromAngles(float angle, bool GlobalAngle)
    {
        if (!GlobalAngle)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
    void FacePlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); 
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }
    public void ReciveDmg(float Damage)
    {
        int Random_dodge = Random.Range(1, 1);
        if(Random_dodge <= dodge_chance)
        {
            current_hp -= Damage;
        } else
        {
            Debug.Log("Dodged");
        }
    }
    void Die()
    {
        if (burning_effect != null)
        {
            Destroy(burning_effect,1f);
        }
        if(death == null)
        {
            death = (GameObject)Instantiate(deathparticles, transform.position + transform.up, Quaternion.identity);
        }
        Destroy(death, 2.3f);
        Destroy(gameObject, 2.4f);
    }
}
