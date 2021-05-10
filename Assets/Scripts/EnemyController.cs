using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public float walkRadius = 5f;
    public float timeToReset = 3f;
    public float activeRadius = 2f;
    // Update is called once per frame
    bool hasDestination = false;
    Vector3 finalPosition;
    Vector3 randomDirection;
    Vector3 previousPosition;
    Transform target;
    NavMeshHit hit;
    float stuckTimeStart;
    float timeSinceStuck;
    bool stuck = false;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activeRadius);
    }

    private void Awake()
    {
        target = PlayerManager.instance.player.transform;
    }
    void Update()
    {

        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= activeRadius)
        {
            agent.SetDestination(target.position);
            agent.speed = 8;
            hasDestination = true;
        }

        if (!hasDestination || timeSinceStuck > timeToReset)
        {
            randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += transform.position;
            NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
            finalPosition = hit.position;
            agent.SetDestination(finalPosition);
            hasDestination = true;
            stuck = false;
            timeSinceStuck = 0;
            agent.speed = 4;
        }

        if (previousPosition == transform.position)
        {
            if (!stuck)
            {
                stuckTimeStart = Time.time;
                stuck = true;
            } else
            {
                timeSinceStuck = Time.time - stuckTimeStart;
            }
        }

        if (transform.position.x == finalPosition.x && transform.position.z == finalPosition.z)
        {
            hasDestination = false;
        }

        previousPosition = transform.position;
    }
}
