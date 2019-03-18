using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

   [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float interactDistance;

    [SerializeField]
    private Transform cameraPosition;
       [SerializeField]
    private Image interactImage;
        private int health;
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int damage;
    [SerializeField]
    private Transform bulletPrefab;

    [SerializeField]
    private Text healthText;


    public bool isAlive = true;

    void Start()
    {
     health = maxHealth;   
    }

    private void Update(){     
        healthText.text = health.ToString() + "%";  
        Ray ray = new Ray(cameraPosition.position, cameraPosition.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, interactDistance, layerMask)){
            interactImage.color = Color.red;
            if(Input.GetKeyDown(KeyCode.Mouse0)){
                switch(hit.collider.tag){
                    case "Enemy":
                        Destroy(hit.collider.gameObject);
                    break;
                    case "Wall":
                        var shoot = Instantiate(bulletPrefab,hit.point,Quaternion.identity);
                        
                    break;
                    case "Obstacle":
                        hit.collider.gameObject.GetComponent<Obstacles>().SetHealth(damage);
                        shoot = Instantiate(bulletPrefab,hit.point,Quaternion.identity);
                        shoot.LookAt(hit.collider.transform.position);
                    break;
                }
            }            
        }else{
            interactImage.color = Color.green;
        }
        
    }

    public void KillPlayer(){
        
    }
}
