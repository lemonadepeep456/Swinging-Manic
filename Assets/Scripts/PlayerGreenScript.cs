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
    public HingeJoint2D hingeJoint2D;

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
        hingeJoint2D = GetComponent<HingeJoint2D>();


        // Make sure the hinge starts disabled
        if (hingeJoint2D != null)
        {
            hingeJoint2D.enabled = false;

            hingeJoint2D.autoConfigureConnectedAnchor = false;

            hingeJoint2D.connectedAnchor = Vector2.zero;
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

        if (hingeJoint2D == null ||
            hingeJoint2D.connectedBody == null)
        {
            return;
        }

        currentHandHolder = leftHandHolder;

        Vector3 ropePosition =
            hingeJoint2D.connectedBody.transform.position;

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

        hingeJoint2D.anchor =
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

        if (hingeJoint2D == null ||
            hingeJoint2D.connectedBody == null)
        {
            return;
        }

        currentHandHolder = rightHandHolder;

        Vector3 ropePosition =
            hingeJoint2D.connectedBody.transform.position;

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

        hingeJoint2D.anchor =
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

        hingeJoint2D.connectedBody = ropeBone;


        // ============================================================
        // USE HAND HOLDER AS PLAYER'S HINGE ANCHOR
        // ============================================================

        Vector3 localHolderPosition =
            transform.InverseTransformPoint(
                currentHandHolder.position
            );

        hingeJoint2D.anchor = localHolderPosition;


        // ============================================================
        // ENABLE HINGE
        // ============================================================

        hingeJoint2D.enabled = true;


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

        hingeJoint2D.anchor = localHolderPosition;


        // Make sure the hinge stays connected
        hingeJoint2D.enabled = true;


        Debug.Log(
            "Switched to " +
            currentHandHolder.name
        );
    }

    // ============================================================
    // DETACH FROM ROPE
    // ============================================================

    public void Detach()
    {
        if (!isAttached)
        {
            return;
        }


        // Get current rope segment
        Rigidbody2D connectedBody =
            hingeJoint2D.connectedBody;


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
        hingeJoint2D.enabled = false;


        // Remove connected body
        hingeJoint2D.connectedBody = null;


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

        if (hingeJoint2D == null ||
            hingeJoint2D.connectedBody == null)
        {
            return;
        }

        // Get the rope segment the player is currently attached to
        RopeSegment currentSegment =
            hingeJoint2D.connectedBody.GetComponent<RopeSegment>();

        if (currentSegment == null)
        {
            return;
        }

        GameObject newSegment = null;


        // ============================================================
        // MOVE UP
        // ============================================================

        if (direction > 0)
        {
            if (currentSegment.connectedAbove != null)
            {
                newSegment = currentSegment.connectedAbove;
            }
        }


        // ============================================================
        // MOVE DOWN
        // ============================================================

        if (direction < 0)
        {
            if (currentSegment.connectedBelow != null)
            {
                newSegment = currentSegment.connectedBelow;
            }
        }


        // ============================================================
        // NO SEGMENT FOUND
        // ============================================================

        if (newSegment == null)
        {
            return;
        }


        // ============================================================
        // GET NEW ROPE SEGMENT
        // ============================================================

        RopeSegment newRopeSegment =
            newSegment.GetComponent<RopeSegment>();

        if (newRopeSegment == null)
        {
            return;
        }


        // ============================================================
        // REMOVE PLAYER FROM OLD SEGMENT
        // ============================================================

        currentSegment.isPlayerAttached = false;


        // ============================================================
        // ATTACH PLAYER TO NEW SEGMENT
        // ============================================================

        newRopeSegment.isPlayerAttached = true;


        // Move player to the new rope segment
        transform.position =
            newSegment.transform.position;


        // Get new Rigidbody2D
        Rigidbody2D newBody =
            newSegment.GetComponent<Rigidbody2D>();


        if (newBody == null)
        {
            return;
        }


        // Connect player to new segment
        hingeJoint2D.connectedBody =
            newBody;
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
            Detach();
            gameManagerObject.GetComponent<GameManagerScript>().RespawnPlayer();
           // Destroy(collision.gameObject, 0.2f);
        }
        if (collision.gameObject.tag == "CheckPoint")
        {
            respawnPosition = collision.transform;
            respawnPosition = gameManagerScript.respawnPosition.transform;

        }
    }
}