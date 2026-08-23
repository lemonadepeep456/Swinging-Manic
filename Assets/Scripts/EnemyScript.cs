using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject gameManagerObject;
    public int health = 3;
    public GameManagerScript gameManagerScript;
    public bool isAttacked;
    public Animator enemyAnimator;
    public Rigidbody2D rb2D;
    public void Start()
    {
        enemyAnimator = GetComponent<Animator>();
      //  rb2D = GetComponent<Rigidbody2D>();
       
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        //  enemyAnimator.Play("LemurAttacked");
        StartCoroutine(AttackedAnim());
        Debug.Log("Enemy took " + damage + " damage!");
       
        if (health <= 0)
        {
          //  rb2D.gravityScale = 1;
            enemyAnimator.Play("LemurFall");
            Destroy(gameObject, 0.5f);
        }
    }
    IEnumerator AttackedAnim()
    {
        if (health >= 0)
        {
            enemyAnimator.Play("LemurAttacked");
            yield return new WaitForSeconds(0.5f);
            enemyAnimator.Play("LemurIdle");
        }
    }


}