using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    Animator anim;
    SpriteRenderer sprite;
    SpriteRenderer sprite2;
    SpriteRenderer sprite3;
    BoxCollider2D playerCol;
    BoxCollider2D itemCol;
    BoxCollider2D enemyKillCol;

    public Transform player;   // 일반 마리오
    public Transform player2;  // 슈퍼 마리오
    public Transform player3;  // 플라워 마리오
    public Transform firePos; // 불 발사 포지션
    public GameObject fire;  // 불 프리팹

    Vector2 originColSize;
    Vector2 originColOffSet;
    Vector2 originItemColOffSet;


    public float moveForce = 3f;  // 이동속도
    public float maxSpeed = 1f;   // 최대 이동속도
    public float jumpForce = 30f; // 점프력
    public float shotJumpForce = 100f; // 짧게 눌렀을때 점프력
    public float maxJumpTime = 0.2f; // 최대 점프 시간
    float jumpStartTime;  // 점프 시작 시간

    public float groundCheckDistance = 0.2f; // 바닥 체크 거리
    public LayerMask groundLayer; // 바닥에 해당하는 레이어


    float h;
    bool end;
    bool enemyKillJump;  // 트리거 중복 방지
    bool isJumping = false;    // 점프 중인지 확인

    public bool isGround = true;   // 바닥 체크
    bool dirRight = true;           // 플립 체크
    bool isInvincibility = false;    // 피격시 무적 체크

    public bool sizeUp = false;     // 사이즈업 체크
    public bool starState = false; // 스타 상태 체크
    public bool flowerState = false; // 플라워 상태 체크
    bool canFire = true;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        anim = player.GetComponent<Animator>();

        sprite = player.GetComponent<SpriteRenderer>();
        sprite2 = player2.GetComponent<SpriteRenderer>();
        sprite3 = player3.GetComponent<SpriteRenderer>();

        playerCol = GetComponent<BoxCollider2D>();
        itemCol = GameObject.Find("ItemCollider").GetComponent<BoxCollider2D>();
        enemyKillCol = GameObject.Find("EnemyKill").GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        originColSize = playerCol.size;
        originColOffSet = playerCol.offset;
        originItemColOffSet = itemCol.offset;
    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");

        // 점프
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            anim.SetBool("Jump", true);
            if (!sizeUp)
            {
                SoundManager.instance.PlaySfx("jump-small");
            }
            else if (sizeUp)
            {
                SoundManager.instance.PlaySfx("jump-super");
            }

            isJumping = true;
            isGround = false;
            jumpStartTime = Time.time; // 점프 시작 시간 기록
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0); // 기존 Y속도 초기화
            rigidbody2D.AddForce(Vector2.up * shotJumpForce);  // 초기 점프            
        }

        // 플라워 상태일때 불 발사
        if (Input.GetKeyDown(KeyCode.Z) && flowerState && canFire)
        {
            canFire = false;
            anim.SetTrigger("Fire");
            SoundManager.instance.PlaySfx("fireball");
            StartCoroutine(FireDelay());
        }



        // 이동 방향 전환 (좌우 플립)
        if (h > 0 && !dirRight && isGround)
        {
            Flip();
        }
        else if (h < 0 && dirRight && isGround)
        {
            Flip();
        }

    }

    void FixedUpdate()
    {
        // 이동
        anim.SetFloat("Speed", Mathf.Abs(h));
        if (Mathf.Abs(rigidbody2D.velocity.x) < maxSpeed)
        {
            rigidbody2D.AddForce(Vector2.right * moveForce * h, ForceMode2D.Force);
        }

        if (Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed) // 최대 스피드가 넘으면 스피드 고정
        {
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
        }

        if (Input.GetButton("Jump") && isJumping)  // 점프 버튼을 누르고 있는 동안
        {
            float jumpDuration = Time.time - jumpStartTime;  // 버튼을 누른 시간을 계산
            if (jumpDuration < maxJumpTime)  // 최대 점프 시간 이내일 경우 점프 강화
            {
                rigidbody2D.AddForce(Vector2.up * jumpForce * Time.deltaTime, ForceMode2D.Impulse);  // 점프력 증가
            }
        }

        // 점프 종료
        if (isJumping && !Input.GetButton("Jump") || Time.time - jumpStartTime >= maxJumpTime)  // 점프 버튼을 떼면 점프 종료
        {
            isJumping = false;

        }


        if (end)
        {
            Vector3 theScale = player.localScale;
            theScale.x = 1;
            player.localScale = theScale;
            player2.localScale = theScale;
            rigidbody2D.velocity = new Vector2(0.5f, rigidbody2D.velocity.y);
            anim.SetTrigger("End2");
        }



    }

    #region Mario Func
    public void BasicMario() // 일반 마리오
    {
        SoundManager.instance.PlaySfx("powerdown");
        sizeUp = false;
        anim = player.GetComponent<Animator>();
        player.gameObject.SetActive(true);
        player2.gameObject.SetActive(false);
        player3.gameObject.SetActive(false);
        SizeEffect();
        UpdateColliderSize();
        StartCoroutine(Invincibility());
    }

    public void SuperMario()  // 사이즈업 상태 마리오
    {
        SoundManager.instance.PlaySfx("powerup");
        sizeUp = true;
        anim = player2.GetComponent<Animator>();
        player.gameObject.SetActive(false);
        player2.gameObject.SetActive(true);
        player.gameObject.SetActive(false);

        SizeEffect();
        UpdateColliderSize();
    }

    public void FlowerMario()  // 플라워 상태 마리오
    {
        SoundManager.instance.PlaySfx("powerup");
        if (!sizeUp) return;  // 사이즈업 상태 아닐경우 리턴
        FlowerEffect();
        flowerState = true;
        anim = player3.GetComponent<Animator>();
        player2.gameObject.SetActive(false);
        player3.gameObject.SetActive(true);
    }

    IEnumerator FireDelay()
    {
       GameObject _fire = Instantiate(fire, firePos.position, Quaternion.identity);
        _fire.GetComponent<Fire>().dir = dirRight ? 1 : -1;  // 나가는 방향 설정
        yield return new WaitForSeconds(0.3f);
        canFire = true;
    }

    void Flip() // 방향전환
    {
        dirRight = !dirRight;
        anim.SetTrigger("Turn");
        anim.SetFloat("TurnSpeed", Mathf.Abs(rigidbody2D.velocity.x));

        Vector3 theScale = player.localScale;
        theScale.x *= -1;
        player.localScale = theScale;
        player2.localScale = theScale;
        player3.localScale = theScale;
    }

    private bool IsGrounded()  // 레이캐스트로 바닥 체크
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    void PlayerDie() // 플레이어 죽음
    {
        anim.SetTrigger("Dead");
        // 조작 안되도록 움직임값들 0으로 초기화
        moveForce = 0;
        jumpForce = 0f;
        shotJumpForce = 0f;
        rigidbody2D.velocity = new Vector2(0, 0); // 속도 초기화
        rigidbody2D.AddForce(Vector2.up * 250f);
        playerCol.enabled = false;
        itemCol.enabled = false;
        enemyKillCol.enabled = false;

        StartCoroutine(LoadScene());
    }

    void UpdateColliderSize() // 콜라이더 업데이트 일반마리오 상태 또는 슈퍼 마리오 상태
    {
        if (sizeUp)
        {
            playerCol.size = new Vector2(playerCol.size.x, 0.3f);
            playerCol.offset = new Vector2(playerCol.offset.x, 0.085f);
            itemCol.offset = new Vector2(itemCol.offset.x, 0.22f);
        }
        else
        {
            // 원래 크기로 복구
            playerCol.size = originColSize;
            playerCol.offset = originColOffSet;
            itemCol.offset = originItemColOffSet;
        }
    }

    public void SizeEffect()  //  크기 효과
    {
        StartCoroutine(CorSizeEffect());
    }

    //크기 효과 코루틴
    IEnumerator CorSizeEffect()
    {
        Time.timeScale = 0f;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        for (int i = 0; i < 5; i++)
        {
            player.gameObject.SetActive(true);  // 기본 마리오 상태로 전환
            player2.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.08f);
            player2.gameObject.SetActive(true);  // 슈퍼 마리오 상태로 전환
            player.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.08f);
        }

        if (sizeUp) // 사이즈업 상태면 슈퍼마리오로
        {
            anim = player2.GetComponent<Animator>();
            player.gameObject.SetActive(false);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(false);
        }
        else if (!sizeUp)  // 사이즈업 상태가 아니면 기본 마리오로
        {
            anim = player.GetComponent<Animator>();
            player.gameObject.SetActive(true);
            player2.gameObject.SetActive(false);
            player3.gameObject.SetActive(false);
        }

        rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionY;

        Time.timeScale = 1f;

    }

    public void StarEffect()
    {
        StartCoroutine(CorStarEffect());
    }

    IEnumerator CorStarEffect()
    {
        starState = true;
        // 태그 변경
        gameObject.tag = "Star";

        // 목표 색상 (255, 193, 153) -> RGB 값을 0에서 1로 변환하여 사용
        Color changeColor = new Color(224f / 255f, 153f / 255f, 107f / 255f);

        // 색상 변화 시간
        float changeTime = 10f;


        // 기존 색상 저장
        Color originColor = sprite.color;
        Color originColor2 = sprite2.color;
        Color originColor3 = sprite3.color;


        while (0 < changeTime)
        {
            // sprite의 색상 변경
            sprite.color = changeColor;
            sprite2.color = changeColor;
            sprite3.color = changeColor;

            yield return new WaitForSeconds(0.1f);

            sprite.color = originColor;
            sprite2.color = originColor2;
            sprite3.color = originColor3;

            yield return new WaitForSeconds(0.1f);

            // 시간 경과 0.1초 만큼 2번 멈추니까
            changeTime -= 0.2f;
        }

        starState = false;

        // 태그 변경
        gameObject.tag = "Player";

        // 색상 원상복구
        sprite.color = originColor;
        sprite2.color = originColor2;
        sprite3.color = originColor3;
    }

    public void FlowerEffect()   // 꽃 먹을때 이펙트
    {
        StartCoroutine(FlowerEffectCor());
    }

    IEnumerator FlowerEffectCor()
    {
        Time.timeScale = 0f;
        // 목표 색상 (255, 193, 153) -> RGB 값을 0에서 1로 변환하여 사용
        Color changeColor = new Color(204f / 255f, 144f / 255f, 106f / 255f);

        // 색상 변화 시간
        float changeTime = 0.5f;

        // 기존 색상 저장
        Color originColor3 = sprite3.color;

        while (0 < changeTime)
        {
            // sprite의 색상 변경
            sprite3.color = changeColor;
            yield return new WaitForSecondsRealtime(0.1f);
            sprite3.color = originColor3;
            yield return new WaitForSecondsRealtime(0.1f);

            // 시간 경과 0.1초 만큼 2번 멈추니까
            changeTime -= 0.2f;
        }

        // 색상 원상복구
        sprite3.color = originColor3;
        Time.timeScale = 1f;
    }

    IEnumerator Invincibility()  // 무적시간
    {
        isInvincibility = true;
        yield return new WaitForSeconds(1f);
        isInvincibility = false;
    }


    IEnumerator LoadScene()
    {
        // 지정한 시간만큼 대기
        yield return new WaitForSeconds(2f);
        // 씬 로드
        SceneManager.LoadScene(0);
    }

    IEnumerator EndOn()
    {
        yield return new WaitForSeconds(0.6f);
        end = true;
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Ground")) || (collision.gameObject.CompareTag("Enemy")))
        {
            isGround = true;
            anim.SetBool("Jump", false);
        }

        if (collision.gameObject.CompareTag("Enemy") && !sizeUp && !starState) // 적과 충돌시 죽음
        {
            if (!isInvincibility)
            {
                SoundManager.instance.PlayBackground("Lost a Life");
                PlayerDie();
            }
        }

        if (collision.gameObject.CompareTag("Enemy") && sizeUp && !starState) // 사이즈업 상태일 때 적과 충돌시 사이즈업 해제
        {

            BasicMario();
        }


        if (collision.gameObject.CompareTag("Enemy") && flowerState) // 플라워 상태일 때 적과 충돌하면 기본 마리오 상태로
        {
            flowerState = false;  // 플라워 상태 해제
            BasicMario();  // 기본 마리오 상태로 변경
        }



        if (collision.gameObject.CompareTag("DeadKoopa") && !sizeUp && !starState) // 등껍질에 충돌시 죽음
        {
            PlayerDie();
        }


        if (collision.gameObject.CompareTag("DeadKoopa") && sizeUp && !starState) // 등껍질에 충돌시 사이즈업 해제
        {
            sizeUp = false;
            SizeEffect();
        }



    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !starState) // 적을 밟으면 위로 올라감
        {
            if (!enemyKillJump)
            {
                enemyKillJump = true;
                SoundManager.instance.PlaySfx("stomp");
                rigidbody2D.AddForce(Vector2.up * 200f);
            }
        }

        if (collision.CompareTag("End")) // 깃발 도착
        {
            SoundManager.instance.PlayBackground("Level Complete");
            anim.SetTrigger("End");
            rigidbody2D.velocity = Vector2.zero;
            moveForce = 0;
            jumpForce = 0;
            rigidbody2D.gravityScale = 0.3f;


        }

        if (collision.CompareTag("Flag"))  // 깃발에 닿으면 반전
        {
            transform.position = new Vector3(14.7f, transform.position.y, 0);
            Vector3 theScale = player.localScale;
            theScale.x = -1;
            player.localScale = theScale;
            player2.localScale = theScale;
            player3.localScale = theScale;
            StartCoroutine(EndOn());
        }

        if (collision.CompareTag("End2"))   // 성에 도착
        {
            sprite.enabled = false;
            sprite2.enabled = false;
            sprite3.enabled = false;
            end = false;
            rigidbody2D.velocity = Vector2.zero;
            StartCoroutine(LoadScene());

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemyKillJump = false;
        }
    }
}