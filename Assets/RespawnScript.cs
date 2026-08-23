using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    public GameObject gameManagerScript;
    public Transform respawnFlagTransform;
    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript.GetComponent<GameManagerScript>();
        respawnFlagTransform = gameManagerScript.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
 