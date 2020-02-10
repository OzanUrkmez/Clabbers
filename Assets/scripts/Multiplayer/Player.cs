using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    //Properties
   
    private static byte Ready = 0;
    [SyncVar(hook = "OnPlayerIDChanged")]
    public int PlayerID = -1;
    public CardSet PlayerSet;
    //0 is always the main starting position, while 1 is the one in front of it.
    public static Vector3[] StartingPositions;
    public Vector3[] startingPositions = new Vector3[4];
    public Vector3[] startingPositions2 = new Vector3[2];
    public GameObject CardSetObject;
    [SyncVar]
    public bool CanMiddleClick = true;
    [SyncVar]
    public bool ReadiedUp;

    public LoadingCircle LocalLoadingCircle;

    public static Dictionary<int, Player> Players =  new Dictionary<int, Player>();

    public static bool StartingPositionsSet = false;

    public GameObject StandardCard;
    //Unity Functions
    private void Awake()
    {
        
        Card.CardObject = StandardCard;
    }
    void Start()
    {
        if (PlayerID != -1)
        {
            Players.Add(PlayerID, this);
        }
      
        if (!StartingPositionsSet)
        {
            if (!GameProperties.isGame2Player())
            {
                StartingPositions = new Vector3[4];
                for (int j = 0; j < startingPositions.Length; j++)
                {
                    StartingPositions[j] = startingPositions[j];
                }
                StartingPositionsSet = true;
            }
            else
            {
                StartingPositions = new Vector3[2];
                for (int j = 0; j < startingPositions2.Length; j++)
                {
                    StartingPositions[j] = startingPositions2[j];
                }
                StartingPositionsSet = true;
            }
        }
        if (isLocalPlayer)
        {
            LocalLoadingCircle = GameObject.Find("LoadingCircle").GetComponent<LoadingCircle>();
            Server.LocalPlayer = this;
            CmdReadyUp();
        }
    }

    public void ShakePlayer()
    {
        PlayerSet.ShakeSet();
    }

    public void MiddleClick()
    {
        if (CanMiddleClick)
        {
            StartCoroutine(LocalLoadingCircle.InitiateReload(GameProperties.GetMiddleClickCooldown()));
            CanMiddleClick = false;
            StartCoroutine(InitiateCooldown());
            CmdTryClaimMiddle();
            Debug.Log("Tried to Claim Middle");
        }
        else
        {
            //TODO Let the player know that he cannot middle click as his cooldown is still in effect.

        }
    }
    [Command]
    public void CmdTryClaimMiddle()
    {
        if (GameProperties.IsThereSameTypesInMiddle())
        {
            //TODO play middle click animation.
            GameProperties.NotSameTypesInMiddle();
            GameProperties.ResetNumberOfCardsOnTheTable();
            Server.CardsBeforeMove = 1;
            Server.OnRoyalRound = false;
            Server.CurrentRoyalRoundTimes = 1;
            PlayerSet.ClaimMiddleCards();
        }
        else
        {
            //Someone else got the cards... Warn them perhaps?
        }
    }

    IEnumerator InitiateCooldown()
    {
        float Seconds = GameProperties.GetMiddleClickCooldown();
        while(Seconds >= 0)
        {
            Seconds -= Time.deltaTime;
            //TODO Deal with middle click wait animation right here m8
            yield return new WaitForEndOfFrame();
        }
        CanMiddleClick = true;
    }

    public void ActivateOwnCardIfItsPlayerTurn()
    {
        CmdActivateOwnCardIfItsPlayerTurn(PlayerID);
    }

    //Network Functions
    [Command]
    public void CmdActivateOwnCardIfItsPlayerTurn(int id)
    {
        TargetActivateOwnCardIfItsPlayersTurn(connectionToClient,(id == GameProperties.GetCurrentPlayerID() && GameProperties.CardCanBePlayed()));
    }

    [TargetRpc]
    public void TargetActivateOwnCardIfItsPlayersTurn(NetworkConnection target,bool activate)
    {
        if (activate) TouchManager.ActivatePlayerOwnCardHit();
        else Debug.Log("It's not the player's turn no more!");
    }

    [Command]
    public void CmdReadyUp()
    {
        Ready++;
        ReadiedUp = true;
        Server.RegisterPlayer(connectionToClient.connectionId, this);
        PlayerID = connectionToClient.connectionId;
    }
    [Command]
    public void CmdSpawnSet()
    {
            GameObject g = Instantiate(CardSetObject);
            NetworkServer.Spawn(g);
            g.GetComponent<CardSet>().SetPlayer(PlayerID);
    }
    public void PlayerSetDone()
    {
        CmdCheckStart();
    }
    [Command] 
    public void CmdCheckStart()
    {
        if (Server.isGame2Player())
        {
            if (Ready >= 2)
            {
                RpcSetPlayerLocations();
                Server.StartGame();
            }
        }
        else
        {
            if (Ready >= 4)
            {
                RpcSetPlayerLocations();
                Server.StartGame();
            }
        }
    }
    [ClientRpc]
    public void RpcSetPlayerLocations()
    {
        int i = 0;
        foreach(Player p in Players.Values)
        {
            p.gameObject.transform.position = StartingPositions[i];
            p.PlayerSet.SetSetPositionAccordingToOwner();
            i++;
        }
    }
    [Server]
    public static void UnreadyPlayer()
    {
        Ready--;
    }
    //Events
    public void OnPlayerIDChanged(int id)
    {
        Debug.Log("Player id is now " + id);
        PlayerID = id;
        Players.Add(id, this);
        if (isLocalPlayer) CmdSpawnSet();
    }
    //IEnumeration


    //Private Classes
}
