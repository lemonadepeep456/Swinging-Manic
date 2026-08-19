using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public int score;
    public float timer;

    public GameObject player;
    public Transform playerTransform;
    public Transform respawnPosition;

    public bool isGameOver;
    public bool playerWon;
    public bool playerLost;

    // How far the player can fall before respawning
    public float voidY = -10f;


    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (playerTransform != null &&
            playerTransform.position.y <= voidY)
        {
            playerLost = true;
        }

        if (isGameOver == false)
        {
            if (score >= 6)
            {
                isGameOver = true;
                playerWon = true;

                Debug.Log("You win!");
            }

            if (playerLost == true)
            {
                RespawnPlayer();
            }
        }
    }
    void RespawnPlayer()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is not assigned!");
            return;
        }

        if (respawnPosition == null)
        {
            Debug.LogWarning("Respawn Position is not assigned!");
            return;
        }


 
        playerTransform.position =
            respawnPosition.position;

        Rigidbody2D playerRB =
            playerTransform.GetComponent<Rigidbody2D>();

        if (playerRB != null)
        {
            playerRB.velocity = Vector2.zero;
        }


    
        playerLost = false;


        Debug.Log("Player respawned!");
    }
}
