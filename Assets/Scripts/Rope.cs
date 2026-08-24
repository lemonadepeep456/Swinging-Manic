using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D hook;
    public GameObject[] prefabRopeSegs;
    public int numlinks = 5;

    public HingeJoint2D top;
    void Start()
    {
        GenerateRope();
    }


void GenerateRope()
    {
        Rigidbody2D prevBod = hook;

        for (int i = 0; i < numlinks; i++)
        {
            int index = Random.Range(0, prefabRopeSegs.Length);

            GameObject newSeg = Instantiate(
                prefabRopeSegs[index],
                transform.position,
                Quaternion.identity
            );

            newSeg.transform.SetParent(transform);

            HingeJoint2D hj = newSeg.GetComponent<HingeJoint2D>();

            hj.connectedBody = prevBod;

            RopeSegment ropeSegment =
                newSeg.GetComponent<RopeSegment>();

            if (ropeSegment != null)
            {
                ropeSegment.hingeJoint2D = hj;
            }

            prevBod = newSeg.GetComponent<Rigidbody2D>();

            if (i == 0)
            {
                top = hj;
            }
        }
    }
    public void AddLink()
    {
        int index = Random.Range(0, prefabRopeSegs.Length);
        GameObject newLink = Instantiate(prefabRopeSegs[index]);
        newLink.transform.parent = transform;
        newLink.transform.position=transform.position;
        HingeJoint2D hj = newLink.GetComponent<HingeJoint2D>();
        hj.connectedBody = hook;
        newLink.GetComponent<RopeSegment>().connectedBelow = top.gameObject;
        top.connectedBody = newLink.GetComponent<Rigidbody2D>();
        top.GetComponent<RopeSegment>().ResetAnchor();
        top = hj;
    }
    public void RemoveLink()
    {
        HingeJoint2D newTop = top.gameObject.GetComponent<RopeSegment>().connectedBelow.GetComponent<HingeJoint2D>();
        newTop.connectedBody = hook;
        newTop.gameObject.transform.position = hook.gameObject.transform.position;
        newTop.GetComponent<RopeSegment>().ResetAnchor();
        Destroy(top.gameObject);
        top = newTop;
    }
} 

