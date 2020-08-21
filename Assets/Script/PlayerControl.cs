using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [Header("캐릭터 설정")]
    public float speed = 3;
    public int jumpMaxNum = 2;
    public float playerJumpPower = 15;
    public float playerMaxHP = 100;
    public float invincibilityTime = 1;
    public JoystickControl script_JoystickControl;
    public Image playerHPImage;

    float playerHP;
    float PlayerHP
    {
        set
        {
            playerHP = value;
            
            if (playerHP > playerMaxHP)
            {
                playerHP = playerMaxHP;
            }
        }
        get
        {
            return playerHP;
        }
    }

    public bool isNoJump = false;

    int animState = 0;
    int jumpCount = 0;
    bool isInvincibility = false;
    SpriteRenderer characterRender;
    Animator characterAnimator;
    Rigidbody2D characterRig;

    void Start()
    {
        PlayerHP = playerMaxHP;
        characterRender = GetComponent<SpriteRenderer>();
        characterAnimator = GetComponent<Animator>();
        characterRig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        Move();
        bool isGround = GroundCheck();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UI_JumpButton();
        }
        else if (Mathf.Abs(characterRig.velocity.y) < 1&& isGround)
        {
            jumpCount = 0;
        }
        characterAnimator.SetInteger("JumpCount", jumpCount);
        characterAnimator.SetInteger("AnimState", animState);
        characterAnimator.SetBool("IsGround", isGround);

        playerHPImage.fillAmount = (PlayerHP / playerMaxHP);
        if (PlayerHP <= 0)
        {
            Destroy(gameObject);
        }
    }
    void Move()
    {
        float moveVaule = script_JoystickControl.GetJoystickVecValue.x;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            moveVaule = Input.GetAxisRaw("Horizontal");

        if (moveVaule != 0)
        {
            characterRender.flipX = moveVaule < 0;
            animState = 1;
        }
        else
        {
            animState = 0;
        }
        //transform.localPosition += new Vector3(moveVaule * Time.deltaTime * speed, 0, 0);

        if (Mathf.Abs(characterRig.velocity.x) < speed)
        {
            characterRig.AddForce(new Vector2(50 * moveVaule, 0));
        }
        else
        {
            characterRig.velocity = new Vector2(speed * moveVaule, characterRig.velocity.y);
        }
    }
    public void UI_JumpButton()
    {
        if (jumpCount < jumpMaxNum)
        {
            if (jumpCount == 0)
                characterAnimator.SetTrigger("Jump");
            Jump(playerJumpPower, false);
            jumpCount++;
        }
    }
    public void Jump(float jumpPower,bool isStopJump)
    {
        if (isStopJump)
        {
            jumpCount = 2; 
            characterAnimator.SetTrigger("Trampoline");
        }
        //characterRig.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        characterRig.velocity = Vector2.up * jumpPower;        
    }
    bool GroundCheck()
    {
        Debug.DrawRay(transform.position, Vector2.down * 0.1f, Color.red, 0);
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        return Physics2D.Raycast(transform.position, Vector2.down, 0.1f, layerMask);
    }
    IEnumerator Invincibility()
    {
        isInvincibility = true;
        //yield return new WaitForSeconds(invincibilityTime);

        float timer = invincibilityTime;
        float blinkingSpeed = 10;
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        while (true)
        {
            playerRenderer.color = new Color(1, 1, 1, Mathf.Abs(Mathf.Sin(timer * blinkingSpeed)));
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                break;
            }
            yield return null;
        }
        playerRenderer.color = new Color(1, 1, 1, 1);

        isInvincibility = false;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Item"))
        {
            if (col.GetComponent<SpriteRenderer>().sprite.name == "apple")
            {
                Destroy(col.transform.gameObject);
                PlayerHP += 10;
            }
            else if (col.GetComponent<SpriteRenderer>().sprite.name == "hp")
            {
                Destroy(col.transform.gameObject);
                PlayerHP += 50;
            }
        }
        else if (col.CompareTag("Enemy") && !isInvincibility)
        {
            PlayerDamage(20);
        }
    }
    public void PlayerDamage(float value)
    {
        StartCoroutine(Invincibility());
        characterAnimator.SetTrigger("Hit");
        PlayerHP -= value;
    }
}
