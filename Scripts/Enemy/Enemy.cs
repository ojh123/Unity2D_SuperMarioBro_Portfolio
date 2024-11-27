using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 0.3f;
    public float range = 4.0f; // �����̴� ���� �Ÿ�
    public float dis; // �÷��̾���� �Ÿ�
    public bool isDie;
    bool moveOn;
    protected Rigidbody2D rb;
    protected BoxCollider2D col;
    protected BoxCollider2D floorCol;
    public Animator anim;
    public GameObject mario;

     void Awake()
    {
        floorCol = transform.Find("FloorCol").GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update()
    {
        // �÷��̾���� �Ÿ� ���
        dis = Vector2.Distance(transform.position, mario.transform.position);
        //dis = Mathf.Abs(dis);

        if (dis < range) moveOn = true;

        if (!isDie && moveOn)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10)  // ��ֹ� ���̾�� �浹�� �ݴ�� �̵�
        {
            speed *= -1;  
        }

        if(collision.gameObject.CompareTag("Fire") || collision.gameObject.CompareTag("Star"))
        {
            if (!isDie)
            {
                EnemyDie2();
            }
        }

        if(collision.gameObject.CompareTag("DeadZone"))
        {
            Destroy(this.gameObject);
        }

       

    }

    
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyKill"))
        {
            if (isDie) return;
            gameObject.layer = LayerMask.NameToLayer("EnemyDie");
            col.enabled = false;
            isDie = true;
            anim.SetBool("Die", true);
            Destroy(this.gameObject, 0.2f);
        }

        if (collision.CompareTag("DeadKoopa") || collision.CompareTag("Star"))
        {
            if (!isDie)
            {
                EnemyDie2();
            }
        }
    }


    protected void EnemyDie2() // Ƣ������鼭 ���������鼭 ����
    {
        SoundManager.instance.PlaySfx("kick");
        gameObject.layer = LayerMask.NameToLayer("EnemyDie");
        isDie = true;
        col.enabled = false;
        floorCol.enabled = false;
        rb.velocity = new Vector2(0, 0);
        gameObject.transform.localScale = new Vector3(1, -1, 1);
        rb.AddForce(Vector2.up * 120f);
        rb.AddForce(Vector2.right * 10f);
        Destroy(this.gameObject, 1f);
    }

}
