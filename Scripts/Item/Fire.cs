using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D col;
    public float dir; // 나가는 방향

    public float xSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        
    }

    private void Start()
    {
        rb.AddForce(Vector2.right * xSpeed * dir, ForceMode2D.Force);
        Destroy(this.gameObject, 2f);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("CameraWall"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Side"))
        {
            Destroy(gameObject);
        }
    }
}
