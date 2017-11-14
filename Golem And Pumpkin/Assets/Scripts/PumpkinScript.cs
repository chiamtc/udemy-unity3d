using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinScript : MonoBehaviour {



	// Use this for initialization
	void Update () {
		if(transform.position.y < -3f){
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter(Collision target){
        if(target.gameObject.tag == "Golem"){
            Destroy(target.gameObject);
            Time.timeScale = 0f;
        }

    }

}
