using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyTarget : Target
{
    public GameObject canvas;
    public Slider slider;
    public NavMeshAgent agent;
    public Animator animator;

    public float knockForce = 0.5f;

    bool tookDamage = false;

    private void FixedUpdate()
    {
        if (tookDamage && health > 0)
        {
            transform.Translate(Vector3.forward * knockForce, Camera.main.transform);
            agent.velocity = new Vector3(0, 0, 0);
            tookDamage = false;
        }
    }
    public override void TakeDamage(float amount)
    {
        tookDamage = true;
        health -= amount;

        slider.value = health / initialHealth;

        if (health <= 0f)
        {
            StartCoroutine(WaitForDie());
            
        }
    }
    
    IEnumerator WaitForDie()
    {
        animator.SetBool("Death", true);
        agent.updatePosition = false;
        agent.updateRotation = false;
        GetComponent<CapsuleCollider>().enabled = false;

        yield return new WaitForSeconds(3f);

        Die();
    }

    private void Update()
    {
        HideInfo();
        animator.SetFloat("VelocityY", agent.velocity.magnitude);
    }

    private void LateUpdate()
    {
        canvas.transform.LookAt(Camera.main.transform);
        canvas.transform.Rotate(0, 180, 0);
    }

    public void HideInfo()
    {
        canvas.SetActive(false);
    }

    public void ShowInfo()
    {
        canvas.SetActive(true);
    }


}
