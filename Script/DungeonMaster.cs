using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMaster : MonoBehaviour {

	public enum GameScreen { StartScreen, LobbyScreen, BattleScreen }
	private GameScreen gameScreen;
	public static Action<GameScreen> OnGameScreenChanged;
	
	[SerializeField]
	private GameObject goScreenController;
	private GameScreenController gameScreenController;

	void OnEnable(){
		TwitchChatExample.OnUsersGathered += AllUsersGathers;
		TwitchChatExample.OnUserLocked += UserLockedIn;
		DungeonMaster.OnGameScreenChanged += ScreenChanged;
	}

	void OnDisable(){
		TwitchChatExample.OnUsersGathered -= AllUsersGathers;
		TwitchChatExample.OnUserLocked -= UserLockedIn;
		DungeonMaster.OnGameScreenChanged -= ScreenChanged;
	}

	void Awake(){
		goScreenController = Instantiate( goScreenController, transform.position, Quaternion.identity ) as GameObject;
		gameScreenController = goScreenController.GetComponentInChildren<GameScreenController>();
		gameScreenController.Init( this );
	}

	void Start(){
		ChangeGameScreen( GameScreen.LobbyScreen );
	}

	private void UserLockedIn( string usr, string msg ){
		gameScreenController.UserLockedIn( usr, msg );
	}

	private void AllUsersGathers( List<string> users ){
		gameScreenController.SetUsers( users );
		ChangeGameScreen( GameScreen.BattleScreen );
	}

	private void ChangeGameScreen( GameScreen goToScreen ){
		gameScreen = goToScreen;
		if( OnGameScreenChanged != null ){
			OnGameScreenChanged( gameScreen );
		}
	}

	private void ScreenChanged( GameScreen currentScreen ){
		switch( currentScreen ){
			case GameScreen.StartScreen:
			Debug.Log( "Start Screen" );
			break;
			case GameScreen.LobbyScreen:
			Debug.Log( "Lobby Screen" );
			break;
			case GameScreen.BattleScreen:
			Debug.Log( "Battle Screen" );
			gameScreenController.StartCountDown();
			break;
		}
	}

}
