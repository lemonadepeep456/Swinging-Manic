using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    // Static variable keeps the position saved globally across checkpoints
    public static Vector2 LastCheckpointPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the trigger is tagged as "Player"
        if (collision.CompareTag("Player"))
        {
            // Store this checkpoint's exact position
            LastCheckpointPosition = transform.position;
            Debug.Log("Checkpoint Activated: " + LastCheckpointPosition);
        }
    }
}
