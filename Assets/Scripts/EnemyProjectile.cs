using System.Collections;
using System.Threading;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    private GameObject player;
    public float duration = 0.3f;
    private Rigidbody2D rb;
    public float force;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
       if (timer > 5)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(RopeCoolDown());
            player.GetComponent<PlayerGreenScript>().Detach();

            Destroy(gameObject,0.5f);
        }
    }
    IEnumerator RopeCoolDown()
    {
        player.GetComponent<PlayerGreenScript>().ropeReattachCooldown = 0.5f;

        yield return new WaitForSeconds(0.3f);

        player.GetComponent<PlayerGreenScript>().ropeReattachCooldown = 0.2f;
    }
}
