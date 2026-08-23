using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulleyController : MonoBehaviour
{
    // Start is called before the first frame update

    public Rigidbody2D rb;
    private HingeJoint2D hj;
    public GameObject pulleySelected;
    // Update is called once per frame
    void Awake()
    {
        CheckPulleyInputs();
    }
    void CheckPulleyInputs()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if(hit.collider != null && hit.transform.gameObject.tag == "Crank")
            {
                if(pulleySelected != hit.transform.gameObject)
                {
                    if (pulleySelected != null)
                    {
                        {
                            pulleySelected.GetComponent<Crank>().DeSelect();

                        }
                        pulleySelected = hit.transform.gameObject;
                        pulleySelected.GetComponent<Crank>().Select();
                    }
                    else if (pulleySelected == hit.transform.gameObject)
                    {
                        pulleySelected.GetComponent<Crank>().DeSelect();
                        pulleySelected = null;
                    }
                }
                else
                {
                    if(pulleySelected != null)
                    {
                        pulleySelected.GetComponent<Crank>().DeSelect();
                        pulleySelected = null;
                    }
                }
            }

        }
        if(Input.GetKeyDown("f") && pulleySelected != null)
        {
            pulleySelected.GetComponent<Crank>().Rotate(1);
        }
        if (Input.GetKeyDown("r") && pulleySelected != null)
        {
            pulleySelected.GetComponent<Crank>().Rotate(-1);
        }
    }
}
