using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : MonoBehaviour
{

    public float radius = 2f;
    Transform interactionPoint;
    Transform interactionObject;
    protected GameObject player;
    public virtual void Interaction()
    {

    }

    private void Start()
    {
        interactionObject = transform;
        player = PlayerManager.instance.player;
        interactionPoint = player.transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(interactionObject.position, interactionPoint.position);
        if (distance <= radius)
        {
            Interaction();
        }
    }
}
