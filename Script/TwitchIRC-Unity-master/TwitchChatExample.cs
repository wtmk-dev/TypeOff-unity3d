using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TwitchIRC))]
public class TwitchChatExample : MonoBehaviour {

    public int maxMessages = 100; //we start deleting UI elements when the count is larger than this var.
    private LinkedList<GameObject> messages =
        new LinkedList<GameObject>();
    private TwitchIRC IRC;
    //when message is recieved from IRC-server or our own message.

    //move to diffrent class
    private int maxUsers = 10;
    private string joinCMD = "*jq", startCMD = "*ab", lowdownCmd = "*ld";
    private Dictionary<string,string> currentUsersMap;
    private DungeonMaster.GameScreen currentScreen;
    private DungeonMaster controller;

    public delegate void ScreenChanged( DungeonMaster.GameScreen gameScreen );
    public event ScreenChanged OnScreenChanged;
    public delegate void UserGathered( string user );
    public event UserGathered OnUserGathered;
    public delegate void UserLocked( string user, string msg );
    public event UserLocked OnUserLocked;

    public void OnChatMsgRecieved( string msg ){
        //parse from buffer.
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);

        //remove old messages for performance reasons.
        if ( messages.Count > maxMessages ) {
            Destroy( messages.First.Value );
            messages.RemoveFirst();
        }

        ChatMsgRecievedAction( msgString, user );
    }

    void OnEnable(){
        
    }

    void OnDisable(){
        if( controller != null ){
            controller.OnGameScreenChanged -= UpdateScreen;
        }
    }
    
    void Awake(){
        currentUsersMap = new Dictionary<string,string>();

        controller = GetComponent<DungeonMaster>();
        if( controller != null ){
            controller.OnGameScreenChanged += UpdateScreen;
        }
    }

    void Start(){
        IRC = this.GetComponent<TwitchIRC>();
        //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
        IRC.messageRecievedEvent.AddListener( OnChatMsgRecieved );
    }

    private void SaveUser( string user ){
        if( maxUsers == 0 ){
            return;
        }

        Debug.Log( user );
        maxUsers--;

        if( OnUserGathered != null ){
            OnUserGathered( user );
        }
    }

    private void ChatMsgRecievedAction( string msgString, string user ){
        switch( currentScreen ){
            case DungeonMaster.GameScreen.StartScreen:
            if( msgString == startCMD ){
                ChangeScreen( DungeonMaster.GameScreen.LobbyScreen );
            }
            break;
            case DungeonMaster.GameScreen.LobbyScreen:
            if( msgString == joinCMD ){
                SaveUser( user );
            }
            break;
            case DungeonMaster.GameScreen.BattleScreen:
            string check = controller.twitchUsersMap[ user ];
            if( check != null && check != "" ){
                if( OnUserLocked != null ){
                    OnUserLocked( user, msgString );
                }
            }
            break;
        }
    }

    private void UpdateScreen( DungeonMaster.GameScreen screen ){
        currentScreen = screen;
    }

    private void ChangeScreen( DungeonMaster.GameScreen screen ){
        currentScreen = screen;

        if( OnScreenChanged != null ){
            OnScreenChanged( currentScreen );
        }
    }

    public Color ColorFromUsername( string username ) {
        Random.seed = username.Length + (int)username[0] + (int)username[username.Length - 1];
        return new Color(Random.Range(0.25f, 0.55f), Random.Range(0.20f, 0.55f), Random.Range(0.25f, 0.55f));
    }

}
