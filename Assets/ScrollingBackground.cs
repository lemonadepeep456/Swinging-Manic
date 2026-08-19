using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    [SerializeField]
    private Renderer bgRenderer;
    void Update()
    {
        bgRenderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
    }
}
