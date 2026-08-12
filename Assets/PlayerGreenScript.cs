using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerGreenScript : MonoBehaviour
{
    public Vector3 leftMoveForce;
    public Vector3 rightMoveForce;
    public Vector3 jumpForce;
    public GameObject leftProjectilePrefab;
    public GameObject rightProjectilePrefab;
    public Vector3 leftProjectileOffset;
    public Vector3 rightProjectileOffset;
    public int playerFacing;
    public bool canJump;
    public bool doubleJump;
    public bool isWalking;
    public bool isJumping;
    public bool isOnGround;
    public bool LeapCooldown;
    public float timer;
    public GameObject gameManagerObject;
    public Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {
        isWalking = false;
        isOnGround = false;
        LeapCooldown = false;
        leftMoveForce.x = -5f;
        rightMoveForce.x = 5f;
        jumpForce.y = 400f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 30)
            Destroy(gameObject);


        if (Input.GetKey(KeyCode.D)) //&& isOnGround == true)
        {
            playerFacing = 1;
            isWalking = true;
            GetComponent<Rigidbody2D>().AddForce(rightMoveForce);
            GetComponent<Animator>().Play("PlayerWalk");
            GetComponent<SpriteRenderer>().flipX = false;
        }

        if (Input.GetKey(KeyCode.A)) // isOnGround == true)
        {
            playerFacing = -1;
            isWalking = true;
            GetComponent<Rigidbody2D>().AddForce(leftMoveForce);
            GetComponent<Animator>().Play("PlayerWalk");
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (playerFacing == -1 && isWalking == false && isJumping == false)
        {
            isWalking = false;
            GetComponent<Animator>().Play("PlayerIdle");
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (playerFacing == 1 && isWalking == false && isJumping == false)
        {
            isWalking = false;
            GetComponent<Animator>().Play("PlayerIdle");

            GetComponent<SpriteRenderer>().flipX = false;
        }

        {
        }



        if (Input.GetKeyDown(KeyCode.W)) //&& isOnGround == true)
        {

            if (canJump == true)
            {
                canJump = false;
                GetComponent<Rigidbody2D>().AddForce(jumpForce);
                //GetComponent<Animator>().Play("PlayerJump");
                // isWalking = false;
                isOnGround = false;
                StartCoroutine(Jumping());
            }
            else if (doubleJump == true)
            {
                //  isWalking = false;

                doubleJump = false;
                GetComponent<Rigidbody2D>().AddForce(jumpForce);

            }
            if (playerFacing == 1)
            {
                Instantiate(rightProjectilePrefab, GetComponent<Transform>().position + rightProjectileOffset,
                    Quaternion.identity);
            }
            if (playerFacing == -1)
            {
                Instantiate(leftProjectilePrefab, GetComponent<Transform>().position + leftProjectileOffset,
                    Quaternion.identity);
            }
            if (GetComponent<Transform>().position.y <= -10f)
            {
                gameManagerObject.GetComponent<GameManagerScript>().playerLost = true;
                Destroy(gameObject);
            }
            //  else if (canJump == false)//(canJump == false && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D) && (playerFacing == 1))
            // {
            //     GetComponent<Animator>().Play("PlayerJump");
            // }




            IEnumerator Jumping()
            {
                // yield return new WaitForSeconds(1f);
                // while (isOnGround == false)
                //  {

                GetComponent<Animator>().Play("PlayerJump");
                yield return new WaitForSeconds(1f);
                GetComponent<Animator>().Play("PlayerIdle");
                Debug.Log("Jumping!");
                // yield return null;

                // }


            }

        }
        IEnumerator DoubleJumping()
        {
            // yield return new WaitForSeconds(1f);
            // while (isOnGround == false)
            //  {

            GetComponent<Animator>().Play("PlayerJump");
            yield return new WaitForSeconds(2f);
            //  GetComponent<Animator>().Play("PlayerIdle");
            Debug.Log("DoubleJumping!");
            // yield return null;

            // }


        }
        IEnumerator LeapingLeft()
        {
            // yield return new WaitForSeconds(1f);
            // while (isOnGround == false)
            //  {

            GetComponent<Animator>().Play("PlayerLeap");
            leftMoveForce.x += -150f;
            jumpForce.y += 300f;
            isOnGround = false;
            LeapCooldown = true;
            yield return new WaitForSeconds(1f);
          //  isOnGround = true;
            GetComponent<Animator>().Play("PlayerIdle");
            leftMoveForce.x += 150f;
            jumpForce.y -= 300f;
            isOnGround = true;
            Debug.Log("Leaping");
            LeapCooldown = false;
            // yield return null;

            // }


        }
        IEnumerator LeapingRight()
        {
            // yield return new WaitForSeconds(1f);
            // while (isOnGround == false)
            //  {

            GetComponent<Animator>().Play("PlayerLeap");
        
            rightMoveForce.x += 150f;
            isOnGround = false;
            LeapCooldown = true;
            jumpForce.y += 300f;
            yield return new WaitForSeconds(1f);
            //  isOnGround = true;
            GetComponent<Animator>().Play("PlayerIdle");
            rightMoveForce.x -= 150f;
            jumpForce.y -= 300f;
            isOnGround = true;
            Debug.Log("Leaping");
            LeapCooldown = false;
            // yield return null;

            // }


        }
        if (Input.GetKeyDown(KeyCode.W) && doubleJump == true)
        {
            StartCoroutine(DoubleJumping());
        }
        if (Input.GetKeyDown(KeyCode.W) && (Input.GetKey(KeyCode.A) && LeapCooldown == false))
        {
         
            StartCoroutine(LeapingLeft());
            GetComponent<Rigidbody2D>().AddForce(leftMoveForce);
            //  GetComponent<Animator>().Play("PlayerLeap");

        }
        if (Input.GetKeyDown(KeyCode.W) && (Input.GetKey(KeyCode.D) && LeapCooldown == false))
        {

            StartCoroutine(LeapingRight());
            GetComponent<Rigidbody2D>().AddForce(rightMoveForce);
            //  GetComponent<Animator>().Play("PlayerLeap");

        }
    }

private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.tag == "Ground")
    {
            canJump = true;
            isOnGround = true;
    }
    if (collision.gameObject.tag == "PowerUp")
    {
        doubleJump = true;
        Destroy(collision.gameObject);
    }
    if (collision.gameObject.tag == "Gem")
    {
        gameManagerObject.GetComponent<GameManagerScript>().score += 1;
        Destroy(collision.gameObject);
    }

}
}
