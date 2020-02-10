using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameProperties : NetworkBehaviour {

    [SyncVar]
    private bool is2PlayerGame;

    [SyncVar]
    private int CurrentPlayerNumberID = -1;
    [SyncVar]
    private int CurrentPlayerID = -1;
    [SyncVar]
    private bool CanPlayCard = true;

    [SyncVar]
    private int NumberofCardsOnTable = 0;

    [SyncVar]
    private int CardCount;

    [SyncVar]
    private float MiddleClickCooldown = 3;

    [SyncVar]
    private bool SameTypeInMiddle = false;

    [SyncVar]
    private float SecondsBeforeMiddleObsolete = 3;

    private static GameProperties propertiesObject;

    public Dictionary<CardTypes, Sprite> TypeSprites = new Dictionary<CardTypes, Sprite>(52);

    public CardTypes[] TypesInDic = new CardTypes[52];

    public Sprite[] SpritesinDic = new Sprite[52];

    public GameObject Cursor;

    public Sprite[] NumberSprites = new Sprite[52];

    public float CursorOffset = 0.5f;

    [SyncVar]
    private float z = 0;

    private void Start()
    {
        propertiesObject = this;
        if (isServer)
        {
            is2PlayerGame = Server.isGame2Player();
            CardCount = Server.CardCount;
        }

        for(int i = 0; i < SpritesinDic.Length; i++)
        {
            TypeSprites.Add(TypesInDic[i], SpritesinDic[i]);
        }

    }

    public static bool isGame2Player()
    {
        return propertiesObject.is2PlayerGame;
    }

    public static Sprite GetTypeSprite(CardTypes t)
    {
        return propertiesObject.TypeSprites[t];
    }

    public static int GetCurrentPlayerNumberID()
    {
        return propertiesObject.CurrentPlayerNumberID;
    }

    public static int GetNumberOfCardsOnTable()
    {
        return propertiesObject.NumberofCardsOnTable;
    } 

    public static int GetCurrentPlayerID()
    {
        return propertiesObject.CurrentPlayerID;
    }

    public static float GetMiddleClickCooldown()
    {
        return propertiesObject.MiddleClickCooldown;
    }

    public static bool IsThereSameTypesInMiddle()
    {
        return propertiesObject.SameTypeInMiddle;
    }

    [Server]
    public static void ActivateSameTypesInMiddle(CardTypes type, int PlayerID)
    {
        propertiesObject.SameTypeInMiddle = true;
        propertiesObject.StartCoroutine(propertiesObject.StartMiddleObsoleteTimer(propertiesObject.SecondsBeforeMiddleObsolete,type,PlayerID));
    }

    IEnumerator StartMiddleObsoleteTimer(float seconds, CardTypes type, int PlayerID) {
        float InitialSeconds = seconds;
        bool Shaked = false;
        while(seconds > 0)
        {
            seconds -= Time.deltaTime;
            if(seconds <= InitialSeconds / 1.5f)
            {
                if (!Shaked)
                {
                    MiddleSet.ShakeMiddleSet(InitialSeconds / 1.5f);
                    Shaked = true;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        if (propertiesObject.SameTypeInMiddle)
        {
            //The players failed to click the middle within time, continuing the game.
            Debug.Log("The player has failed to middle click, playing the card again. FOR FOCKS SAKE GAME PLEASE DONT BREAK FOR FOCKS SAKE");
            Server.PlayedCard(PlayerID, type, false);
        }
    }

    public static Sprite GetNumberSpriteByNumber(int index)
    {
        if(index > propertiesObject.NumberSprites.Length - 1)
        Debug.Log(index + " Card is out of range!" );
        return propertiesObject.NumberSprites[index];
    }

    [Server]
    public static void NotSameTypesInMiddle()
    {
        propertiesObject.SameTypeInMiddle = false;
    }

    [Server]
    public static void ChangeMiddleClickCooldown(float newCooldown)
    {
        propertiesObject.MiddleClickCooldown = newCooldown;
    }
    [Server]
    public static void CardAddedToTheTable()
    {
        propertiesObject.NumberofCardsOnTable++;
    }

    [Server]
    public static void ResetNumberOfCardsOnTheTable()
    {
        propertiesObject.NumberofCardsOnTable = 0;
    }

    [Server]
    public static void ChangeGameTo2Player()
    {
        propertiesObject.is2PlayerGame = true;
    }

    [Server]
    public static void MakeCardUnplayable()
    {
        propertiesObject.CanPlayCard = false;
    }
    [Server]
    public static void MakeCardPlayable()
    {
        propertiesObject.CanPlayCard = true;
    }

    public static bool CardCanBePlayed()
    {
        return propertiesObject.CanPlayCard;
    }

    [Server]
    public static void ChangeGameTo4Player()
    {
        propertiesObject.is2PlayerGame = false;
    }

    [Server]
    public static void MoveToNextPlayer()
    {
        if (isGame2Player())
        {
            if(propertiesObject.CurrentPlayerNumberID == 1)
            {
                propertiesObject.CurrentPlayerNumberID++;
            }
            else
            {
                propertiesObject.CurrentPlayerNumberID = 1;
            }
        }
        else
        {
            if(propertiesObject.CurrentPlayerNumberID == 4)
            {
                propertiesObject.CurrentPlayerNumberID = 1;
            }
            else
            {
                propertiesObject.CurrentPlayerNumberID = (propertiesObject.CurrentPlayerNumberID == -1) ? 1 : propertiesObject.CurrentPlayerNumberID + 1;
            }
        }
        MakeCardPlayable();
        propertiesObject.CurrentPlayerID = Server.NumbertoPlayer[propertiesObject.CurrentPlayerNumberID].PlayerID;
        MoveCursorToPlayer(propertiesObject.CurrentPlayerNumberID);
    }

    [Server]
    public static void MoveToPlayerBefore()
    {

       

        if(propertiesObject.CurrentPlayerNumberID == 1)
        {
            if (propertiesObject.is2PlayerGame)
            {
                propertiesObject.CurrentPlayerNumberID = 2;
            }
            else
            {
                propertiesObject.CurrentPlayerNumberID = 4;
            }
        }
        else
        {
            propertiesObject.CurrentPlayerNumberID--;
        }
        MakeCardPlayable();
        propertiesObject.CurrentPlayerID = Server.NumbertoPlayer[propertiesObject.CurrentPlayerNumberID].PlayerID;
        MoveCursorToPlayer(propertiesObject.CurrentPlayerNumberID);
    }

    [Server]
    public static void MoveCursorToPlayer(int NumberID)
    {
        propertiesObject.RpcMoveCurstorToPlayer(Server.NumbertoPlayer[NumberID].PlayerID);
    }

    [Server]
    public static void ChangeSecondsBeforeMiddleObsolete(float newSeconds)
    {
        propertiesObject.SecondsBeforeMiddleObsolete = newSeconds;
    }

    [ClientRpc]
    public void RpcMoveCurstorToPlayer(int ID)
    {
        Cursor.gameObject.transform.position = Player.Players[ID].transform.position + 
            ((Player.Players[ID].transform.position.y > 0)? new Vector3(0, -CursorOffset, 0) : new Vector3(0, +CursorOffset, -6));
        Cursor.gameObject.transform.rotation = Quaternion.Euler(0,0,
            ((Player.Players[ID].transform.position.y > 0) ? 180: 0));

    }

    public static float FetchAssignableCardZPosition()
    {
        propertiesObject.CmdChangeZ(propertiesObject.z - 0.1f);
        return propertiesObject.z;
    }

    public static void ResetAssignableZPosition()
    {
        propertiesObject.CmdChangeZ(0);
    }

    public static int GetCardCount()
    {
        return propertiesObject.CardCount;
    }

    [Command]
    private void CmdChangeZ(float x)
    {
        z = x;
    }
}
