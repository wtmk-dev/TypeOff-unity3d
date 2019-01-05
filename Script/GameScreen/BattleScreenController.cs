using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleScreenController : MonoBehaviour, IGameScreen {

	[SerializeField]
	private GameObject goLeft, goRight, goCountDown;
	[SerializeField]
	private float countDownDelay, timeStamp;
	private GameScreenController controller;
	private List<TextMeshProUGUI> leftTexts, rightTexts, countDownTexts;
	public DungeonMaster.GameScreen Screen { get; set; }
	public GameObject Clone { get; set; }

	private IEnumerator countDownClock;
	private Dictionary<string,TextMeshProUGUI> userMaps;
	private List<string> users;

	public void SetActive ( bool isActive ){
		goLeft.SetActive( isActive );
		goRight.SetActive( isActive );
		goCountDown.SetActive( isActive );
	}

	void Awake(){
		Screen = DungeonMaster.GameScreen.BattleScreen;
		Clone = gameObject;
	}

	public void Init( GameScreenController controller ){
		this.controller = controller;
		leftTexts = LoadText( goLeft );
		rightTexts = LoadText( goRight );
		countDownTexts = LoadText( goCountDown );
		UpdateTMPUIText( countDownTexts[ 1 ], "" );
		userMaps = new Dictionary<string,TextMeshProUGUI>();
		users = new List<string>();
	}

	public void UserLockedIn( string user, string msg ){
		if( msg == countDownTexts[ 1 ].text ){
			float timeElapsed = Time.timeSinceLevelLoad;
			timeElapsed -= timeStamp;
			UpdateTMPUIText( userMaps[ user ], timeElapsed.ToString() );
		}

		int count = 0;
		foreach( string usr in users ){
			if( userMaps[ usr].text != "" ){
				count++;
			}
		}

		if( count == 2 ){
			controller.BattleComplete();
		}
	}

	public void UpdateUsers( List<string> users ){
		this.users = users;
		UpdateTMPUIText( leftTexts[0], users[0] );
		userMaps.Add( users[0], leftTexts[1] );
		UpdateTMPUIText( rightTexts[0], users[1] );
		userMaps.Add( users[1], rightTexts[1] );
	}

	public void StartCountDown(){
		if( countDownClock != null ) {
			UpdateTMPUIText( countDownTexts[ 1 ], "" );
			StopCoroutine( countDownClock );
		}

		countDownClock = CountDownClockRoutine();
		StartCoroutine( countDownClock );
	}

	private List<TextMeshProUGUI> LoadText( GameObject go ){
		List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
		foreach( Transform clone in go.transform ){
			texts.Add( clone.gameObject.GetComponent<TextMeshProUGUI>() );
		}
		return texts;
	}

	private void UpdateTMPUIText( TextMeshProUGUI text, string rap ){
		text.text = rap;
	}

	private IEnumerator CountDownClockRoutine(){
		int count = 3;
		do{
			UpdateTMPUIText( countDownTexts[ 0 ], count.ToString() );
			yield return new WaitForSeconds( countDownDelay );
			count--;
		}while( count > 0 );

		timeStamp = Time.timeSinceLevelLoad;
		UpdateTMPUIText( countDownTexts[ 1 ], "lets dule" );
	}
}
