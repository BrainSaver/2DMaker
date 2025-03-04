using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsulecollider;

    public int nextMove;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsulecollider = GetComponent<CapsuleCollider2D>();

        Invoke("Think", 5);//5초 후에 Think 함수를 실행
    }

    void FixedUpdate()
    {
        //Move
        rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);


            //Platform Check
            Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
            
            if(rayHit.collider == null){
                Debug.Log("경고! 이 앞은 낭떠러지.");
                Turn();
            }
    }

    //재귀 함수
    void Think(){
        nextMove = Random.Range(-1, 2);//-1, 0, 1

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        if(nextMove != 0){
            Turn();
        }

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);//nextThinkTime초 후에 Think 함수를 실행
    }

    void Turn(){
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();//Invoke 함수를 멈춤
        Invoke("Think", 2);
    }

    void OnAttack(Transform enemy){
    //Point

    //Enemy Die
    EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
    enemyMove.OnDamaged();
    }

    public void OnDamaged(){
    //Sprite Alpha
    spriteRenderer.color = new Color(1, 1, 1, 0.4f);

    //Sprite Flip Y
    spriteRenderer.flipY = true;

    //Collider Disable
    capsulecollider.enabled = false;

    //Die Effect Jump
    rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

    //Destroy
    Invoke("DeActive", 5);
    }

    void DeActive(){
        gameObject.SetActive(false);
    }
}
