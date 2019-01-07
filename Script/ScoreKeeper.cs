using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScoreKeeper : MonoBehaviour {

	public static string path = "Assets/Resources/Assets/score.txt";

	public static void WriteScore( string username, int score ){		
		using( StreamWriter writer = new StreamWriter( path ) ){
			writer.WriteLine( username + " : " + score.ToString() );
		}
	}
}
