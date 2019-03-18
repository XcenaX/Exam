using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{    

    [SerializeField]
    private int maxHealth;
    private int health;

    [Header("Nav mesh settings")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private NavMeshAgent navMesh;

    [SerializeField]
    private Animator anim;

    private string state = "Idle";
    private bool isAlive = true;

    [Header("Enemy settings")]
    [SerializeField]
    private float searchRadius;

    [SerializeField]
    private float waitTime;
    private float wait;

    [SerializeField]
    private Transform triggerZone;

    private bool highAlert = false;
   private float alertLevel = 0; // уровень тревоги

   [SerializeField]
   private Transform enemyPrefab;


    void Start()
    {
        health = maxHealth;
        navMesh.speed = 1;
        anim.speed = 1;
    }
    
    private void Update(){
        if(isAlive == false)
            return; 
            if(navMesh != null)       
        anim.SetFloat("speed", navMesh.velocity.magnitude);        

        switch(state){
            case "Idle":                   
                GoToRandomPoint();
            break;
            case "Walk":
                CheckDistance();
            break;
            case "Search":
                Search();
            break;
            case "Chase":
                ChaseForPlayer();
            break;
            case "Hunt":
                CheckDistance();
            break;
            case "Kill":                
                anim.speed = 0.4f;
            break;
            default:
            break;
        }
    }

    private void ChaseForPlayer(){
        navMesh.SetDestination(player.position);
        float distance = Vector3.Distance(transform.position, player.position);
        if(distance > 10f){
            state = "Hunt";
            highAlert = true;
            alertLevel = 20;
        }else if(navMesh.remainingDistance <= navMesh.stoppingDistance && navMesh.pathPending == false){            
            var playerController = player.GetComponent<PlayerController>();
            if(playerController.isAlive == true){
                state = "Kill";
                KillPlayer();
            }
        }
    }

    private void KillPlayer(){
        anim.SetTrigger("Kill");
        var playerController = player.GetComponent<PlayerController>();
        playerController.KillPlayer();

        Camera.main.gameObject.SetActive(false);
        Invoke("RestartLevel", 2f);
    }

    private void RestartLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CheckSight(){
        if(isAlive == false)
            return;
        
        RaycastHit hit;
        if(Physics.Linecast(triggerZone.position, player.position, out hit)){            

        if(hit.collider.tag == "Player"){
            if(state != "Kill"){
                state = "Chase";
                navMesh.speed = 2;
                anim.speed = 2;                
            }
        }

        }
    }

    private void GoToRandomPoint(){
        // генерируем случайную позицию внутри сферы
        Vector3 randomPos = Random.insideUnitSphere * searchRadius;
        NavMeshHit navHit;
        NavMesh.SamplePosition(
            transform.position + randomPos, 
            out navHit, 
            searchRadius, 
            NavMesh.AllAreas
        );

        if(highAlert){
            NavMesh.SamplePosition(
            player.position, 
            out navHit, 
            searchRadius, 
            NavMesh.AllAreas            
        );
        alertLevel -=5;
        if(alertLevel <= 0){
            highAlert = false;
            navMesh.speed = 1;
            anim.speed = 1;
        }
        }
        if(navMesh != null)navMesh.SetDestination(navHit.position);
        state = "Walk";
    }

    private void CheckDistance(){
        if(navMesh == null)return;
        var remainingDistance = navMesh.remainingDistance;
        var stoppingDistance = navMesh.stoppingDistance;
        // когда достигли цели
        if(remainingDistance <= stoppingDistance && navMesh.pathPending == false){
            state = "Search";
            wait = waitTime;
        }
    }

    private void Search(){
        if(wait <= 0){
            state = "Idle";
            return;
        }

        wait -= Time.deltaTime;
        transform.Rotate(0, 120f * Time.deltaTime ,0);
    }

    public void SetPlayer(Transform player){
        this.player = player;
    }
    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }


    public void KillEnemy(){
        Destroy(transform.gameObject);
    }
}
