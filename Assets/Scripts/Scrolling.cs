using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour {

    private float speed;

    // Use this for initialization
    void Start () {
        speed = 0.1f;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameStart.started)
            this.transform.position = new Vector3(this.transform.position.x-speed, this.transform.position.y,10);
	}
}
