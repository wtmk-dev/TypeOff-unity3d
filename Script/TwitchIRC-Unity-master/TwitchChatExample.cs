using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TwitchIRC))]
public class TwitchChatExample : MonoBehaviour
{
    public int maxMessages = 100; //we start deleting UI elements when the count is larger than this var.
    private LinkedList<GameObject> messages =
        new LinkedList<GameObject>();
    public UnityEngine.UI.InputField inputField;
    public UnityEngine.UI.Button submitButton;
    public UnityEngine.RectTransform chatBox;
    public UnityEngine.UI.ScrollRect scrollRect;
    private TwitchIRC IRC;
    //when message is recieved from IRC-server or our own message.
    void OnChatMsgRecieved(string msg)
    {
        //parse from buffer.
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);

        //remove old messages for performance reasons.
        if (messages.Count > maxMessages)
        {
            Destroy(messages.First.Value);
            messages.RemoveFirst();
        }

        switch( currentScreen ){
            case DungeonMaster.GameScreen.LobbyScreen:
            if( msgString == joinCMD ){
                SaveUser( user );
            }
            break;
            case DungeonMaster.GameScreen.StartScreen:
            string check = currentUsersMap[ user ];
            if( check != null && check != "" ){
                if( OnUserLocked != null ){
                    OnUserLocked( user, msgString );
                }
            }
            break;
        }
    }
    void CreateUIMessage(string userName, string msgString)
    {
        Color32 c = ColorFromUsername(userName);
        string nameColor = "#" + c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");
        GameObject go = new GameObject("twitchMsg");
        var text = go.AddComponent<UnityEngine.UI.Text>();
        var layout = go.AddComponent<UnityEngine.UI.LayoutElement>();
        go.transform.SetParent(chatBox);
        messages.AddLast(go);

        layout.minHeight = 20f;
        text.text = "<color=" + nameColor + "><b>" + userName + "</b></color>" + ": " + msgString;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        scrollRect.velocity = new Vector2(0, 1000f);
    }
    //when Submit button is clicked or ENTER is pressed.
    public void OnSubmit()
    {
        if (inputField.text.Length > 0)
        {
            IRC.SendMsg(inputField.text); //send message.
            CreateUIMessage(IRC.nickName, inputField.text); //create ui element.
            inputField.text = "";
        }
    }
    Color ColorFromUsername(string username)
    {
        Random.seed = username.Length + (int)username[0] + (int)username[username.Length - 1];
        return new Color(Random.Range(0.25f, 0.55f), Random.Range(0.20f, 0.55f), Random.Range(0.25f, 0.55f));
    }
    // Use this for initialization
    void Start()
    {
        IRC = this.GetComponent<TwitchIRC>();
        //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
        IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
    }

    void OnEnable(){
        DungeonMaster.OnGameScreenChanged += UpdateCurrentScreen;
    }

    void OnDisable(){
        DungeonMaster.OnGameScreenChanged -= UpdateCurrentScreen;
    }
    
    void Awake(){
        currentUsers = new List<string>();
        currentUsersMap = new Dictionary<string,string>();
    }

//move to diffrent class
    private int maxUsers = 2;
    private string joinCMD = "*join";
    private List<string> currentUsers;
    private Dictionary<string,string> currentUsersMap;
    private DungeonMaster.GameScreen currentScreen;
    public delegate void UsersGathered( List<string> users );
    public static event UsersGathered OnUsersGathered;
    public delegate void UserLocked( string user, string msg );
    public static event UserLocked OnUserLocked;

    private void SaveUser( string user ){
        if( maxUsers == 0 ){
            return;
        }

        Debug.Log( user );

        currentUsers.Add( user );
        currentUsersMap.Add( user, user );
        maxUsers--;

        if( OnUsersGathered != null && maxUsers == 0 ){
            OnUsersGathered( currentUsers );
        }
    }

    private void UpdateCurrentScreen( DungeonMaster.GameScreen screen ){
        currentScreen = screen;
    }

}
