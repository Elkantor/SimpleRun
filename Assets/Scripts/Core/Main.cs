using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct LevelChunk
{
    public List<GameObject> gameObjects;
}

public class Main : MonoBehaviour {
    
    public GameObject pauseMenu, gameOverMenu, timeText, bestTimeText, startMenu;
    GameObject character;
    public List<LevelChunk> levelChunks = new List<LevelChunk>();
    bool stop = true;
    float currentPositionX = 0;
    float currentPositionY = 0;
    public float timeBegin, timePause;
    float bestScore = 0.0f;
    LoaderJSON loaderJson = new LoaderJSON();
    GameObject fallingBlock;
    int indexBlockFalling = 0;
    int updateCount = 0;
    List<GameObject> fallingBlocks = new List<GameObject>();

    void Start() {
        character = GameObject.Find("Character");
        fallingBlock = GameObject.Find("Fallingblock");
        DontDestroyOnLoad(gameObject);
        InitializeMap();
        InitializeBlocks();
    }

    private void InitializeBlocks()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject newblock = Instantiate(fallingBlock);
            fallingBlocks.Add(newblock);
        }
        fallingBlock.SetActive(false);
    }

    private void InitializeMap() {
        currentPositionX = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x;
        currentPositionY = 0;
        Chunk[] chunks = loaderJson.LoadGameData("Datas/Chunks.json");
        bestScore = loaderJson.LoadBestScore("Datas/Scores.json");
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

            float newY2 = newY + (levelComponent.height / 10) + 0.6f;
            chunkGameObject2.transform.position = new Vector3(newX + 0.1f, newY2, 0.0f);
            chunkGameObject2.transform.localScale = new Vector3(chunk.chunkWidth - 1, chunk.chunkHeight, 1.0f);

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
        float newTime = Time.time - timeBegin;
        if (newTime > bestScore)
        {
            bestScore = newTime;
            loaderJson.SaveBestScore(bestScore, "Datas/Scores.json");

            UpdateBestTime("New record");
        }
        //Active GameOver
        gameOverMenu.SetActive(true);
        stop = true;
    }

    void Update()
    {
        //To restart
        if (Input.GetKeyDown("r"))
        {
            Restart();
        }
        if (stop)
        {
            if (Input.GetKeyDown("space"))
            {
                StartGame();
            }
            if (Input.GetKeyDown("escape"))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown("c"))
            {
                Continue();
            }
        }
        else
        {
            float newTime = Time.time - timeBegin;
            int min = (int)(newTime / 60);
            int sec = (int)(newTime % 60);
            string secStr = sec < 10 ? ("0" + sec.ToString()) : sec.ToString();
            timeText.GetComponent<Text>().text = "Score time : " + min + ":" + secStr;

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
                pauseMenu.SetActive(true);
                stop = true;
                timePause = Time.time;
            }
            if(updateCount % 1000 == 0)
            {
                Debug.Log("1000");
                if (Random.Range(0.0f, 1.0f) > 0.5f)
                {
                    fallingBlocks[indexBlockFalling].GetComponent<FallingBlockScript>().EnableFalling();
                    indexBlockFalling++;
                    if (indexBlockFalling > 3) indexBlockFalling = 0;
                }
                updateCount = 1;
            }
            updateCount++;
        }
    }


    public void StartGame(){
        foreach (LevelChunk lc in levelChunks)
        {
            foreach (GameObject go in lc.gameObjects)
            {
                LevelComponent levelComponentScript = go.GetComponent<LevelComponent>();
                levelComponentScript.gameStarted = true;
            }
        }
        character.GetComponent<CubeController>().gameStarted=true;
        fallingBlock.GetComponent<FallingBlockScript>().gameStarted = true;
        stop = false;
        timeText.SetActive(true);
        bestTimeText.SetActive(true);
        timeBegin = Time.time;
        startMenu.SetActive(false);
        UpdateBestTime("Best time");
    }

    public void Continue()
    {
        if(stop)
        {
            foreach (LevelChunk lc in levelChunks)
            {
                foreach (GameObject go in lc.gameObjects)
                {
                    go.GetComponent<LevelComponent>().speedScrolling = 0.01f;
                }
            }
            pauseMenu.SetActive(false);
            stop = false;
            timeBegin += Time.time - timePause;
        }
    }

    public void Restart()
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        foreach (LevelChunk lc in levelChunks)
        {
            foreach (GameObject go in lc.gameObjects)
            {
                DestroyImmediate(go);
            }
        }
        levelChunks.Clear();
        InitializeMap();
        character.GetComponent<CubeController>().speedScrolling = 0.01f;
        StartGame();
    }

    public void UpdateBestTime(string label)
    {
        int minBest = (int)(bestScore / 60);
        int secBest = (int)(bestScore % 60);
        string secBestStr = secBest < 10 ? ("0" + secBest.ToString()) : secBest.ToString();
        bestTimeText.GetComponent<Text>().text = label+" : " + minBest + ":" + secBestStr;
    }
}
