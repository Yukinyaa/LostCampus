using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Wander : NetworkBehaviour { 

    public float wanderRadius;
    public float wanderTimer;
    public bool isChase;
    public bool isAttack;
    public GameObject target;

    private NavMeshAgent agent;
    private float timer;
    private float distance;
    private Animator anim;
    private Rigidbody rigid;
    private Weapon weapon;
    

    // Use this for initialization
    void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        anim = GetComponent<Animator>();
        this.weapon = GetComponentInChildren<Weapon>();

    }

    // Update is called once per frame
    void Update()
    {
        target = FindTarget();
        if (target)
            distance = Vector3.Distance(target.transform.position, transform.position);

        Debug.Log(target.tag);
        timer += Time.deltaTime;
        if (distance < 8)
        {
     

            anim.SetBool("isWalk", true);
            agent.SetDestination(target.transform.position); 
            isChase = true;

            if (isAttack)
            {
                agent.isStopped = true;
            }
            else
                agent.isStopped = false;


        }

        else
        {
            isChase = false;
            if (timer >= wanderTimer)
            {
                anim.SetBool("isWalk", true);
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }

    GameObject FindClosestPlayer()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    GameObject FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);

        GameObject targetGameObject = null;

        float temp = 0;

        float shortTemp = 10;

        for (int i = 0; i < hitColliders.Length; i++)
        {

            if (hitColliders[i].tag == "Player")
            {

                temp = Vector3.Distance(hitColliders[i].transform.position, transform.position);

                if (temp < shortTemp)
                {

                    targetGameObject = hitColliders[i].gameObject;

                }

            }

        }

        return targetGameObject;
    }
    
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        float targetRadius = 0.5f;
        float targetRange = 1f;

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        weapon.Set(true);
        yield return new WaitForSeconds(0.2f);
        
        yield return new WaitForSeconds(1f);
        
        yield return new WaitForSeconds(1f);
        weapon.Set(false);
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
