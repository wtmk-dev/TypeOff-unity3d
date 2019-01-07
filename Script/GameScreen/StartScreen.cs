using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour, IGameScreen {

	public GameObject Clone { get; set; }
	public DungeonMaster.GameScreen Screen { get; set; }

	void Awake(){
		Screen = DungeonMaster.GameScreen.StartScreen;
		Clone = gameObject;
	}

	public void SetActive( bool isActive ){
		gameObject.SetActive( isActive );
	}
}
