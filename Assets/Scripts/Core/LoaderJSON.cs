using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoadChunk;

[System.Serializable]
public class Chunk {

    public string chunkName;
    public int chunkId;
    public float chunkProbability;

}


public class LoaderJSON : MonoBehaviour {

	// Use this for initialization
	void Start () {

        DontDestroyOnLoad(gameObject);
		LoadGameData("Datas/chunks.json");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void LoadGameData(string gameDataFileName){
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.dataPath, gameDataFileName);

        if(File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath); 
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            Chunk[] chunks = JsonHelper.FromJson<Chunk>(dataAsJson);
            Debug.Log(chunks[0]);


            // Retrieve the allRoundData property of loadedData
            //allRoundData = loadedData.allRoundData;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

}
