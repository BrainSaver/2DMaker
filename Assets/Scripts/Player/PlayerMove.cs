using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
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
        //Stop Speed
        if(Input.GetButtonUp("Horizontal")){
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        //Direction Sprite
        float h = Input.GetAxisRaw("Horizontal");
        if(h != 0){
            spriteRenderer.flipX = h == -1;
        }

        //Animation
        //Mathf.Abs : 절대값
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
    }
}
