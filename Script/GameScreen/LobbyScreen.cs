using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyScreen : MonoBehaviour, IGameScreen {

	public DungeonMaster.GameScreen Screen { get; set; }
	public GameObject Clone { get; set; }
	private GameScreenController controller;
	private float lobbyTime = 30f;
	[SerializeField]
	private TextMeshProUGUI timerText, queueText, champText;
	private IEnumerator lobbyTimer;

	void Awake(){
		Screen = DungeonMaster.GameScreen.LobbyScreen;
		Clone = gameObject;
	}

	public void Init( GameScreenController controller ){
		this.controller = controller;
		lobbyTime = 30f;
		queueText.text = "Battle Queue!" + "\n" + "--------------" + "\n";
		champText.text = "";
	}

	public void SetActive( bool isActive ){
		gameObject.SetActive( isActive );
	}

	public void UpdateQueue( Queue<string> userQueue ){
		string userText = "Battle Queue!" + "\n" + "--------------" + "\n";

		foreach( var user in userQueue ){
			userText += user + "\n";
		}

		queueText.text = userText;
	}

	public void ResetTimer(){
		lobbyTime = 30f;
	}

	public void StartTimer(){
		if( lobbyTimer != null ){
			StopCoroutine( lobbyTimer );
		}

		lobbyTimer = LobbyTimerRoutine();
		StartCoroutine( lobbyTimer );
	}

	public void UpdateChamp( string champ, string score ){
		champText.text = "Champ: " + champ + " : " + score + " wins";
	}

	private IEnumerator LobbyTimerRoutine(){
		do{
			lobbyTime -= Time.deltaTime;
			int sec = (int) lobbyTime;
			timerText.text = "Battle starting in: " + sec;
			yield return null;
		}while( lobbyTime > 0 );

		controller.LobbyTimerComplete();
	}
}
