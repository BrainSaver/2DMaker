using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    
    void Awake(){
        //초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update(){
        //Jump
        if(Input.GetButtonUp("Jump") && !anim.GetBool("isJumping")){//점프가 아닌 경우에만 동작
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        //Stop Speed
        if(Input.GetButtonUp("Horizontal")){
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        //Direction Sprite
        float h = Input.GetAxisRaw("Horizontal");
        if(h != 0){
            spriteRenderer.flipX = h == -1;
        }

        //Animation Mathf.Abs : 절대값
        if(Mathf.Abs(rigid.velocity.x) < 0.3 ){
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
        if(rigid.velocity.x > maxSpeed){
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);//오른쪽에 대한 것
        }
        else if(rigid.velocity.x < maxSpeed * (-1)){
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);//왼쪽에 대한 것
        }

        //Landing Platform 바닥으로 레이저를 쏨. 레이저를 쏘면 레이저에 닿은 물체의 정보를 받아옴
        if(rigid.velocity.y < 0){
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
            //Debug.Log("플레이어가 맞았습니다!");
            OnDamaged(collision.transform.position);
        }
    }

    void OnDamaged(Vector2 targetPos){
        gameObject.layer = 8;//무적모드로로
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

}