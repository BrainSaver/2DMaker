using UnityEngine;
//점수, 스테이지 이동 관리리
public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public void NextStage()
    {   
        //Change Stage
        if(stageIndex < Stages.Length - 1){
        Stages[stageIndex].SetActive(false);
        stageIndex++;
        Stages[stageIndex].SetActive(true);
        PlayerReposition();
        }
        else{
            //Game Clear
            Time.timeScale = 0;//시간을 멈춤
            Debug.Log("Game Clear!!");
        }

        //Calculate point
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
            PlayerReposition();
            }
            
            //Health Down
            HealthDown();
            
        }
    }

    void PlayerReposition(){
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }
}
