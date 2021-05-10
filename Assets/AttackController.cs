using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public GameObject projectile;
    public Transform firePoint;
    public Animator animator;

    GameObject parabolaStart;
    GameObject parabolaMiddle;
    GameObject parabolaEnd;
    GameObject parabolaRoot;
    float weight;
    // Start is called before the first frame update
    void Start()
    {
        parabolaStart = new GameObject("Point");
        parabolaMiddle = new GameObject("Point");
        parabolaEnd = new GameObject("Point");

        parabolaRoot = new GameObject("ParabolaRoot");
        parabolaRoot.transform.parent = transform.parent;
        parabolaStart.transform.parent = parabolaRoot.transform;
        parabolaMiddle.transform.parent = parabolaRoot.transform;
        parabolaEnd.transform.parent = parabolaRoot.transform;
        
    }

    IEnumerator ChangeLayerWeightSmooth(int layerIndex, float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            weight = Mathf.Lerp(v_start, v_end, elapsed / duration);
            animator.SetLayerWeight(layerIndex, weight);
            elapsed += Time.deltaTime;
            yield return null;
        }
        weight = v_end;
        animator.SetLayerWeight(layerIndex, weight);
    }

    IEnumerator waitForAttack()
    {
        animator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(2);

       // animator.SetLayerWeight(2, 0);
        StartCoroutine(ChangeLayerWeightSmooth(2, 1.0f, 0.0f, 0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetBool("isAttacking", true);
            // animator.SetLayerWeight(2, 1);

            StartCoroutine(ChangeLayerWeightSmooth(2, 0.0f, 1.0f, 0.5f));
            var projectileInstance = Instantiate(projectile, firePoint.position, Quaternion.Euler(0f, 0f, 0f), transform);
            ParabolaController parabolaController = projectileInstance.GetComponent<ParabolaController>();

            parabolaStart.transform.position = firePoint.position;
            parabolaMiddle.transform.position = firePoint.position + new Vector3(0, 3, 2);
            parabolaEnd.transform.position = firePoint.position + new Vector3(0, 0, 4);

            if (parabolaController)
            {
                parabolaController.CreateParabola(parabolaRoot);
            }
        } else if (Input.GetButtonUp("Fire1"))
        {
            StartCoroutine(waitForAttack());
        }
    }
}
