using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LevelChunk
{
    public List<GameObject> gameObjects;
}

public class Main : MonoBehaviour {

    public GameObject pauseText, gameOverText;
    //List<GameObject> levelChunks = new List<GameObject>();
    public List<LevelChunk> levelChunks = new List<LevelChunk>();
    bool stop = true;
    float currentPositionX = 0;
    float currentPositionY = 0;

    void Start() {
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

        levelChunks.Clear();
        for (int i = 0; i < 20; i++) {
            float probability = Random.Range(0.0f, 0.99f);
            float previousProbability = 0.0f;
            for (int j = 0; j < chunks.Length; j++) {
                if (probability > previousProbability && probability <= probabilities[j]) {
                    if (i == 0){
                        CreateChunk(chunks[j], new LevelChunk(), chunks);
                        break;
                    }
                    else{
                        CreateChunk(chunks[j], levelChunks[i-1], chunks);
                        break;
                    }
                }
                previousProbability = probabilities[j];
            }
        }
        foreach (GameObject go in levelChunks[0].gameObjects)
        {
            go.GetComponent<LevelComponent>().previousChunk = levelChunks[levelChunks.Count - 1];
        }
    }

    void CreateChunk(Chunk chunk, LevelChunk previousChunk, Chunk[] chunksPossibilities){
        int chunkNumber = 0;
        foreach(LevelChunk lc in levelChunks)
        {
            if(lc.gameObjects != null)
            {
                chunkNumber += lc.gameObjects.Count;
            }
        }
        GameObject chunkGameObject = new GameObject("Chunk" + chunkNumber);
        chunkNumber++;
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

        LevelChunk levelChunk = new LevelChunk();
        levelChunk.gameObjects = new List<GameObject>();
        levelChunk.gameObjects.Add(chunkGameObject);

        // If the chunk is a corridor, we must create the top part in addition to the bottom part
        if(chunk.chunkId == 2)
        {
            GameObject chunkGameObject2 = new GameObject("Chunk" + chunkNumber);
            LevelComponent levelComponent2 = chunkGameObject2.AddComponent<LevelComponent>();

            levelComponent2.LoadLevelComponent(
                chunk.chunkWidth,
                chunk.chunkHeight,
                chunk.chunkProbability,
                chunk.chunkColorRed,
                chunk.chunkColorGreen,
                chunk.chunkColorBlue,
                previousChunk,
                chunksPossibilities
            );

            SpriteRenderer chunkSpriteRenderer2 = chunkGameObject2.AddComponent<SpriteRenderer>();
            chunkSpriteRenderer2.color = new Color(chunk.chunkColorRed, chunk.chunkColorGreen, chunk.chunkColorBlue);
            chunkSpriteRenderer2.sprite = templateSpriteRenderer.sprite;

            BoxCollider2D chunkBoxCollider2D2 = chunkGameObject2.AddComponent<BoxCollider2D>();
            chunkBoxCollider2D2.size = new Vector2(0.2f, 0.2f);

            float newY2 = newY + levelComponent.height / 10 + 1;
            chunkGameObject2.transform.position = new Vector3(newX, newY2, 0.0f);
            chunkGameObject2.transform.localScale = new Vector3(chunk.chunkWidth, chunk.chunkHeight, 1.0f);

            levelChunk.gameObjects.Add(chunkGameObject2);
        }
        levelChunks.Add(levelChunk);

    }
    
    public void GameOver()
    {
        // Stop level movment
        foreach (LevelChunk lc in levelChunks)
        {
            Debug.Log("Count game objects" + lc.gameObjects.Count);
            foreach(GameObject go in lc.gameObjects){
                go.GetComponent<LevelComponent>().speedScrolling = 0;
            }
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
            foreach(LevelChunk lc in levelChunks)
            {
                foreach(GameObject go in lc.gameObjects)
                {
                    DestroyImmediate(go);
                }
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
                foreach(LevelChunk lc in levelChunks)
                {
                    foreach (GameObject go in lc.gameObjects)
                    {
                        go.GetComponent<LevelComponent>().speedScrolling = 0.01f;
                    }
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
                foreach (LevelChunk lc in levelChunks)
                {
                    foreach (GameObject go in lc.gameObjects)
                    {
                        go.GetComponent<LevelComponent>().speedScrolling = 0;
                    }
                }
                pauseText.SetActive(true);
                stop = true;
            }
        }
    }

    void StartGame(){
        foreach (LevelChunk lc in levelChunks)
        {
            foreach (GameObject go in lc.gameObjects)
            {
                LevelComponent levelComponentScript = go.GetComponent<LevelComponent>();
                levelComponentScript.gameStarted = true;
            }
        }
        stop = false;
    }
}
