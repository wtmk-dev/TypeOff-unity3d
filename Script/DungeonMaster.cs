using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonMaster : MonoBehaviour {

	public enum GameScreen { StartScreen, LobbyScreen, BattleScreen }
	private GameScreen gameScreen;
	public Action<GameScreen> OnGameScreenChanged;
	private int score;
	private string battleLeft = "left", battleRight = "right", 
	battleText = "its a game", champ;
	[SerializeField]
	private GameObject goScreenController;
	private GameScreenController gameScreenController;
	private TwitchChatExample twitchChatBot;
	private Queue<string> userQueue;
	public Dictionary<string,string> currentUsersMap;
	public Dictionary<string,string> twitchUsersMap;

	void OnEnable(){
		
	}

	void OnDisable(){
		UnsubEvents();
	}

	void Awake(){
		champ = "";
		score = 0;
		currentUsersMap = new Dictionary<string, string>();
		twitchUsersMap = new Dictionary<string, string>();
		userQueue = new Queue<string>();
		goScreenController = Instantiate( goScreenController, transform.position, Quaternion.identity ) as GameObject;
		gameScreenController = goScreenController.GetComponentInChildren<GameScreenController>();
		gameScreenController.Init( this );

		twitchChatBot = GetComponent<TwitchChatExample>();
		SubEvents();
	}

	private void SubEvents(){
		if( twitchChatBot != null ){
			twitchChatBot.OnScreenChanged += ChangeGameScreen;
			twitchChatBot.OnUserGathered += UserSaved;
			twitchChatBot.OnUserLocked += UserLockedIn;
		}

		this.OnGameScreenChanged += ScreenChanged;
	}

	private void UnsubEvents(){
		if( twitchChatBot != null ){
			twitchChatBot.OnScreenChanged -= ChangeGameScreen;
			twitchChatBot.OnUserGathered -= UserSaved;
			twitchChatBot.OnUserLocked -= UserLockedIn;
		}

		this.OnGameScreenChanged -= ScreenChanged;
	}

	void Start(){
		ChangeGameScreen( GameScreen.StartScreen );
	}

	private void UserSaved( string user ){
		if( !IsAlreadyQueued( user ) ){
			userQueue.Enqueue( user );
			gameScreenController.UpdateUserQueue( userQueue );
		}
	}

	private void UserLockedIn( string usr, string msg ){
		if( msg == battleText ){
			gameScreenController.UserLockedIn( usr );
		}
	}

	private bool IsAlreadyQueued( string user ){
		bool isQueued = false;

		foreach( var usr in userQueue ){
			if( usr == user ){
				isQueued = true;
				break;
			}
		}

		return isQueued;
	}

	private void ChangeGameScreen( GameScreen goToScreen ){
		gameScreen = goToScreen;
		if( OnGameScreenChanged != null ){
			OnGameScreenChanged( gameScreen );
		}
	}

	private void ScreenChanged( GameScreen currentScreen ){
		Debug.Log( currentScreen );
		gameScreenController.Activate( currentScreen );
	}

	public void LobbyTimerComplete(){
		if( userQueue.Count < 2 ){
			ChangeGameScreen( GameScreen.LobbyScreen );
		} else {
			string p1 = userQueue.Dequeue();
			string p2 = userQueue.Dequeue();

			currentUsersMap = new Dictionary<string, string>();
			currentUsersMap.Add( battleLeft, p1 );
			currentUsersMap.Add( battleRight, p2 );

			twitchUsersMap = new Dictionary<string, string>();
			twitchUsersMap.Add( p1, p1 );
			twitchUsersMap.Add( p2, p2 );

			ChangeGameScreen( GameScreen.BattleScreen );
		}
	}

	public void WinnerDecided( string winner, string loser ){
		if( winner == champ ){
			score++;
		} else {
			champ = winner;
			score = 1;
		}

		gameScreenController.UpdateChamp( champ, score.ToString() );

		var count = userQueue.Count;
		Queue<string> tempQueue = new Queue<string>();
		for( int i = 0; i < count; i++ ){
			string check = userQueue.Dequeue();
			if( check != loser ){
				tempQueue.Enqueue( check );
			}	
		}

		userQueue.Enqueue( winner );
		
		count = tempQueue.Count;
		for( int i = 0; i < count; i++ ){
			userQueue.Enqueue( tempQueue.Dequeue() );
		}

		gameScreenController.UpdateUserQueue( userQueue );
		ChangeGameScreen( DungeonMaster.GameScreen.LobbyScreen );
	}

	public void ResetGame(){
		SceneManager.LoadScene( "SampleScene" );
	}

	public void SetChamp(){
		if( champ != "" ){
			ScoreKeeper.WriteScore( champ, score );
		}
	}
}
