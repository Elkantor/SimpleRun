using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComponent : MonoBehaviour {

    public bool gameStarted = false;
    SpriteRenderer spriteRenderer;
    public float width;
    public float height;
    public float probability;
    public float colorR;
    public float colorB;
    public float colorG;
    public float speedScrolling = 0.01f;

    public void LoadLevelComponent(float width, float height, float probability, float colorR, float colorG, float colorB){
        this.width = width;
        this.height = height;
        this.probability = probability;
        this.colorR = colorR;
        this.colorG = colorG;
        this.colorB = colorB;
    }

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update(){
        if (gameStarted){
            Debug.Log(transform.position.x);
            transform.position = new Vector3(transform.position.x - speedScrolling, transform.position.y, transform.position.z);
        }
    }

}
