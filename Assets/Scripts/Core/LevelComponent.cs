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
    //public GameObject previousChunk;
    public LevelChunk previousChunk;
    public Chunk[] chunksPossibilities;
    float[] probabilities;
    float cameraBoundXNegative;

    public void LoadLevelComponent(
        float width, 
        float height, 
        float probability, 
        float colorR, 
        float colorG, 
        float colorB, 
        //GameObject previousChunk, 
        LevelChunk previousChunk,
        Chunk[] chunksPossibilities
    )
    {
        this.width = width;
        this.height = height;
        this.probability = probability;
        this.colorR = colorR;
        this.colorG = colorG;
        this.colorB = colorB;
        this.previousChunk = previousChunk;
        this.chunksPossibilities = chunksPossibilities;
        probabilities = new float[chunksPossibilities.Length];
        probabilities[0] = chunksPossibilities[0].chunkProbability;
        for (int i = 1; i < chunksPossibilities.Length; i++)
        {
            probabilities[i] = probabilities[i - 1] + chunksPossibilities[i].chunkProbability;
        }
        

    }

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraBoundXNegative = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x;

    }

    // Update is called once per frame
    void Update(){
        if (gameStarted){
            Main main = GameObject.Find("GameManager").GetComponent<Main>();
            speedScrolling += Mathf.Lerp(main.timeBegin, Time.time, Mathf.SmoothStep(main.timeBegin, Time.time, 0.01f)) / 1000000.0f;
            transform.position = new Vector3(transform.position.x - speedScrolling, transform.position.y, transform.position.z);
            if((transform.position.x + width/10) <= cameraBoundXNegative)
            {
                RegenerateChunk();
            }
        }
    }

    void SetChunkLevelComponent(Chunk chunk, Vector3 position){
        
        width = chunk.chunkWidth;
        height = chunk.chunkHeight;
        probability = chunk.chunkProbability;
        colorR = chunk.chunkColorRed;
        colorG = chunk.chunkColorGreen;
        colorB = chunk.chunkColorBlue;

        if(previousChunk.gameObjects.Count > 1)
        {
            GameObject corridorTop = previousChunk.gameObjects[1];
            previousChunk.gameObjects.RemoveAt(1);
            DestroyImmediate(corridorTop);
        }

        spriteRenderer.color = new Color(chunk.chunkColorRed, chunk.chunkColorGreen, chunk.chunkColorBlue);
        transform.localScale = new Vector3(width, height, 1);
        // Set the position of this game object, to be next to the previous game object
        float newX = previousChunk.gameObjects[0].transform.position.x + (previousChunk.gameObjects[0].transform.localScale.x/10) + (width/10);
        float newY = height/10;
        transform.position = new Vector3(newX, newY, 0.0f);

        if (chunk.chunkId == 2)
        {
            Debug.Log("Corridor created");
            int chunkNumber = 0;
            foreach (LevelChunk lc in GameObject.Find("GameManager").GetComponent<Main>().levelChunks)
            {
                chunkNumber += lc.gameObjects.Count;
            }
            GameObject chunkGameObject = new GameObject("Chunk" + chunkNumber);

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
            levelComponent.gameStarted = true;
            levelComponent.speedScrolling = speedScrolling;

            SpriteRenderer chunkSpriteRenderer2 = chunkGameObject.AddComponent<SpriteRenderer>();
            chunkSpriteRenderer2.color = new Color(colorR, colorG, colorB);
            chunkSpriteRenderer2.sprite = GetComponent<SpriteRenderer>().sprite;

            BoxCollider2D chunkBoxCollider2D2 = chunkGameObject.AddComponent<BoxCollider2D>();
            chunkBoxCollider2D2.size = new Vector2(0.2f, 0.2f);

            float newY2 = newY + (height / 10) + 0.6f;
            chunkGameObject.transform.position = new Vector3(newX + 0.1f, newY2, 0.0f);
            chunkGameObject.transform.localScale = new Vector3(chunk.chunkWidth - 1, chunk.chunkHeight, 1.0f);
            previousChunk.gameObjects.Add(chunkGameObject);
        }

    }

    private void RegenerateChunk(){
        float newProbability = Random.Range(0.0f, 0.99f);
        float previousProbability = 0;
        for (int i = 0; i < probabilities.Length; i++)
        {
            if (newProbability >= previousProbability && newProbability <= probabilities[i])
            {
                SetChunkLevelComponent(chunksPossibilities[i], previousChunk.gameObjects[0].transform.position);
                return;
            }
            previousProbability += probabilities[i];
        }
    }
}
