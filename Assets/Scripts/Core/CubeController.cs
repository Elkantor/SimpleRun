using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float maxSpeed = 10f;
    private Rigidbody2D _rigidBody2D;
    private int jumpCount=0;
    private bool droit=true;
    private AudioSource[] audioSources;
    private AudioSource audioJump;
    private AudioSource audioSquish;

    // Use this for initialization
    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        audioJump = audioSources[0];
        audioSquish = audioSources[1];
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Round(transform.rotation.eulerAngles.z) == 90 || Mathf.Round(transform.rotation.eulerAngles.z) == 270)
            droit = false;
        else
            droit = true;
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount == 0)
        {
            _rigidBody2D.AddForce(new Vector2(0, 500));
            if (!droit)
                transform.localScale += new Vector3(0.5F, -0.5F, 0);
            else
                transform.localScale += new Vector3(-0.5F, 0.5F, 0);
            audioJump.Play();
            jumpCount = 1;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (droit)
                transform.localScale += new Vector3(0.5F, -0.5F,0);
            else
                transform.localScale += new Vector3(-0.5F, 0.5F, 0);
            audioSquish.Play();
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (droit)
                transform.localScale += new Vector3(-0.5F, 0.5F, 0);
            else
                transform.localScale += new Vector3(0.5F, -0.5F, 0);
        }
        //_rigidBody2D.position = new Vector2(_rigidBody2D.position.x+0.01f, _rigidBody2D.position.y);
        //Debug.Log();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (jumpCount == 1)
        {
            if (droit)
                transform.localScale += new Vector3(0.5F, -0.5F, 0);
            else
                transform.localScale += new Vector3(-0.5F, 0.5F, 0);
            Debug.Log(droit);
        }
        jumpCount = 0;
    }
}
