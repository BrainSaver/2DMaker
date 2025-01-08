using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{   
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsulecollider;

    
    void Awake(){
        //초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
    }

    void Update(){
        //Jump
        if(Input.GetButtonUp("Jump") && !anim.GetBool("isJumping")){//점프가 아닌 경우에만 동작
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        //Stop Speed
        if(Input.GetButtonUp("Horizontal")){
            rigid.linearVelocity = new Vector2(0, rigid.linearVelocity.y);
        }

        //Direction Sprite
        float h = Input.GetAxisRaw("Horizontal");
        if(Input.GetButton("Horizontal")){
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Animation Mathf.Abs : 절대값
        if(Mathf.Abs(rigid.linearVelocity.x) < 0.3 ){
            anim.SetBool("isWalking", false);
        }
        else{
            anim.SetBool("isWalking", true);
        }
    }

    void FixedUpdate(){

        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Speed Limit
        if(rigid.linearVelocity.x > maxSpeed){
            rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);//오른쪽에 대한 것
        }
        else if(rigid.linearVelocity.x < maxSpeed * (-1)){
            rigid.linearVelocity = new Vector2(maxSpeed * (-1), rigid.linearVelocity.y);//왼쪽에 대한 것
        }

        //Landing Platform 바닥으로 레이저를 쏨. 레이저를 쏘면 레이저에 닿은 물체의 정보를 받아옴
        if(rigid.linearVelocity.y < 0){
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            
                if(rayHit.collider != null){
                    if(rayHit.distance < 0.55f)
                        anim.SetBool("isJumping", false);
                }
        }

    }

    //충돌체크 (대문자 주의)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy"){
            //Attack
            if(rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y){
                OnAttack(collision.transform);
            }
            else{
            //Debug.Log("플레이어가 맞았습니다!");
            OnDamaged(collision.transform.position);
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Item"){
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");//Contains(비교문) : 대상 문자열에 비교문이 있으면 true
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            
            if(isBronze)
                gameManager.stagePoint += 50;
            else if(isSilver)
                gameManager.stagePoint += 100;
            else if(isGold)
                gameManager.stagePoint += 300;
            //Deactive Item
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Finish"){
            //Next Stage
            gameManager.NextStage();
        }

    }

    void OnAttack(Transform enemy){
        //Point
        gameManager.stagePoint += 100;
        //Reaction Force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos){
        //healthDown
        gameManager.HealthDown();

        gameObject.layer = 8;//무적모드로
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);//투명도

        //Reaction Force
        int dirc = transform.position.x-targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", 3);//3초뒤에 실행
    }
    
    void OffDamaged(){
        gameObject.layer = 7;//무적모드 해제
        spriteRenderer.color = new Color(1, 1, 1, 1);//투명도
    }

    public void OnDie(){
    //Sprite Alpha
    spriteRenderer.color = new Color(1, 1, 1, 0.4f);

    //Sprite Flip Y
    spriteRenderer.flipY = true;

    //Collider Disable
    capsulecollider.enabled = false;

    //Die Effect Jump
    rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero(){
        rigid.linearVelocity = Vector2.zero;
    }

}