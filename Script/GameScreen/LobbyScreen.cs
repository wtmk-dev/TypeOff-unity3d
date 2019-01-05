using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScreen : MonoBehaviour, IGameScreen {

	public DungeonMaster.GameScreen Screen { get; set; }
	public GameObject Clone { get; set; }

	void Awake(){
		Screen = DungeonMaster.GameScreen.LobbyScreen;
		Clone = gameObject;
	}

	public void SetActive( bool isActive ){
		gameObject.SetActive( isActive );
	}
}
