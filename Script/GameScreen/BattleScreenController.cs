using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleScreenController : MonoBehaviour, IGameScreen {

	[SerializeField]
	private TextMeshProUGUI lUsername, rUsername, lScore, rScore, 
	countDownText, battleText;
	private float countDownDelay, timeStamp;
	private int userCount;
	private string battleLeft = "left", battleRight = "right";
	private GameScreenController controller;
	
	public DungeonMaster.GameScreen Screen { get; set; }
	public GameObject Clone { get; set; }
	private IEnumerator delayReportWin;
	private IEnumerator countDownClock;
	private Dictionary<string,double> userTimeMap;
	private Dictionary<string,string> currentUsers;
	private List<double> times;

	void Awake(){
		Screen = DungeonMaster.GameScreen.BattleScreen;
		Clone = gameObject;
	}

	public void Init( GameScreenController controller ){
		this.controller = controller;
		userTimeMap = new Dictionary<string,double>();
		currentUsers = new Dictionary<string,string>();
		times = new List<double>();
		countDownDelay = 1f;
		userCount = 0;
	}

	public void SetActive( bool isActive ){
		Debug.Log( "bsc set active" );
	}

	public void UpdateUsers( Dictionary<string,string> users ){
		times = new List<double>();
		userTimeMap = new Dictionary<string, double>();
		currentUsers = new Dictionary<string, string>();

		currentUsers.Add( users[ battleLeft ], battleLeft );
		currentUsers.Add( users[ battleRight ], battleRight );
		userCount = currentUsers.Count;
	
		UpdateTMPUIText( battleText, "" );
		UpdateTMPUIText( rScore, "" );
		UpdateTMPUIText( lScore, "" );

		UpdateTMPUIText( lUsername, users[ battleLeft ] );
		UpdateTMPUIText( rUsername, users[ battleRight ] );
	}

	public void StartCountDown(){
		if( countDownClock != null ) {
			UpdateTMPUIText( countDownText, "" );
			StopCoroutine( countDownClock );
		}

		countDownClock = CountDownClockRoutine();
		StartCoroutine( countDownClock );
	}

	public void UserLockedIn( string usr ){
		if( countDownClock != null ){
			StopCoroutine( countDownClock );
		}
		
		string side = currentUsers[usr];
		float time  = Time.timeSinceLevelLoad - timeStamp;
		
		if( side == battleLeft ){
			double lTime = (double) time;
			UpdateTMPUIText( lScore, lTime.ToString() );
			userTimeMap.Add( usr, lTime );
			times.Add( lTime );
		} else {
			double rTime = (double) time;
			UpdateTMPUIText( rScore, rTime.ToString() );
			userTimeMap.Add( usr, rTime );
			times.Add( rTime );
		}

		if( userTimeMap.Count > 0 ){
			string winner = "";
			string loser = "";
			times.Sort();
			foreach( var item in userTimeMap ){
				if( item.Value == times[0] ){
					winner = item.Key;
				}else{
					loser = item.Key;
				}
			}

			if( delayReportWin != null ){
				StopCoroutine( delayReportWin );
			}

			delayReportWin = ReportWinRoutine( winner, loser );
			StartCoroutine( delayReportWin );
		}
	}

	private void UpdateTMPUIText( TextMeshProUGUI text, string rap ){
		text.text = rap;
	}

	private IEnumerator CountDownClockRoutine(){
		yield return new WaitForSeconds( countDownDelay );
		int count = 3;
		
		do {
			UpdateTMPUIText( countDownText, count.ToString() );
			yield return new WaitForSeconds( countDownDelay );
			count--;
		} while ( count > 0 );

		timeStamp = Time.timeSinceLevelLoad;
		UpdateTMPUIText( countDownText, "TYPE!" );
		UpdateTMPUIText( battleText, "its a game" );
		yield return new WaitForSeconds( 30f );
		controller.ResetGame();
	}

	private IEnumerator ReportWinRoutine( string winner, string loser ){
		UpdateTMPUIText( countDownText, "( *-* " + winner + " *-* )" );
		yield return new WaitForSeconds( countDownDelay + 2f );
		controller.WinnerDecided( winner, loser );
	}
}
