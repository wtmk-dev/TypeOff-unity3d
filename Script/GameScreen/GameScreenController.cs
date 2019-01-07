using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenController : MonoBehaviour {

	private DungeonMaster.GameScreen currentScreen;
	private DungeonMaster.GameScreen previousScreen;

	private Dictionary<DungeonMaster.GameScreen, IGameScreen> gameScreens;
	private DungeonMaster controller;
	private BattleScreenController battleScreen;
	private LobbyScreen lobbyScreen;
	private List<string> users;

	void OnEnable(){
		
	}

	void OnDisable(){
		UnsubEvents();
	}
	
	private void SubEvents(){
		if( controller != null ){
			controller.OnGameScreenChanged += ChangeScreen;
		}
	}

	private void UnsubEvents(){
		if( controller != null ){
			controller.OnGameScreenChanged -= ChangeScreen;
		}
	}

	public void Init( DungeonMaster controller ){
		this.controller = controller;
		gameScreens = LoadGameScreens();
		battleScreen = (BattleScreenController) gameScreens[DungeonMaster.GameScreen.BattleScreen ];
		battleScreen.Init( this );

		lobbyScreen = (LobbyScreen) gameScreens[ DungeonMaster.GameScreen.LobbyScreen ];
		lobbyScreen.Init( this );

		currentScreen = DungeonMaster.GameScreen.StartScreen; // safty for previous screen
		SubEvents();
	}

	public void Activate( DungeonMaster.GameScreen currnetScreen ){
		switch( currentScreen ){
			case DungeonMaster.GameScreen.LobbyScreen:
			lobbyScreen.ResetTimer();
			lobbyScreen.StartTimer();
			break;
			case DungeonMaster.GameScreen.BattleScreen:
			battleScreen.UpdateUsers( controller.currentUsersMap );
			battleScreen.StartCountDown();
			break;
		}
	}

	public void UpdateUserQueue( Queue<string> userQueue ){
		lobbyScreen.UpdateQueue( userQueue );
	}

	public void UpdateChamp( string champ, string score ){
		lobbyScreen.UpdateChamp( champ, score );
	}

	public void LobbyTimerComplete(){
		controller.LobbyTimerComplete();
	}

	public void UserLockedIn( string usr ){
		battleScreen.UserLockedIn( usr );
	}

	public void ResetGame(){
		controller.SetChamp();
		controller.ResetGame();
	}

	public void WinnerDecided( string usr, string loser ){
		controller.WinnerDecided( usr, loser );
	}

	private Dictionary<DungeonMaster.GameScreen,IGameScreen> LoadGameScreens(){
		Dictionary<DungeonMaster.GameScreen,IGameScreen> screens = new Dictionary<DungeonMaster.GameScreen,IGameScreen>();
		foreach ( Transform item in transform ) {
			IGameScreen screen = item.gameObject.GetComponent<IGameScreen>();
			screens.Add( screen.Screen, screen );
			screen.Clone.SetActive( false );
		}
		return screens;
	}

	private void ChangeScreen( DungeonMaster.GameScreen screen ){
		previousScreen = currentScreen;
		currentScreen = screen;

		gameScreens[ previousScreen ].Clone.SetActive( false );
		gameScreens[ currentScreen ].Clone.SetActive( true );
	}
}
