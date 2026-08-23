using UnityEngine;


public class AmbientGlowController : MonoBehaviour
{
    public float fadeSpeed = 2f;

    [Range(0f, 1f)] public float minAlpha = 0.3f;

    
    [Range(0f, 1f)] public float maxAlpha = 1f;

   
    public float hdrIntensity = 2f;

    private SpriteRenderer spriteRenderer;
    private Material glowMaterial;
    private Color baseColor;

    void Start()
    {
        // Get the Sprite Renderer on this object
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure we are editing a unique instance of the material
        glowMaterial = spriteRenderer.material;

        // Save the original color chosen in the inspector
        baseColor = spriteRenderer.color;
    }

    void Update()
    {
        // Calculate a smooth ping-pong value between minAlpha and maxAlpha using a sine wave
        float timeFactor = (Mathf.Sin(Time.time * fadeSpeed) + 1f) / 2f;
        float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, timeFactor);

        // Update the transparency of the sprite
        Color newColor = baseColor;
        newColor.a = currentAlpha;
        spriteRenderer.color = newColor;

        // Multiply the color by HDR intensity to feed into the URP Bloom filter
        // "_Color" is the standard property name for URP 2D Unlit/Lit materials
        if (glowMaterial.HasProperty("_Color"))
        {
            Color emissionColor = baseColor * currentAlpha * hdrIntensity;
            glowMaterial.SetColor("_Color", emissionColor);
        }
    }
}