using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerGreenScript : MonoBehaviour

{
    public Vector3 leftMoveForce;
    public Vector3 rightMoveForce;
    public Vector3 jumpForce;

    public GameObject rightProjectilePrefab;
    public Vector3 rightProjectileOffset;

    public int playerFacing;

    public bool canJump;
    public bool doubleJump;
    public bool isWalking;
    public bool isJumping;
    public bool isOnGround;
    public bool LeapCooldown;
   public Transform respawnPosition;
   public Transform respawnFlagTransform;
    
    public float timer;

    public GameObject gameManagerObject;
    public Animator playerAnimator;
    public SpriteRenderer spriteRenderer;
    public GameManagerScript gameManagerScript;

    // ============================================================
    // ROPE VARIABLES
    // ============================================================

    public Rigidbody2D rb;
    public new HingeJoint2D hingeJoint;

    public float ropeSwingForce = 10f;

    public bool isAttached = false;

    public Transform attachedTo;

    // Player hand holders
    public Transform leftHandHolder;
    public Transform rightHandHolder;

    // The holder currently being used
    private Transform currentHandHolder;




    // ============================================================
    // ROPE REATTACH COOLDOWN
    // ============================================================


    public float ropeReattachCooldown = 0.3f;
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

        gameManagerScript = gameManagerObject.GetComponent<GameManagerScript>();
      //  respawnPosition = gameManagerScript.respawnPosition;
       // respawnFlagTransform = gameManagerScript.respawnFlagTransform;

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

            // Allow shooting while on the rope
            if (Input.GetMouseButtonDown(0))
            {
                ShootProjectile();
            }

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
            // FALL CHECK
            // ====================================================
            if (GetComponent<Transform>().position.y <= -20f)
            {
                gameManagerObject.GetComponent<GameManagerScript>().playerLost = true;
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
        // ====================================================
        // PROJECTILE
        // ====================================================

        if (Input.GetMouseButtonDown(0))

        {
            ShootProjectile();

        }
        if (Input.GetMouseButtonDown(0) && isAttached == true && (Input.GetKey(KeyCode.D) || (Input.GetMouseButtonDown(0) && isAttached == true && (Input.GetKey(KeyCode.A)))))
        {
            ShootProjectile();
            playerAnimator.Play("PlayerHoldingGun");
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
        // DOUBLE JUMP INPUT
        // ========================================================

        if (Input.GetKeyDown(KeyCode.W) &&
            doubleJump == true)
        {
            StartCoroutine(DoubleJumping());
        }



    }
    private void ShootProjectile()
    {
        if (isAttached == true)
        {
            playerAnimator.Play("PlayerHoldingGun");
        }

        // Get mouse position in the world
        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        mousePosition.z = transform.position.z;


        // Calculate direction from player to mouse
        Vector2 direction =
            mousePosition - transform.position;


        // Make sure we actually have a direction
        if (direction == Vector2.zero)
        {
            return;
        }


        // Create projectile
        GameObject projectile =
            Instantiate(
                rightProjectilePrefab,
                transform.position,
                Quaternion.identity
            );


        // Get projectile script
        ProjectileScript projectileScript =
            projectile.GetComponent<ProjectileScript>();


        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
            //  playerAnimator.Play("PlayerHoldingGun");

        }

    }

    // ============================================================
    // ROPE CONTROLS
    // ============================================================

    private void CheckRopeControls()
    {
        // ============================================================
        // SWING LEFT
        // ============================================================
        if (Input.GetKey(KeyCode.A) && (Input.GetMouseButtonDown(0)) ||
           Input.GetKey(KeyCode.LeftArrow) && (Input.GetMouseButtonDown(0)))
        {
            playerFacing = -1;
            playerAnimator.Play("PlayerHoldingGun");
            spriteRenderer.flipX = true;

            rb.AddRelativeForce(
                Vector2.left * ropeSwingForce
            );

            SwitchToLeftSide();
        }


        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow))
        {
            playerFacing = -1;
            playerAnimator.Play("PlayerRope");
            spriteRenderer.flipX = true;

            rb.AddRelativeForce(
                Vector2.left * ropeSwingForce
            );

            SwitchToLeftSide();
        }


        // ============================================================
        // SWING RIGHT
        // ============================================================
        if (Input.GetKey(KeyCode.D) && (Input.GetMouseButtonDown(0)) ||
        Input.GetKey(KeyCode.RightArrow) && (Input.GetMouseButtonDown(0)))
        {
            playerFacing = 1;
            playerAnimator.Play("PlayerHoldingGun");
            spriteRenderer.flipX = false;
            rb.AddRelativeForce(
                Vector2.right * ropeSwingForce
            );

            SwitchToRightSide();
        }


        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            playerFacing = 1;
            playerAnimator.Play("PlayerRope");
            spriteRenderer.flipX = false;
            rb.AddRelativeForce(
                Vector2.right * ropeSwingForce
            );

            SwitchToRightSide();
        }



        // ============================================================
        // CLIMB UP
        // ============================================================

        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.UpArrow))
        {
            Slide(1);
        }


        // ============================================================
        // CLIMB DOWN
        // ============================================================

        if (Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.DownArrow))
        {
            Slide(-1);
        }


        // ============================================================
        // DETACH
        // ============================================================

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Detach();
            playerAnimator.Play("PlayerLeap");
        }
    }
    private void SwitchToLeftSide()
    {
        if (!isAttached)
        {
            return;
        }

        if (leftHandHolder == null)
        {
            return;
        }

        if (hingeJoint == null ||
            hingeJoint.connectedBody == null)
        {
            return;
        }

        currentHandHolder = leftHandHolder;

        Vector3 ropePosition =
            hingeJoint.connectedBody.transform.position;

        float distanceFromRope =
            Mathf.Abs(
                transform.position.x - ropePosition.x
            );

        distanceFromRope =
            Mathf.Max(distanceFromRope, 0.25f);

        Vector3 newPosition =
            transform.position;

        newPosition.x =
            ropePosition.x - distanceFromRope;

        transform.position =
            newPosition;

        Vector3 localHolderPosition =
            transform.InverseTransformPoint(
                currentHandHolder.position
            );

        hingeJoint.anchor =
            localHolderPosition;
    }


    private void SwitchToRightSide()
    {
        if (!isAttached)
        {
            return;
        }

        if (rightHandHolder == null)
        {
            return;
        }

        if (hingeJoint == null ||
            hingeJoint.connectedBody == null)
        {
            return;
        }

        currentHandHolder = rightHandHolder;

        Vector3 ropePosition =
            hingeJoint.connectedBody.transform.position;

        float distanceFromRope =
            Mathf.Abs(
                transform.position.x - ropePosition.x
            );

        distanceFromRope =
            Mathf.Max(distanceFromRope, 0.25f);

        Vector3 newPosition =
            transform.position;

        newPosition.x =
            ropePosition.x + distanceFromRope;

        transform.position =
            newPosition;

        Vector3 localHolderPosition =
            transform.InverseTransformPoint(
                currentHandHolder.position
            );

        hingeJoint.anchor =
            localHolderPosition;
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


        // ============================================================
        // CHOOSE WHICH HAND HOLDS THE ROPE
        // ============================================================

        if (playerFacing == 1)
        {
            // Player facing RIGHT
            currentHandHolder = rightHandHolder;
            playerAnimator.Play("PlayerRope");
            spriteRenderer.flipX = false;
        }
        else
        {
            // Player facing LEFT
            currentHandHolder = leftHandHolder;
            playerAnimator.Play("PlayerRope");
            spriteRenderer.flipX = true;
        }


        // Make sure the correct holder exists
        if (currentHandHolder == null)
        {
            Debug.LogWarning(
                "Player hand holder has not been assigned!"
            );

            return;
        }


        // ============================================================
        // TELL ROPE SEGMENT PLAYER IS ATTACHED
        // ============================================================

        ropeSegment.isPlayerAttached = true;


        // ============================================================
        // CONNECT PLAYER TO ROPE
        // ============================================================

        hingeJoint.connectedBody = ropeBone;


        // ============================================================
        // USE HAND HOLDER AS PLAYER'S HINGE ANCHOR
        // ============================================================

        Vector3 localHolderPosition =
            transform.InverseTransformPoint(
                currentHandHolder.position
            );

        hingeJoint.anchor = localHolderPosition;


        // ============================================================
        // ENABLE HINGE
        // ============================================================

        hingeJoint.enabled = true;


        // Player is attached
        isAttached = true;


        // Store rope reference
        attachedTo = ropeBone.transform.parent;


        Debug.Log(
            "Player attached to rope using " +
            currentHandHolder.name
        );
    }
    private void SwitchRopeSide()
    {
        if (!isAttached)
        {
            return;
        }


        // Make sure both holders exist
        if (leftHandHolder == null ||
            rightHandHolder == null)
        {
            Debug.LogWarning(
                "Left and Right Hand Holders must be assigned!"
            );

            return;
        }


        // ============================================================
        // SWITCH FROM RIGHT HAND TO LEFT HAND
        // ============================================================

        if (currentHandHolder == rightHandHolder)
        {
            currentHandHolder = leftHandHolder;
        }


        // ============================================================
        // SWITCH FROM LEFT HAND TO RIGHT HAND
        // ============================================================

        else
        {
            currentHandHolder = rightHandHolder;
        }


        // ============================================================
        // UPDATE THE HINGE ANCHOR
        // ============================================================

        Vector3 localHolderPosition =
            transform.InverseTransformPoint(
                currentHandHolder.position
            );

        hingeJoint.anchor = localHolderPosition;


        // Make sure the hinge stays connected
        hingeJoint.enabled = true;


        Debug.Log(
            "Switched to " +
            currentHandHolder.name
        );
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
        if (collision.gameObject.tag == "Enemy")
        {
            Detach();
            gameManagerObject.GetComponent<GameManagerScript>().RespawnPlayer();
        }
        if (collision.gameObject.tag == "Tripmine")
        {
            //Detach();
            gameManagerObject.GetComponent<GameManagerScript>().RespawnPlayer();
           // Destroy(collision.gameObject, 0.2f);
        }
        if (collision.gameObject.tag == "CheckPoint")
        {
            respawnPosition = collision.transform;
            respawnPosition = gameManagerScript.respawner.transform;

        }
    }
}