using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour {
    private ParticleSystem particleSystem;
	// Use this for initialization
	void Start () {
        particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newpos = GameObject.Find("Character").transform.position;
        particleSystem.transform.position = new Vector3(newpos.x,newpos.y-1.5f,newpos.z);
	}
}
