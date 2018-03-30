using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Chunk {

    public string chunkName;
    public int chunkId;
    public float chunkProbability;
    public float chunkHeight;
    public float chunkWidth;
    public float chunkColorRed;
    public float chunkColorBlue;
    public float chunkColorGreen;

}

[System.Serializable]
public class CollectionChunks
{
   public Chunk[] chunks;
}

[System.Serializable]
public class Scores
{
    public float bestScore;
}

public class LoaderJSON {

	public Chunk[] LoadGameData(string gameDataFileName){
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.dataPath, gameDataFileName);

        if(File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath); 
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            CollectionChunks collectionChunks = JsonUtility.FromJson<CollectionChunks>(dataAsJson);
            return collectionChunks.chunks;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
            return null;
        }
    }

    public float LoadBestScore(string scoresFileName)
    {
        string filePath = Path.Combine(Application.dataPath, scoresFileName);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            Scores scoresData = JsonUtility.FromJson<Scores>(dataAsJson);
            return scoresData.bestScore;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
            return 0.0f;
        }
    }

    public void SaveBestScore(float bestScore, string scoresFileName)
    {
        string filePath = Path.Combine(Application.dataPath, scoresFileName);

        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
        // Read the json from the file into a string
        File.WriteAllText(filePath, "{\"bestScore\" : " + bestScore + "}");
    }
}
