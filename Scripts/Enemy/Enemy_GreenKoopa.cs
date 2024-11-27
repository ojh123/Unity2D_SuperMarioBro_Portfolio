using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_GreenKoopa : Enemy
{
    bool isShoot;
    bool isRecovery;  // ȸ�� ����
    float dieTime;    // ���� �ð�
    float recoverTime; // ȸ�� �ð�
    float dieDelay;    // �װ��� �ٷ� Ʈ���� �۵����ϰ� ������
    public float shootSpeed = 10;  // ���ư��� ���ǵ�
    private float shootDir; // ���ư��� ����

    void Awake()
    {
        floorCol = transform.Find("FloorCol").GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();

        if (isRecovery)   // ȸ������
        {
            dieDelay += Time.deltaTime;
            dieTime -= Time.deltaTime;
        }

        if (dieTime < 0)    // ȸ������2 (�ٸ�����)
        {
            anim.SetBool("Die", false);
            anim.SetBool("Recovery", true);
            isRecovery = false;
            recoverTime -= Time.deltaTime;
        }

        if (recoverTime < 0)  // ȸ���Ϸ�
        {
            anim.SetTrigger("Recovery2");
            anim.SetBool("Recovery", false);
            isDie = false;
            dieDelay = 0f;
            gameObject.tag = "Enemy";
            col.isTrigger = false; // ȸ�� �� Ʈ���� ����
        }


        if (isDie && isShoot)  // ���󰡴� ����
        {
            // �÷��̾�� ���� �����ʿ� ������ ��������, ���ʿ� ������ ���������� ����
            transform.Translate(Vector3.right * shootSpeed * shootDir * Time.deltaTime);
        }

    }


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        

        if (collision.gameObject.layer == 10)  // ��ֹ� ���̾�� �浹�� �ݴ�� �̵�
        {
            speed *= -1;
            shootSpeed *= -1;
        }

        if (collision.gameObject.CompareTag("Fire") || collision.gameObject.CompareTag("Star"))
        {
            if (!isDie)
            {
                EnemyDie2();
            }
        }

        if (collision.gameObject.CompareTag("DeadZone"))
        {
            Destroy(this.gameObject);
        }

    }

    // Ʈ���� �浹 ó�� (���� ó��)
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyKill"))
        {
            if (isDie) return;

            isDie = true;
            isRecovery = true;

            col.isTrigger = true;  // ȸ�� �߿��� �ݶ��̴��� Ʈ���ŷ� ����

            anim.SetBool("Die", true);
            dieTime = 3f;  // ���� �ð� ����
            recoverTime = 3f;  // ȸ�� �ð� ����
        }

        if (collision.CompareTag("Star"))
        {
            if (!isDie)
            {
                EnemyDie2();
            }
        }


        if (isDie && dieDelay > 0.1f) // �߻�
        {
            // �±׿� ���̾� ���� ���̾�� Enemy���� �浹ó�� �ȵǰ� �س��� �ٲ�
            gameObject.tag = "DeadKoopa";
            gameObject.layer = LayerMask.NameToLayer("DeadKoopa");

            if (collision.CompareTag("Player") && !isShoot)
            {
                SoundManager.instance.PlaySfx("kick");
                dieTime = 0f;
                recoverTime = 0f;
                isRecovery = false;
                isShoot = true;
                anim.SetTrigger("Shoot");
                anim.SetBool("Recovery", false);
                shootDir = transform.position.x > mario.transform.position.x ? 1f : -1f;
            }

            if (collision.CompareTag("EnemyKill") && !isShoot)
            {
                SoundManager.instance.PlaySfx("kick");
                dieTime = 0f;
                recoverTime = 0f;
                isRecovery = false;
                isShoot = true;
                anim.SetTrigger("Shoot");
                anim.SetBool("Recovery", false);
                shootDir = transform.position.x > mario.transform.position.x ? 1f : -1f;
            }
        }

    }
}



