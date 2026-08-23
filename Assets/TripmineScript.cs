using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripmineScript : MonoBehaviour
{
    public GameObject player;
    public Animator animator;
    // Start is called before the first frame update

    void Start()
    {
        animator.GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            animator.Play("TripmineExplosion");
            Destroy(gameObject, 0.2f);
        }
    }

}
