using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenController : MonoBehaviour {

	private DungeonMaster.GameScreen currentScreen;
	private DungeonMaster.GameScreen previousScreen;

	private Dictionary<DungeonMaster.GameScreen, IGameScreen> gameScreens;
	private DungeonMaster controller;
	private BattleScreenController battleScreen;
	private List<string> users;

	void OnEnable(){
		DungeonMaster.OnGameScreenChanged += ChangeScreen;
	}

	void OnDisable(){
		DungeonMaster.OnGameScreenChanged -= ChangeScreen;
	}

	void Start(){
		
	}

	public void Init( DungeonMaster controller ){
		this.controller = controller;
		gameScreens = LoadGameScreens();
		battleScreen = (BattleScreenController) gameScreens[DungeonMaster.GameScreen.BattleScreen ];
		battleScreen.Init( this );
		Debug.Log( gameScreens );
		currentScreen = DungeonMaster.GameScreen.StartScreen;
	}

	public void StartCountDown(){
		battleScreen.StartCountDown();
	}

	public void BattleComplete(){
		ChangeScreen( DungeonMaster.GameScreen.StartScreen );
	}

	public void UserLockedIn( string user, string msg ){
		battleScreen.UserLockedIn( user, msg );
	}

	public void SetUsers( List<string> users ){
		this.users = users;
		battleScreen.UpdateUsers( users ); 
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

		if( previousScreen != DungeonMaster.GameScreen.StartScreen ){
			gameScreens[ previousScreen ].Clone.SetActive( false );
		}

		gameScreens[ currentScreen ].Clone.SetActive( true );

	}
}
