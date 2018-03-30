using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public GameObject pauseText, gameOverText;
    List<GameObject> levelChunks = new List<GameObject>();
    GameObject character;
    GameObject fallingBlock;
    bool stop = true;
    float currentPositionX = 0;
    float currentPositionY = 0;

    void Start() {
        character = GameObject.Find("Character");
        fallingBlock = GameObject.Find("Fallingblock");
        DontDestroyOnLoad(gameObject);
        InitializeMap();
    }

    private void InitializeMap() {
        currentPositionX = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x;
        currentPositionY = 0;
        LoaderJSON loaderChunks = new LoaderJSON();
        Chunk[] chunks = loaderChunks.LoadGameData("Datas/Chunks.json");

        float[] probabilities = new float[chunks.Length];

        probabilities[0] = chunks[0].chunkProbability;
        for (int i = 1; i < chunks.Length; i++) {
            probabilities[i] = probabilities[i - 1] + chunks[i].chunkProbability;
        }

        for (int i = 0; i < 20; i++) {
            float probability = Random.Range(0.0f, 1.0f);
            float previousProbability = 0.0f;
            for (int j = 0; j < probabilities.Length; j++) {
                if (probability > previousProbability && probability <= probabilities[j]) {
                    if (i == 0){
                        CreateChunk(chunks[j], null, chunks);
                    }
                    else{
                        CreateChunk(chunks[j], levelChunks[i-1], chunks);
                    }
                }
                previousProbability = probabilities[j];
            }
        }
        levelChunks[0].GetComponent<LevelComponent>().previousChunk = levelChunks[levelChunks.Count - 1];
    }

    void CreateChunk(Chunk chunk, GameObject previousChunk, Chunk[] chunksPossibilities){
        GameObject chunkGameObject = new GameObject("Chunk" + levelChunks.Count);
        LevelComponent levelComponent = chunkGameObject.AddComponent<LevelComponent>();

        levelComponent.LoadLevelComponent(
            chunk.chunkWidth,
            chunk.chunkHeight,
            chunk.chunkProbability,
            chunk.chunkColorRed,
            chunk.chunkColorGreen,
            chunk.chunkColorBlue,
            previousChunk,
            chunksPossibilities
        );
        
        GameObject templateGameObject = GameObject.Find("Ground");
        SpriteRenderer templateSpriteRenderer = templateGameObject.GetComponent<SpriteRenderer>();
        BoxCollider2D templateBoxCollider2D = templateGameObject.GetComponent<BoxCollider2D>();

        SpriteRenderer chunkSpriteRenderer = chunkGameObject.AddComponent<SpriteRenderer>();
        chunkSpriteRenderer.color = new Color(levelComponent.colorR, levelComponent.colorG, levelComponent.colorB);
        chunkSpriteRenderer.sprite = templateSpriteRenderer.sprite;

        BoxCollider2D chunkBoxCollider2D = chunkGameObject.AddComponent<BoxCollider2D>();
        chunkBoxCollider2D.size = new Vector2(0.2f, 0.2f);

        // Set the position of the new game object
        float newX = this.currentPositionX + (levelComponent.width / 10);
        //float newY = this.currentPositionY + (levelComponent.height / 10);
        float newY = levelComponent.height / 10;
        chunkGameObject.transform.position = new Vector3(newX, newY, 0.0f);


        // Update the coordinates of the current game object created in the scene
        this.currentPositionX = currentPositionX + (levelComponent.width / 5);

        // Update the scale of the new game object
        chunkGameObject.transform.localScale = new Vector3(chunk.chunkWidth, chunk.chunkHeight, 1.0f);

        levelChunks.Add(chunkGameObject);

    }
    
    public void GameOver()
    {
        // Stop level movment
        foreach (GameObject chunk in levelChunks)
        {
            chunk.GetComponent<LevelComponent>().speedScrolling = 0;
        }

        //Active GameOver
        gameOverText.SetActive(true);
        stop = true;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartGame();
        }
        //To restart
        if (Input.GetKeyDown("r"))
        {
            foreach(GameObject go in levelChunks)
            {
                DestroyImmediate(go);
            }
            levelChunks.Clear();
            InitializeMap();
        }
        if (stop)
        {
            if (Input.GetKeyDown("escape"))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown("c"))
            {
                foreach (GameObject chunk in levelChunks)
                {
                    chunk.GetComponent<LevelComponent>().speedScrolling = 0.01f;
                }
                pauseText.SetActive(false);
                stop = false;
            }
        }
        else
        {
            if (Input.GetKeyDown("escape"))
            {
                // Stop level movment
                foreach (GameObject chunk in levelChunks)
                {
                    chunk.GetComponent<LevelComponent>().speedScrolling = 0;
                }
                pauseText.SetActive(true);
                stop = true;
            }
        }
    }

    void StartGame(){
        foreach(GameObject chunk in levelChunks){
            LevelComponent levelComponentScript = chunk.GetComponent<LevelComponent>();
            levelComponentScript.gameStarted = true;
        }
        character.GetComponent<CubeController>().gameStarted=true;
        fallingBlock.GetComponent<FallingBlockScript>().gameStarted = true;
        stop = false;
    }
}
