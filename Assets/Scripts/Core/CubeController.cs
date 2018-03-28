using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {
    public float maxSpeed = 10f;
    private Rigidbody2D _rigidBody2D;
    private bool isJumping = false;
    // Use this for initialization
    void Start () {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        float translation = Input.GetAxis("Vertical")*maxSpeed;
        //Debug.Log(Input.GetAxis("Vertical"));
        if (Input.GetAxis("Vertical")>0)
            isJumping = true;

        _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, translation);
        if (_rigidBody2D.position.y > 1)
            _rigidBody2D.position = new Vector2(_rigidBody2D.position.x, 1);
    }
}
