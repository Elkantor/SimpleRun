using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public GameObject gameOverText;

    private List<LevelComponent> levelComponents = new List<LevelComponent>();
    private float currentPositionX = 0;
    private float currentPositionY = 0;


    void Start () {
		DontDestroyOnLoad(gameObject);
        currentPositionX = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x;
        InitializeMap();
	}
	
	private void InitializeMap(){
		LoaderJSON loaderChunks = new LoaderJSON();
		Chunk[] chunks = loaderChunks.LoadGameData("Datas/Chunks.json");
        GameObject chunk = CreateChunk(chunks[0]);
        GameObject chunk2 = CreateChunk(chunks[1]);
    }

    GameObject CreateChunk(Chunk chunk){
        GameObject chunkGameObject = new GameObject("Chunk" + levelComponents.Count);
        LevelComponent levelComponent = chunkGameObject.AddComponent<LevelComponent>();

        levelComponent.LoadLevelComponent(
            chunk.chunkWidth,
            chunk.chunkHeight,
            chunk.chunkProbability,
            chunk.chunkColorRed,
            chunk.chunkColorGreen,
            chunk.chunkColorBlue
        );
        
        levelComponents.Add(levelComponent);
        
        GameObject templateGameObject = GameObject.Find("Ground");
        SpriteRenderer templateSpriteRenderer = templateGameObject.GetComponent<SpriteRenderer>();
        BoxCollider2D templateBoxCollider2D = templateGameObject.GetComponent<BoxCollider2D>();

        SpriteRenderer chunkSpriteRenderer = chunkGameObject.AddComponent<SpriteRenderer>();
        chunkSpriteRenderer.color = new Color(levelComponent.colorR, levelComponent.colorG, levelComponent.colorB);
        chunkSpriteRenderer.sprite = templateSpriteRenderer.sprite;

        BoxCollider2D chunkBoxCollider2D = chunkGameObject.AddComponent<BoxCollider2D>();
        chunkBoxCollider2D.size = new Vector2(0.2f, 0.2f);

        Debug.Log("currentPosX " + currentPositionX);
        // Set the position of the new game object
        float newX = this.currentPositionX + (levelComponent.width / 10);
        float newY = this.currentPositionY + (levelComponent.height / 10);
        chunkGameObject.transform.position = new Vector3(newX, newY, 0);
        Debug.Log("pos new object " + newX);


        // Update the coordinates of the current game object created in the scene
        this.currentPositionX = currentPositionX + (levelComponent.width / 5);
        this.currentPositionY = newY - (levelComponent.height / 10);

        // Update the scale of the new game object
        chunkGameObject.transform.localScale = new Vector2(chunk.chunkWidth, chunk.chunkHeight);

        return chunkGameObject;

    }

    public void GameOver()
    {
        // Stop level movment
        foreach (LevelComponent lvl in levelComponents)
        {
            lvl.speedScrolling = 0;
        }

        //Active GameOver
        gameOverText.SetActive(true);
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

	void Update ()
    {
        //To restart
        if (Input.GetKey("r"))
        {
            //TO DO : Start function
        }
    }
}
