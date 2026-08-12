using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public int score;
    public float timer;
    public bool isGameOver;
    public bool playerWon;
    public bool playerLost;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 300)
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
                isGameOver = true;
                Debug.Log("You lose..");
            }

        }
    }
}

