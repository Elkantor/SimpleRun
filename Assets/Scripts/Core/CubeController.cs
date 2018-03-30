using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float maxSpeed = 10f;
    public Main gameManager;

    private Rigidbody2D _rigidBody2D;
    private int jumpCount=0;
    float cameraBoundXNegative;
    bool becameInvisible = false;

    // Use this for initialization
    void Start()
    {
        if (!gameManager && GameObject.Find("GameManager"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<Main>();
        }
        _rigidBody2D = GetComponent<Rigidbody2D>();
        cameraBoundXNegative = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            ResetPosition();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount == 0)
        {
            _rigidBody2D.AddForce(new Vector2(0, 500));
            transform.localScale += new Vector3(-0.5F, 0.5F, 0);
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
        if ((transform.position.x + transform.localScale.x / 10) <= cameraBoundXNegative)
        {
            if(!becameInvisible)
            {
                gameManager.GameOver();
                becameInvisible = true;
            }
        }
        else
        {
            becameInvisible = false;
        }
        //_rigidBody2D.position = new Vector2(_rigidBody2D.position.x+0.01f, _rigidBody2D.position.y);
        //Debug.Log();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (jumpCount == 1)
        {
            transform.localScale += new Vector3(0.5F, -0.5F, 0);
        }
        jumpCount = 0;
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0.0f, 2.0f, 0.0f);
    }
}
