using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    public GameObject pauseMenu, gameOverMenu, timeText, bestTimeText, startMenu;
    List<GameObject> levelChunks = new List<GameObject>();
    bool stop = true;
    float currentPositionX = 0;
    float currentPositionY = 0;
    float timeBegin, timePause;
    float bestScore = 0.0f;
    LoaderJSON loaderJson = new LoaderJSON();

    void Start() {
        DontDestroyOnLoad(gameObject);
        InitializeMap();
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
                foreach (GameObject chunk in levelChunks)
                {
                    chunk.GetComponent<LevelComponent>().speedScrolling = 0;
                }
                pauseMenu.SetActive(true);
                stop = true;
                timePause = Time.time;
            }
        }
    }

    public void StartGame(){
        foreach(GameObject chunk in levelChunks){
            LevelComponent levelComponentScript = chunk.GetComponent<LevelComponent>();
            levelComponentScript.gameStarted = true;
        }
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
            foreach (GameObject chunk in levelChunks)
            {
                chunk.GetComponent<LevelComponent>().speedScrolling = 0.01f;
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
        foreach (GameObject go in levelChunks)
        {
            DestroyImmediate(go);
        }
        levelChunks.Clear();
        InitializeMap();
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
