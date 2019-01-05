using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameScreen { 

	GameObject Clone { get; set; }
	DungeonMaster.GameScreen Screen { get; set; }
	void SetActive( bool isActive );

}
