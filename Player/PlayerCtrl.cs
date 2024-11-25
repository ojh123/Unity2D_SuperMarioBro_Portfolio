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

    public Transform player;   // �Ϲ� ������
    public Transform player2;  // ���� ������
    public Transform player3;  // �ö�� ������
    public Transform firePos; // �� �߻� ������
    public GameObject fire;  // �� ������

    Vector2 originColSize;
    Vector2 originColOffSet;
    Vector2 originItemColOffSet;


    public float moveForce = 3f;  // �̵��ӵ�
    public float maxSpeed = 1f;   // �ִ� �̵��ӵ�
    public float jumpForce = 30f; // ������
    public float shotJumpForce = 100f; // ª�� �������� ������
    public float maxJumpTime = 0.2f; // �ִ� ���� �ð�
    float jumpStartTime;  // ���� ���� �ð�

    public float groundCheckDistance = 0.2f; // �ٴ� üũ �Ÿ�
    public LayerMask groundLayer; // �ٴڿ� �ش��ϴ� ���̾�


    float h;
    bool end;
    bool enemyKillJump;  // Ʈ���� �ߺ� ����
    bool isJumping = false;    // ���� ������ Ȯ��

    public bool isGround = true;   // �ٴ� üũ
    bool dirRight = true;           // �ø� üũ
    bool isInvincibility = false;    // �ǰݽ� ���� üũ

    public bool sizeUp = false;     // ������� üũ
    public bool starState = false; // ��Ÿ ���� üũ
    public bool flowerState = false; // �ö�� ���� üũ
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

        // ����
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
            jumpStartTime = Time.time; // ���� ���� �ð� ���
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0); // ���� Y�ӵ� �ʱ�ȭ
            rigidbody2D.AddForce(Vector2.up * shotJumpForce);  // �ʱ� ����            
        }

        // �ö�� �����϶� �� �߻�
        if (Input.GetKeyDown(KeyCode.Z) && flowerState && canFire)
        {
            canFire = false;
            anim.SetTrigger("Fire");
            SoundManager.instance.PlaySfx("fireball");
            StartCoroutine(FireDelay());
        }



        // �̵� ���� ��ȯ (�¿� �ø�)
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
        // �̵�
        anim.SetFloat("Speed", Mathf.Abs(h));
        if (Mathf.Abs(rigidbody2D.velocity.x) < maxSpeed)
        {
            rigidbody2D.AddForce(Vector2.right * moveForce * h, ForceMode2D.Force);
        }

        if (Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed) // �ִ� ���ǵ尡 ������ ���ǵ� ����
        {
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
        }

        if (Input.GetButton("Jump") && isJumping)  // ���� ��ư�� ������ �ִ� ����
        {
            float jumpDuration = Time.time - jumpStartTime;  // ��ư�� ���� �ð��� ���
            if (jumpDuration < maxJumpTime)  // �ִ� ���� �ð� �̳��� ��� ���� ��ȭ
            {
                rigidbody2D.AddForce(Vector2.up * jumpForce * Time.deltaTime, ForceMode2D.Impulse);  // ������ ����
            }
        }

        // ���� ����
        if (isJumping && !Input.GetButton("Jump") || Time.time - jumpStartTime >= maxJumpTime)  // ���� ��ư�� ���� ���� ����
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
    public void BasicMario() // �Ϲ� ������
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

    public void SuperMario()  // ������� ���� ������
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

    public void FlowerMario()  // �ö�� ���� ������
    {
        SoundManager.instance.PlaySfx("powerup");
        if (!sizeUp) return;  // ������� ���� �ƴҰ�� ����
        FlowerEffect();
        flowerState = true;
        anim = player3.GetComponent<Animator>();
        player2.gameObject.SetActive(false);
        player3.gameObject.SetActive(true);
    }

    IEnumerator FireDelay()
    {
       GameObject _fire = Instantiate(fire, firePos.position, Quaternion.identity);
        _fire.GetComponent<Fire>().dir = dirRight ? 1 : -1;  // ������ ���� ����
        yield return new WaitForSeconds(0.3f);
        canFire = true;
    }

    void Flip() // ������ȯ
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

    private bool IsGrounded()  // ����ĳ��Ʈ�� �ٴ� üũ
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    void PlayerDie() // �÷��̾� ����
    {
        anim.SetTrigger("Dead");
        // ���� �ȵǵ��� �����Ӱ��� 0���� �ʱ�ȭ
        moveForce = 0;
        jumpForce = 0f;
        shotJumpForce = 0f;
        rigidbody2D.velocity = new Vector2(0, 0); // �ӵ� �ʱ�ȭ
        rigidbody2D.AddForce(Vector2.up * 250f);
        playerCol.enabled = false;
        itemCol.enabled = false;
        enemyKillCol.enabled = false;

        StartCoroutine(LoadScene());
    }

    void UpdateColliderSize() // �ݶ��̴� ������Ʈ �Ϲݸ����� ���� �Ǵ� ���� ������ ����
    {
        if (sizeUp)
        {
            playerCol.size = new Vector2(playerCol.size.x, 0.3f);
            playerCol.offset = new Vector2(playerCol.offset.x, 0.085f);
            itemCol.offset = new Vector2(itemCol.offset.x, 0.22f);
        }
        else
        {
            // ���� ũ��� ����
            playerCol.size = originColSize;
            playerCol.offset = originColOffSet;
            itemCol.offset = originItemColOffSet;
        }
    }

    public void SizeEffect()  //  ũ�� ȿ��
    {
        StartCoroutine(CorSizeEffect());
    }

    //ũ�� ȿ�� �ڷ�ƾ
    IEnumerator CorSizeEffect()
    {
        Time.timeScale = 0f;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        for (int i = 0; i < 5; i++)
        {
            player.gameObject.SetActive(true);  // �⺻ ������ ���·� ��ȯ
            player2.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.08f);
            player2.gameObject.SetActive(true);  // ���� ������ ���·� ��ȯ
            player.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.08f);
        }

        if (sizeUp) // ������� ���¸� ���۸�������
        {
            anim = player2.GetComponent<Animator>();
            player.gameObject.SetActive(false);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(false);
        }
        else if (!sizeUp)  // ������� ���°� �ƴϸ� �⺻ ��������
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
        // �±� ����
        gameObject.tag = "Star";

        // ��ǥ ���� (255, 193, 153) -> RGB ���� 0���� 1�� ��ȯ�Ͽ� ���
        Color changeColor = new Color(224f / 255f, 153f / 255f, 107f / 255f);

        // ���� ��ȭ �ð�
        float changeTime = 10f;


        // ���� ���� ����
        Color originColor = sprite.color;
        Color originColor2 = sprite2.color;
        Color originColor3 = sprite3.color;


        while (0 < changeTime)
        {
            // sprite�� ���� ����
            sprite.color = changeColor;
            sprite2.color = changeColor;
            sprite3.color = changeColor;

            yield return new WaitForSeconds(0.1f);

            sprite.color = originColor;
            sprite2.color = originColor2;
            sprite3.color = originColor3;

            yield return new WaitForSeconds(0.1f);

            // �ð� ��� 0.1�� ��ŭ 2�� ���ߴϱ�
            changeTime -= 0.2f;
        }

        starState = false;

        // �±� ����
        gameObject.tag = "Player";

        // ���� ���󺹱�
        sprite.color = originColor;
        sprite2.color = originColor2;
        sprite3.color = originColor3;
    }

    public void FlowerEffect()   // �� ������ ����Ʈ
    {
        StartCoroutine(FlowerEffectCor());
    }

    IEnumerator FlowerEffectCor()
    {
        Time.timeScale = 0f;
        // ��ǥ ���� (255, 193, 153) -> RGB ���� 0���� 1�� ��ȯ�Ͽ� ���
        Color changeColor = new Color(204f / 255f, 144f / 255f, 106f / 255f);

        // ���� ��ȭ �ð�
        float changeTime = 0.5f;

        // ���� ���� ����
        Color originColor3 = sprite3.color;

        while (0 < changeTime)
        {
            // sprite�� ���� ����
            sprite3.color = changeColor;
            yield return new WaitForSecondsRealtime(0.1f);
            sprite3.color = originColor3;
            yield return new WaitForSecondsRealtime(0.1f);

            // �ð� ��� 0.1�� ��ŭ 2�� ���ߴϱ�
            changeTime -= 0.2f;
        }

        // ���� ���󺹱�
        sprite3.color = originColor3;
        Time.timeScale = 1f;
    }

    IEnumerator Invincibility()  // �����ð�
    {
        isInvincibility = true;
        yield return new WaitForSeconds(1f);
        isInvincibility = false;
    }


    IEnumerator LoadScene()
    {
        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(2f);
        // �� �ε�
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

        if (collision.gameObject.CompareTag("Enemy") && !sizeUp && !starState) // ���� �浹�� ����
        {
            if (!isInvincibility)
            {
                SoundManager.instance.PlayBackground("Lost a Life");
                PlayerDie();
            }
        }

        if (collision.gameObject.CompareTag("Enemy") && sizeUp && !starState) // ������� ������ �� ���� �浹�� ������� ����
        {

            BasicMario();
        }


        if (collision.gameObject.CompareTag("Enemy") && flowerState) // �ö�� ������ �� ���� �浹�ϸ� �⺻ ������ ���·�
        {
            flowerState = false;  // �ö�� ���� ����
            BasicMario();  // �⺻ ������ ���·� ����
        }



        if (collision.gameObject.CompareTag("DeadKoopa") && !sizeUp && !starState) // ����� �浹�� ����
        {
            PlayerDie();
        }


        if (collision.gameObject.CompareTag("DeadKoopa") && sizeUp && !starState) // ����� �浹�� ������� ����
        {
            sizeUp = false;
            SizeEffect();
        }



    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !starState) // ���� ������ ���� �ö�
        {
            if (!enemyKillJump)
            {
                enemyKillJump = true;
                SoundManager.instance.PlaySfx("stomp");
                rigidbody2D.AddForce(Vector2.up * 200f);
            }
        }

        if (collision.CompareTag("End")) // ��� ����
        {
            SoundManager.instance.PlayBackground("Level Complete");
            anim.SetTrigger("End");
            rigidbody2D.velocity = Vector2.zero;
            moveForce = 0;
            jumpForce = 0;
            rigidbody2D.gravityScale = 0.3f;


        }

        if (collision.CompareTag("Flag"))  // ��߿� ������ ����
        {
            transform.position = new Vector3(14.7f, transform.position.y, 0);
            Vector3 theScale = player.localScale;
            theScale.x = -1;
            player.localScale = theScale;
            player2.localScale = theScale;
            player3.localScale = theScale;
            StartCoroutine(EndOn());
        }

        if (collision.CompareTag("End2"))   // ���� ����
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