using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapsControl : MonoBehaviour
{
    public enum TrapsType
    {
        Non, FallingPlatforms, Fire, Trampoline, SpikeHead
    }
    public TrapsType trapsType;
    bool isTrapStop = false;
    public Vector2 rewr;
    public float attackSpeed = 100;
    public float backSpeed = 20;
    bool isAttack = false;
    Rigidbody2D enemyRir;
    Vector2 disValue = Vector2.zero;
    private void Awake()
    {
        enemyRir = GetComponent<Rigidbody2D>();
        if (trapsType == TrapsType.SpikeHead)
        {
            StartCoroutine(EnemyAI());
        }
    }
    IEnumerator EnemyAI()
    {
        Transform playerTr = GameObject.Find("Player").transform;
        float checkDisValue = 10;
        Vector2 startPos = transform.position;
        while (playerTr)
        {
            if (!isAttack)
            {
                if (Vector2.Distance(transform.position, startPos) != 0)
                {
                    enemyRir.velocity = Vector2.zero;
                    transform.position = Vector2.MoveTowards(transform.position, startPos, Time.deltaTime * backSpeed);
                }
                else
                {
                    Debug.DrawRay(transform.position, Vector2.left * checkDisValue, Color.red, 0);
                    Debug.DrawRay(transform.position, Vector2.right * checkDisValue, Color.red, 0);
                    Debug.DrawRay(transform.position, Vector2.up * checkDisValue, Color.red, 0);
                    Debug.DrawRay(transform.position, Vector2.down * checkDisValue, Color.red, 0);
                    disValue = Vector2.zero;
                    int layerMask = 1 << LayerMask.NameToLayer("Player");
                    if (Physics2D.Raycast(transform.position, Vector2.left, checkDisValue, layerMask))
                    {
                        disValue = Vector2.left;
                    }
                    else if (Physics2D.Raycast(transform.position, Vector2.right, checkDisValue, layerMask))
                    {
                        disValue = Vector2.right;
                    }
                    else if (Physics2D.Raycast(transform.position, Vector2.up, checkDisValue, layerMask))
                    {
                        disValue = Vector2.up;
                    }
                    else if (Physics2D.Raycast(transform.position, Vector2.down, checkDisValue, layerMask))
                    {
                        disValue = Vector2.down;
                    }

                    if (disValue != Vector2.zero)
                    {
                        Attack();
                        isAttack = true;
                    }
                }
            }
            else
            {
                Debug.Log(gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Ready"));
                if (!gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Ready"))
                {
                    gameObject.GetComponent<Animator>().SetInteger("AttackDirection", -1);
                    gameObject.GetComponent<Animator>().SetTrigger("Attack");
                    yield return new WaitForSeconds(1);
                }
                else
                {
                    enemyRir.velocity = disValue * attackSpeed;
                }
            }
            yield return null;
        }
    }

    void OnBecameInvisible()
    {
        if (isTrapStop)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Player"))
        {
            ContactPoint2D contact = col.contacts[0];
            Vector3 pos = (Vector2)transform.position - contact.point;

            Debug.Log(col.transform.tag);
            switch (trapsType)
            {
                case TrapsType.FallingPlatforms:
                    if (col.transform.position.y > transform.position.y + 0.4f)
                    {
                        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    }
                    isTrapStop = true;
                    break;
                case TrapsType.Fire:
                    if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Fire_Idle") && pos.normalized.y < -0.7f)
                    {
                        gameObject.GetComponent<Animator>().SetTrigger("Working");
                    }
                    break;
                case TrapsType.Trampoline:
                    if (col.transform.position.y > transform.position.y + 0.4f)
                    {
                        col.transform.GetComponent<PlayerControl>().Jump(25, true);
                        gameObject.GetComponent<Animator>().SetTrigger("Working");
                    }
                    break;
                case TrapsType.SpikeHead:
                    col.transform.GetComponent<PlayerControl>().PlayerDamage(0); 
                    Attack();
                    isAttack = false;
                    break;
            }
        }
        else if (col.transform.CompareTag("Ground"))
        {
            switch (trapsType)
            {
                case TrapsType.SpikeHead:
                    Attack();
                    isAttack = false;
                    break;
            }
        }

    }
    void Attack()
    {
        int attackNum = 0;
        if (disValue == Vector2.left)
        {
            attackNum = 0;
        }
        else
        {
            attackNum = 1;
        }
        if (disValue == Vector2.up)
        {
            attackNum = 2;
        }
        else
        {
            attackNum = 3;
        }
        gameObject.GetComponent<Animator>().SetInteger("AttackDirection",attackNum);
    }
}
