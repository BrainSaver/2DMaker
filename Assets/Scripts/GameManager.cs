using UnityEngine;
//점수, 스테이지 이동 관리리
public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;

    public void NextStage()
    {
        stageIndex++;

        totalPoint += stagePoint;
        stagePoint = 0;
    }
    public void HealthDown(){
        if(health > 1){
            health--;
        }
        else{
            //Player Die Effect
            player.OnDie();
            //Result UI

            //Retry Button UI
            Debug.Log("Player Die!!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Player"){
            //Player Reposition
            if(health > 1){
            collision.attachedRigidbody.linearVelocity = Vector2.zero;
            collision.transform.position = new Vector3(0, 0, -1);
            }
            
            //Health Down
            HealthDown();
            
        }
    }
}
