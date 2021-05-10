using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : InteractionObject
{

    PlayerController playerController;
    public override void Interaction()
    {
        base.Interaction();

        if (Input.GetButtonDown("Interaction"))
        {
            playerController = player.GetComponent<PlayerController>();

            if (playerController)
            {
                System.Random rand = new System.Random();
                int index = rand.Next(0, 3);
                playerController.auraGround
                    .GetComponent<ParticleSystemRenderer>().material = playerController.auraMaterialsGround[index];
                playerController
                    .auraSpark.GetComponent<ParticleSystemRenderer>().material = playerController.auraMaterialsSpark[index];
            }

            Destroy(gameObject);
        }
    

    }
}
