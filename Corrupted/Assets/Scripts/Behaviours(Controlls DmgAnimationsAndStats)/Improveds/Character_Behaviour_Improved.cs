using UnityEngine;
using UnityEngine.UI;
public class Character_Behaviour_Improved : MonoBehaviour
{
    //TIMERS
    float Period_Timer = 0;
    float global_cooldown = 0;
    float DarkBeyond_CD = 0;
    float Devour_CD = 0;
    float Burn_CD = 0;
    //TIME ("HP")
    public float stay_period;
    public float max_stay_period;
    //Enemy_Detection
    public LayerMask enemy_mask;
    public Camera main_Cam;
    //HUD ELEMENTS
    public Slider period_slider;
    public Slider energy_slider;
    public GameObject[] skills;
    public GameObject next_skill;
    //STATS
    public float MaxRange;
    public float Energy;
    public float Energy_timer = 0;


    void Start()
    {
        energy_slider.maxValue = Energy;
        period_slider.maxValue = max_stay_period;
        stay_period = period_slider.maxValue;
    }
    void Update()
    {
        energy_slider.value = Energy;
        global_cooldown -= Time.deltaTime; //Keeps track of time no matter the frames of the player
        DarkBeyond_CD -= Time.deltaTime;   //Prevents the player from spaming this skillss--
        Devour_CD -= Time.deltaTime;       
        Burn_CD -= Time.deltaTime;         //--//
        Period_Timer += Time.deltaTime;   
        period_slider.value = stay_period;   
        if (Period_Timer >= 400f)  //Reduces de amount of time remaining of the player by 50 every 400f
        {
            stay_period -= 50;
            Period_Timer = 0;
        }
        if(Energy != 3)            //Recharges the  energy
        {
            Energy_timer += Time.deltaTime;
            if (Energy_timer >= 1f)
            {
                Energy++;
                Energy_timer = 0;
            }

        }
        if (global_cooldown <= 0)   //Prevents the player from spamming diferent skills
        {
            if (Input.GetKey(KeyCode.C))
            {
                BasicAttack();
                Debug.Log(next_skill);
                global_cooldown = 1f;
            }
            if(DarkBeyond_CD <= 0)
            {
                if (Input.GetKey(KeyCode.R))
                {
                    DarkBeyond();
                    global_cooldown = 1f;
                    DarkBeyond_CD = 8f;
                }
            }
            if(Devour_CD <= 0)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    Devouer();
                    global_cooldown = 1f;
                    Devour_CD = 5f;
                }
            }
            if (Burn_CD <= 0)
            {
                if (Input.GetKey(KeyCode.V))
                {
                    Burn();
                    global_cooldown = 1f;
                    Burn_CD = 11f;
                }
            }
            if (Energy >=3)
            {
                if (Input.GetKey(KeyCode.Z))
                {
                    AoEStun();
                    global_cooldown = 1f;
                    Energy = 0;
                }
            }
        }
    }
    void BasicAttack()
    {
        Ray ray = main_Cam.ScreenPointToRay(Input.mousePosition); // Gets the mouse position and throws a raycast to it
        RaycastHit hit_info;
        next_skill = (GameObject)Instantiate(skills[0], transform.position + transform.forward * 1.3f, Quaternion.identity); //Spawns the selected skill
        if (Physics.Raycast(ray, out hit_info, MaxRange))  //Gives the spawnedskill rigidbody the raydirection
        {
            Rigidbody rb = next_skill.GetComponent<Rigidbody>();
            Vector3 rotation = hit_info.point - transform.position;
            Quaternion rotationqtr = Quaternion.LookRotation(rotation);
            next_skill.transform.rotation = Quaternion.Slerp(next_skill.transform.rotation, rotationqtr, 1f); //Rotates the spawned skill to mouse position
            rb.AddForce(ray.direction * 1000);
        } else
        {
            var pos = ray.GetPoint(MaxRange);
            Vector3 rotation = pos - transform.position;
            Quaternion rotationqtr = Quaternion.LookRotation(rotation);
            next_skill.transform.rotation = Quaternion.Slerp(next_skill.transform.rotation, rotationqtr, 1f);
            Rigidbody rb = next_skill.GetComponent<Rigidbody>();
            rb.AddForce(ray.direction * 1000);
        }
    }
    void DarkBeyond()   //HUGE AOE (DEALS MEDIUM DAMAGE)
    {
        Ray ray = main_Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit_info;
   
        if (Physics.Raycast(ray, out hit_info, MaxRange))
        {
            next_skill = (GameObject)Instantiate(skills[1], hit_info.point, Quaternion.identity);
        }
    }
    void Devouer()  //FRONTAL SKILL (DEALS HIGH DAMAGE)
    {
        Ray ray = main_Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit_info;
        next_skill = (GameObject)Instantiate(skills[2], transform.position + transform.forward * 1.3f, Quaternion.identity);
        if (Physics.Raycast(ray, out hit_info, MaxRange))
        {
            Rigidbody rb = next_skill.GetComponent<Rigidbody>();
            next_skill.transform.localRotation = transform.rotation;
            rb.AddForce(ray.direction * 400);
        }
        else
        {
            Rigidbody rb = next_skill.GetComponent<Rigidbody>();
            next_skill.transform.localRotation = transform.rotation;
            rb.AddForce(ray.direction * 400);
        }
    }
    void Burn()   //FRONTAL SKILL (DEALS LOW DAMGE ON IMPACT BUT LEAVES A BURNINGMARK THAT BURNS THE PLAYER)
    {
        Ray ray = main_Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit_info;
        next_skill = (GameObject)Instantiate(skills[3], transform.position + transform.forward * 1.3f, Quaternion.identity);
        if (Physics.Raycast(ray, out hit_info, MaxRange))
        {
            Rigidbody rb = next_skill.GetComponent<Rigidbody>();
            Vector3 rotation = hit_info.point - transform.position;
            Quaternion rotationqtr = Quaternion.LookRotation(rotation);
            next_skill.transform.rotation = Quaternion.Slerp(next_skill.transform.rotation, rotationqtr, 1f);
            rb.AddForce(ray.direction * 200);
        }
        else
        {
            var pos = ray.GetPoint(MaxRange);
            Vector3 rotation = pos - transform.position;
            Quaternion rotationqtr = Quaternion.LookRotation(rotation);
            next_skill.transform.rotation = Quaternion.Slerp(next_skill.transform.rotation, rotationqtr, 1f);
            Rigidbody rb = next_skill.GetComponent<Rigidbody>();
            rb.AddForce(ray.direction * 200);
        }
    }
    void AoEStun()  //STUNS DE ENEMIES THAT ARE FOUND IN A CERTAIN RADIUS OF THE EXPLOSION (DEALS NO DAMAGE AND CONSUMES THE WHOLE ENERGY BAR) 
    {
        Ray ray = main_Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit_info;
        if (Physics.Raycast(ray, out hit_info, MaxRange))
        {
            next_skill = (GameObject)Instantiate(skills[4], hit_info.point, Quaternion.identity);
        }
    }

    public void TakeDamage(float Dmg)  //THIS CAN BE CALLED BY ANY OTHER SCRIPT TO DEAL DAMAGE TO THE PLAYER 
    {
        stay_period= stay_period - Dmg;
    }
}
    