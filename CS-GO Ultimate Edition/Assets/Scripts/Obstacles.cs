using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    
    [SerializeField]
    private int health;        

    private void Break(){
        Destroy(this.gameObject);
    }

public void SetHealth(int count){
    health -= count;
}
    
    void Update()
    {
        if(health <= 0)Break();
    }
}
