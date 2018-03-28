﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float maxSpeed = 10f;
    private Rigidbody2D _rigidBody2D;
    private int jumpCount=0;

    // Use this for initialization
    void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount == 0)
        {
            _rigidBody2D.AddForce(new Vector2(0, 500));
            jumpCount = 1;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.localScale += new Vector3(0.5F, -0.5F,0);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            transform.localScale += new Vector3(-0.5F, 0.5F, 0);
        }
        //_rigidBody2D.position = new Vector2(_rigidBody2D.position.x+0.01f, _rigidBody2D.position.y);
        //Debug.Log();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        jumpCount = 0;
    }
}