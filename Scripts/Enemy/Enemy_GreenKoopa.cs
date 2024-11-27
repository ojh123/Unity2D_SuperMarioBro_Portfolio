using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_GreenKoopa : Enemy
{
    bool isShoot;
    bool isRecovery;  // 회복 여부
    float dieTime;    // 죽은 시간
    float recoverTime; // 회복 시간
    float dieDelay;    // 죽고난후 바로 트리거 작동안하게 딜레이
    public float shootSpeed = 10;  // 날아가는 스피드
    private float shootDir; // 날아가는 방향

    void Awake()
    {
        floorCol = transform.Find("FloorCol").GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();

        if (isRecovery)   // 회복상태
        {
            dieDelay += Time.deltaTime;
            dieTime -= Time.deltaTime;
        }

        if (dieTime < 0)    // 회복상태2 (다리나옴)
        {
            anim.SetBool("Die", false);
            anim.SetBool("Recovery", true);
            isRecovery = false;
            recoverTime -= Time.deltaTime;
        }

        if (recoverTime < 0)  // 회복완료
        {
            anim.SetTrigger("Recovery2");
            anim.SetBool("Recovery", false);
            isDie = false;
            dieDelay = 0f;
            gameObject.tag = "Enemy";
            col.isTrigger = false; // 회복 후 트리거 해제
        }


        if (isDie && isShoot)  // 날라가는 상태
        {
            // 플레이어보다 적이 오른쪽에 있으면 왼쪽으로, 왼쪽에 있으면 오른쪽으로 슈팅
            transform.Translate(Vector3.right * shootSpeed * shootDir * Time.deltaTime);
        }

    }


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        

        if (collision.gameObject.layer == 10)  // 장애물 레이어와 충돌시 반대로 이동
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

    // 트리거 충돌 처리 (죽음 처리)
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyKill"))
        {
            if (isDie) return;

            isDie = true;
            isRecovery = true;

            col.isTrigger = true;  // 회복 중에는 콜라이더를 트리거로 설정

            anim.SetBool("Die", true);
            dieTime = 3f;  // 죽음 시간 설정
            recoverTime = 3f;  // 회복 시간 설정
        }

        if (collision.CompareTag("Star"))
        {
            if (!isDie)
            {
                EnemyDie2();
            }
        }


        if (isDie && dieDelay > 0.1f) // 발사
        {
            // 태그와 레이어 변경 레이어는 Enemy끼리 충돌처리 안되게 해놔서 바꿈
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



