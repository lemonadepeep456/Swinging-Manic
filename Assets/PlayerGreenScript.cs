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


    // ============================================================
    // ROPE VARIABLES
    // ============================================================

    public Rigidbody2D rb;
    public HingeJoint2D hingeJoint;

    public float ropeSwingForce = 10f;

    public bool isAttached = false;

    public Transform attachedTo;


    // ============================================================
    // ROPE REATTACH COOLDOWN
    // ============================================================

    public float ropeReattachCooldown = 0.5f;

    private float ropeCooldownTimer = 0f;


    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        isWalking = false;
        isOnGround = false;
        LeapCooldown = false;

        leftMoveForce.x = -5f;
        rightMoveForce.x = 5f;
        jumpForce.y = 400f;


        // Get Rigidbody2D
        rb = GetComponent<Rigidbody2D>();


        // Get HingeJoint2D
        hingeJoint = GetComponent<HingeJoint2D>();


        // Make sure the hinge starts disabled
        if (hingeJoint != null)
        {
            hingeJoint.enabled = false;

            hingeJoint.autoConfigureConnectedAnchor = false;

            hingeJoint.connectedAnchor = Vector2.zero;
        }
    }


    // ============================================================
    // UPDATE
    // ============================================================

    void Update()
    {
        timer += Time.deltaTime;


        // ========================================================
        // ROPE REATTACH COOLDOWN
        // ========================================================

        if (ropeCooldownTimer > 0f)
        {
            ropeCooldownTimer -= Time.deltaTime;
        }


        // ========================================================
        // ROPE CONTROLS
        // ========================================================

        // If the player is attached to a rope,
        // use rope controls instead of normal movement.
        if (isAttached)
        {
            CheckRopeControls();

            return;
        }


        // ========================================================
        // NORMAL MOVEMENT
        // ========================================================

        if (Input.GetKey(KeyCode.D))
        {
            playerFacing = 1;

            isWalking = true;

            GetComponent<Rigidbody2D>().AddForce(rightMoveForce);

            GetComponent<Animator>().Play("PlayerWalk");

            GetComponent<SpriteRenderer>().flipX = false;
        }


        if (Input.GetKey(KeyCode.A))
        {
            playerFacing = -1;

            isWalking = true;

            GetComponent<Rigidbody2D>().AddForce(leftMoveForce);

            GetComponent<Animator>().Play("PlayerWalk");

            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (playerFacing == -1 &&
                 isWalking == false &&
                 isJumping == false)
        {
            isWalking = false;

            GetComponent<Animator>().Play("PlayerIdle");

            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (playerFacing == 1 &&
                 isWalking == false &&
                 isJumping == false)
        {
            isWalking = false;

            GetComponent<Animator>().Play("PlayerIdle");

            GetComponent<SpriteRenderer>().flipX = false;
        }


        // ========================================================
        // NORMAL JUMP
        // ========================================================

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (canJump == true)
            {
                canJump = false;

                GetComponent<Rigidbody2D>().AddForce(jumpForce);

                isOnGround = false;

                StartCoroutine(Jumping());
            }
            else if (doubleJump == true)
            {
                doubleJump = false;

                GetComponent<Rigidbody2D>().AddForce(jumpForce);
            }


            // ====================================================
            // PROJECTILE
            // ====================================================

            if (playerFacing == 1)
            {
                Instantiate(
                    rightProjectilePrefab,
                    GetComponent<Transform>().position +
                    rightProjectileOffset,
                    Quaternion.identity
                );
            }


            if (playerFacing == -1)
            {
                Instantiate(
                    leftProjectilePrefab,
                    GetComponent<Transform>().position +
                    leftProjectileOffset,
                    Quaternion.identity
                );
            }


            // ====================================================
            // FALL CHECK
            // ====================================================

            if (GetComponent<Transform>().position.y <= -10f)
            {
                gameManagerObject
                    .GetComponent<GameManagerScript>()
                    .playerLost = true;

                Destroy(gameObject);
            }


            // ====================================================
            // JUMPING COROUTINE
            // ====================================================

            IEnumerator Jumping()
            {
                GetComponent<Animator>().Play("PlayerJump");

                yield return new WaitForSeconds(1f);

                GetComponent<Animator>().Play("PlayerIdle");

                Debug.Log("Jumping!");
            }
        }


        // ========================================================
        // DOUBLE JUMPING
        // ========================================================

        IEnumerator DoubleJumping()
        {
            GetComponent<Animator>().Play("PlayerJump");

            yield return new WaitForSeconds(2f);

            Debug.Log("DoubleJumping!");
        }


        // ========================================================
        // LEFT LEAP
        // ========================================================

        IEnumerator LeapingLeft()
        {
            GetComponent<Animator>().Play("PlayerLeap");

            leftMoveForce.x += -150f;

            jumpForce.y += 300f;

            isOnGround = false;

            LeapCooldown = true;

            yield return new WaitForSeconds(1f);

            GetComponent<Animator>().Play("PlayerIdle");

            leftMoveForce.x += 150f;

            jumpForce.y -= 300f;

            isOnGround = true;

            Debug.Log("Leaping");

            LeapCooldown = false;
        }


        // ========================================================
        // RIGHT LEAP
        // ========================================================

        IEnumerator LeapingRight()
        {
            GetComponent<Animator>().Play("PlayerLeap");

            rightMoveForce.x += 150f;

            isOnGround = false;

            LeapCooldown = true;

            jumpForce.y += 300f;

            yield return new WaitForSeconds(1f);

            GetComponent<Animator>().Play("PlayerIdle");

            rightMoveForce.x -= 150f;

            jumpForce.y -= 300f;

            isOnGround = true;

            Debug.Log("Leaping");

            LeapCooldown = false;
        }


        // ========================================================
        // DOUBLE JUMP INPUT
        // ========================================================

        if (Input.GetKeyDown(KeyCode.W) &&
            doubleJump == true)
        {
            StartCoroutine(DoubleJumping());
        }


        // ========================================================
        // LEFT LEAP INPUT
        // ========================================================

        if (Input.GetKeyDown(KeyCode.W) &&
            (Input.GetKey(KeyCode.A) &&
            LeapCooldown == false))
        {
            StartCoroutine(LeapingLeft());

            GetComponent<Rigidbody2D>().AddForce(leftMoveForce);
        }


        // ========================================================
        // RIGHT LEAP INPUT
        // ========================================================

        if (Input.GetKeyDown(KeyCode.W) &&
            (Input.GetKey(KeyCode.D) &&
            LeapCooldown == false))
        {
            StartCoroutine(LeapingRight());

            GetComponent<Rigidbody2D>().AddForce(rightMoveForce);
        }
    }


    // ============================================================
    // ROPE CONTROLS
    // ============================================================

    private void CheckRopeControls()
    {
        // --------------------------------------------------------
        // SWING LEFT
        // --------------------------------------------------------

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow))
        {
            playerFacing = -1;

            rb.AddRelativeForce(
                Vector2.left * ropeSwingForce
            );
        }


        // --------------------------------------------------------
        // SWING RIGHT
        // --------------------------------------------------------

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            playerFacing = 1;

            rb.AddRelativeForce(
                Vector2.right * ropeSwingForce
            );
        }


        // --------------------------------------------------------
        // CLIMB UP
        // --------------------------------------------------------

        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.UpArrow))
        {
            Slide(1);
        }


        // --------------------------------------------------------
        // CLIMB DOWN
        // --------------------------------------------------------

        if (Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.DownArrow))
        {
            Slide(-1);
        }


        // --------------------------------------------------------
        // DETACH
        // --------------------------------------------------------

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Detach();
        }
    }


    // ============================================================
    // ROPE DETECTION
    // ============================================================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Already attached
        if (isAttached)
        {
            return;
        }


        // Still waiting for cooldown
        if (ropeCooldownTimer > 0f)
        {
            return;
        }


        // Make sure this is a rope
        if (!collision.CompareTag("Rope"))
        {
            return;
        }


        // Get rope segment Rigidbody2D
        Rigidbody2D ropeBone =
            collision.GetComponent<Rigidbody2D>();


        if (ropeBone != null)
        {
            Attach(ropeBone);
        }
    }


    // ============================================================
    // ATTACH TO ROPE
    // ============================================================

    private void Attach(Rigidbody2D ropeBone)
    {
        if (ropeBone == null)
        {
            return;
        }


        if (isAttached)
        {
            return;
        }


        // Get RopeSegment script
        RopeSegment ropeSegment =
            ropeBone.GetComponent<RopeSegment>();


        if (ropeSegment == null)
        {
            Debug.LogWarning(
                "Rope segment does not have a RopeSegment script!"
            );

            return;
        }


        // Tell rope segment player is attached
        ropeSegment.isPlayerAttached = true;


        // Connect player hinge to rope
        hingeJoint.connectedBody = ropeBone;


        // Enable hinge
        hingeJoint.enabled = true;


        // Player is attached
        isAttached = true;


        // Store rope reference
        attachedTo = ropeBone.transform.parent;


        Debug.Log("Player attached to rope.");
    }


    // ============================================================
    // DETACH FROM ROPE
    // ============================================================

    private void Detach()
    {
        if (!isAttached)
        {
            return;
        }


        // Get current rope segment
        Rigidbody2D connectedBody =
            hingeJoint.connectedBody;


        if (connectedBody != null)
        {
            RopeSegment ropeSegment =
                connectedBody.GetComponent<RopeSegment>();


            if (ropeSegment != null)
            {
                ropeSegment.isPlayerAttached = false;
            }
        }


        // Player is no longer attached
        isAttached = false;


        // Disable hinge
        hingeJoint.enabled = false;


        // Remove connected body
        hingeJoint.connectedBody = null;


        // Remove rope reference
        attachedTo = null;


        // ========================================================
        // START REATTACH COOLDOWN
        // ========================================================

        ropeCooldownTimer =
            ropeReattachCooldown;


        Debug.Log(
            "Detached from rope. Reattach cooldown: " +
            ropeReattachCooldown +
            " seconds."
        );
    }


    // ============================================================
    // CLIMB ROPE
    // ============================================================

    public void Slide(int direction)
    {
        if (!isAttached)
        {
            return;
        }


        if (hingeJoint.connectedBody == null)
        {
            return;
        }


        // Get current rope segment
        RopeSegment myConnection =
            hingeJoint.connectedBody
            .GetComponent<RopeSegment>();


        if (myConnection == null)
        {
            return;
        }


        GameObject newSegment = null;


        // ========================================================
        // MOVE UP
        // ========================================================

        if (direction > 0)
        {
            if (myConnection.hingeJoint != null &&
                myConnection.hingeJoint.connectedBody != null)
            {
                GameObject objectAbove =
                    myConnection.hingeJoint
                    .connectedBody
                    .gameObject;


                if (objectAbove.GetComponent<RopeSegment>() != null)
                {
                    newSegment = objectAbove;
                }
            }
        }


        // ========================================================
        // MOVE DOWN
        // ========================================================

        if (direction < 0)
        {
            if (myConnection.hingeJoint != null &&
                myConnection.hingeJoint.connectedBody != null)
            {
                GameObject objectBelow =
                    myConnection.hingeJoint
                    .connectedBody
                    .gameObject;


                if (objectBelow.GetComponent<RopeSegment>() != null)
                {
                    newSegment = objectBelow;
                }
            }
        }


        // No segment found
        if (newSegment == null)
        {
            return;
        }


        // Old segment no longer has player
        myConnection.isPlayerAttached = false;


        // Get new segment
        RopeSegment newRopeSegment =
            newSegment.GetComponent<RopeSegment>();


        if (newRopeSegment == null)
        {
            return;
        }


        // New segment has player
        newRopeSegment.isPlayerAttached = true;


        // Move player to new segment
        transform.position =
            newSegment.transform.position;


        // Get new Rigidbody2D
        Rigidbody2D newBody =
            newSegment.GetComponent<Rigidbody2D>();


        if (newBody != null)
        {
            hingeJoint.connectedBody =
                newBody;
        }
    }


    // ============================================================
    // COLLISIONS
    // ============================================================

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
            gameManagerObject
                .GetComponent<GameManagerScript>()
                .score += 1;

            Destroy(collision.gameObject);
        }
    }
}