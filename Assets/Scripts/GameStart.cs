using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {

    public static bool started;
	// Use this for initialization
	void Start () {
        started = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            started = true;
            Destroy(gameObject);
        }

    }
}
