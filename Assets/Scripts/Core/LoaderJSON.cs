using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Chunk {

    public string chunkName;
    public int chunkId;
    public float chunkProbability;

}

[System.Serializable]
public class CollectionChunks
{
   public Chunk[] chunks;
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
            Debug.Log(collectionChunks.chunks.Length);
            return collectionChunks.chunks;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
            return null;
        }
    }

}
