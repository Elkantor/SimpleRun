using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	void Start () {
		DontDestroyOnLoad(gameObject);
		InitializeMap();
	}
	
	private void InitializeMap(){
		LoaderJSON loaderChunks = new LoaderJSON();
		Chunk[] chunks = loaderChunks.LoadGameData("Datas/chunks.json");
        foreach(Chunk c in chunks){

        }
	}

	void Update () {
		
	}
}
