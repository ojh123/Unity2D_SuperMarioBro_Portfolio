using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUp : MonoBehaviour
{
    public float moveDuration = 1.1f;
    private float elapsedTime = 0.0f;
    private Rigidbody2D rb; 
    public bool moveOn = false;
    protected float speed = 0.5f;
    UIManager ui;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
        ui = FindAnyObjectByType<UIManager>();
    }

    protected virtual void Start()
    {
        SoundManager.instance.PlaySfx("powerup_appears");
    }


    protected virtual void Update()  // 박스에서 아이템이 올라오는 효과
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < moveDuration)
        {
            transform.position += Vector3.up * Time.deltaTime * 0.15f;
        }

        if (elapsedTime > moveDuration)  
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            moveOn = true;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)   // 플레이어와 충돌시 삭제
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Star"))
        {
            ui.score += 1000;
            Destroy(this.gameObject);
        }
    }
   
}
