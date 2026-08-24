using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    // ============================================================
    // YOUR EXISTING ROPE CONNECTIONS
    // ============================================================

    public GameObject connectedAbove;
    public GameObject connectedBelow;


    // ============================================================
    // PLAYER ROPE CONNECTION
    // ============================================================

    public bool isPlayerAttached = false;

    public HingeJoint2D hingeJoint2D;

    public PlayerGreenScript player;


    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        // Get this rope segment's hinge joint
        hingeJoint2D = GetComponent<HingeJoint2D>();

        // Find the player
        player = FindFirstObjectByType<PlayerGreenScript>();

        ResetAnchor();
    }


    // ============================================================
    // RESET ROPE ANCHOR
    // ============================================================

    public void ResetAnchor()
    {
        // Get hinge if it hasn't been assigned yet
        if (hingeJoint2D == null)
        {
            hingeJoint2D = GetComponent<HingeJoint2D>();
        }

        // Make sure we actually have a hinge
        if (hingeJoint2D == null)
        {
            Debug.LogWarning(
                gameObject.name + " does not have a HingeJoint2D!"
            );

            return;
        }


        // Make sure the hinge has a connected body
        if (hingeJoint2D.connectedBody == null)
        {
            connectedAbove = null;

            hingeJoint2D.connectedAnchor = new Vector2(0, 0);

            return;
        }


        // ========================================================
        // GET OBJECT ABOVE
        // ========================================================

        connectedAbove =
            hingeJoint2D.connectedBody.gameObject;


        // Check if the object above is another rope segment
        RopeSegment aboveSegment =
            connectedAbove.GetComponent<RopeSegment>();


        if (aboveSegment != null)
        {
            // Tell the segment above that THIS segment
            // is connected below it
            aboveSegment.connectedBelow = gameObject;


            // Get the height of the rope segment above
            SpriteRenderer aboveSprite =
                connectedAbove.GetComponent<SpriteRenderer>();


            if (aboveSprite != null)
            {
                float spriteBottom =
                    aboveSprite.bounds.size.y;


                hingeJoint2D.connectedAnchor =
                    new Vector2(0, spriteBottom * -1);
            }
            else
            {
                hingeJoint2D.connectedAnchor =
                    new Vector2(0, 0);
            }
        }
        else
        {
            // This is the top of the rope / connected to a hook
            hingeJoint2D.connectedAnchor =
                new Vector2(0, 0);
        }
    }


    // ============================================================
    // REMOVE ROPE SEGMENT
    // ============================================================

    public void RemoveLink()
    {
        // ========================================================
        // PLAYER IS CURRENTLY ON THIS SEGMENT
        // ========================================================

        if (isPlayerAttached && player != null)
        {
            // Move the player down one segment before
            // destroying this segment
            player.Slide(-1);

            // Make sure this segment no longer thinks
            // the player is attached
            isPlayerAttached = false;
        }


        // ========================================================
        // UPDATE THE ROPE CONNECTIONS
        // ========================================================

        // Tell the segment below that this segment is being removed
        if (connectedBelow != null)
        {
            RopeSegment belowSegment =
                connectedBelow.GetComponent<RopeSegment>();


            if (belowSegment != null)
            {
                belowSegment.connectedAbove = connectedAbove;


                // Reconnect the segment below to the segment above
                if (belowSegment.hingeJoint2D == null)
                {
                    belowSegment.hingeJoint2D =
                        belowSegment.GetComponent<HingeJoint2D>();
                }


                if (belowSegment.hingeJoint2D != null)
                {
                    Rigidbody2D aboveBody = null;


                    if (connectedAbove != null)
                    {
                        aboveBody =
                            connectedAbove.GetComponent<Rigidbody2D>();
                    }


                    belowSegment.hingeJoint2D.connectedBody =
                        aboveBody;


                    belowSegment.ResetAnchor();
                }
            }
        }


        // Tell the segment above that this segment
        // is no longer connected below it
        if (connectedAbove != null)
        {
            RopeSegment aboveSegment =
                connectedAbove.GetComponent<RopeSegment>();


            if (aboveSegment != null)
            {
                aboveSegment.connectedBelow =
                    connectedBelow;
            }
        }


        // Finally destroy this rope segment
        Destroy(gameObject);
    }
}