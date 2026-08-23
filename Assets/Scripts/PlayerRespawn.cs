using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private void Start()
    {
        // Optional: Initialize the checkpoint to the player's starting spot 
        // if they haven't touched a checkpoint flag yet
        if (RespawnScript.LastCheckpointPosition == Vector2.zero)
        {
            RespawnScript.LastCheckpointPosition = transform.position;
        }

        // Send player to the last saved checkpoint position when the game starts/reloads
        transform.position = RespawnScript.LastCheckpointPosition;
    }

    // Call this public function whenever the player dies or falls out of bounds
    public void RespawnPlayer()
    {
        transform.position = RespawnScript.LastCheckpointPosition;

        // Optional: Reset player velocity if using a Rigidbody2D to stop falling momentum
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero; // Note: Use rb.velocity in Unity versions prior to Unity 6
        } // wewew
    }
}
