using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockScript : MonoBehaviour {

    public float speedScrolling = 0.01f;
    public bool gameStarted;
    float hautcameraY;
    float speedShake=50.0f;
    float timeShake=1.5f;
    float startTime = 0.0f;
    float nextActionTime=0.05f;
    Rigidbody2D _rigidbody2D;

    // Use this for initialization
    void Start () {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        hautcameraY = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).y;
        Vector3 newpos = GameObject.Find("Character").transform.position;
        transform.position = new Vector2(newpos.x+1,newpos.y+2);
	}
	
	// Update is called once per frame
	void Update () {
        if (gameStarted)
        {
            transform.position = new Vector3(transform.position.x - speedScrolling, transform.position.y, transform.position.z);
        }
        Vector3 newpos = GameObject.Find("Character").transform.position;
        if (Time.time > nextActionTime)
        {
            startTime = Time.time;
            _rigidbody2D.simulated = false;
            transform.position = new Vector2(newpos.x + 1, hautcameraY + Camera.main.transform.position.y * 2);
            nextActionTime = Time.time+Random.Range(4.0f, 8.0f);
            
        }
        float AngleAmount = (Mathf.Sin(Time.time * speedShake) * 180) / Mathf.PI * 0.5f;
        AngleAmount = Mathf.Clamp(AngleAmount, -15, 15);
        if (Time.time < startTime + timeShake)
            transform.localRotation = Quaternion.Euler(0, 0, AngleAmount);
            //transform.position = new Vector2(transform.position.x + move, transform.position.y);
        else
            _rigidbody2D.simulated = true;
    }
}
