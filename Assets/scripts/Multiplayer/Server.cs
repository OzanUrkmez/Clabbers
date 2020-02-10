using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : NetworkBehaviour
{
    //Relevant inside all
    public static Player LocalPlayer;
    //Irrelevant outside server.
    private static bool is2PlayerGame = true; 
    public static Dictionary<int,Player> Players = new Dictionary<int,Player>();
    public static Dictionary<int, int> IDtoNumber = new Dictionary<int, int>();
    public static Dictionary<int, Player> NumbertoPlayer = new Dictionary<int, Player>();
    private static int LastAvailableNumber = 0;
    public static int CardCount = 52;
    public static float SecondsToServe = 3;
    public static Server server;
    public Sprite ClosedCardSprite;
    public static float CardReachSeconds = 1;
    public static CardTypes LastPlayedType;
    public static int CardsBeforeMove = 1;
    public static bool OnRoyalRound = false;
    //Unity Functions
    private void Start()
    {
        if (isServer)
            server = this;
        else
            Destroy(gameObject);
    }
    //Functions
    
    [Server]
    public static void StartGame()
    {
        Debug.Log("Starting game..");
        server.FormAndServeCards();
        
    }
    [Server]
    public static void ChangeSecondsToServe(float seconds)
    {
        SecondsToServe = seconds;
    }
    [Server]
    public static void ChangeCardCount(int newCount)
    {
        CardCount = newCount;
    }
    [Server]
     public static void ChangeCardReachSeconds(float seconds)
    {
        CardReachSeconds = seconds;
    }
    [Server]
    public static void ChangeTo2Player()
    {
        is2PlayerGame = true;
        GameProperties.ChangeGameTo2Player();
    }
    [Server]
    public static void ChangeTo4Player()
    {
        is2PlayerGame = false;
        GameProperties.ChangeGameTo4Player();
    }
    [Server]
    public static bool isGame2Player()
    {
        return is2PlayerGame;
    }

    [Server]
    public static int RegisterPlayer(int id,Player p)
    {
        Debug.Log("Player " + id + " has joined!");
        Players.Add(id, p);
        LastAvailableNumber++;
        IDtoNumber.Add(id, LastAvailableNumber);
        NumbertoPlayer.Add(LastAvailableNumber, p);
        return id;
    }

    [Server]
    public static void UnRegisterPlayer(int id)
    {
        Debug.Log("Player " + id + " has disconnected!");
        NumbertoPlayer.Remove(IDtoNumber[id]);
        IDtoNumber.Remove(id);
        Players.Remove(id);
    }

    [Server]
    private void FormAndServeCards()
    {
        //Randomization
        CardTypes[] Cards = (CardTypes[])Enum.GetValues(typeof(CardTypes));
        CardTypes[] Randomized = new CardTypes[52];
        List<int> Used = new List<int>();
        for (int i = 0; i < CardCount; i++)
        {
            int index = UnityEngine.Random.Range(0, 52);
            while (Used.Contains(index))
            {
                index = UnityEngine.Random.Range(0, 52);
            }
            Used.Add(index);
            Randomized[i] = Cards[index];
        }


        //Serving
        StartCoroutine(server.ServeCards(Randomized));
    }


    [Server]
    public static void PlayedCard(int PlayerID, CardTypes type, bool isNewCard)
    {
        CurrentRoyalRoundTimes = CardsBeforeMove;
        if (PlayerID != GameProperties.GetCurrentPlayerID())
        {
            throw new Exception("A player whose turn hadn't come yet was able to play!");
        }
        if(isNewCard)
            GameProperties.CardAddedToTheTable();
        if(Card.isSameType(type,LastPlayedType) && GameProperties.GetNumberOfCardsOnTable() > 1 && isNewCard)
        {
            //THE SAME KIND OF CARD HAS BEEN PLAYED
            //TODO Implement reaction to the same kind of card being played. Don't forget to reset number of cards on table afterwards
            Debug.Log("SAME TYPE SAME TYPE SAME TYPEEEEEEEEEEEEEEEEEE");
            GameProperties.ActivateSameTypesInMiddle(type,PlayerID);
            return;
        }
        if (Card.isRoyalCard(type) == -1)
        {
            //A NON ROYAL CARD HAS BEEN PLAYED!
            if (!OnRoyalRound)
            {
                //NOT ON ROYAL ROUND, MOVE TO NEXT PLAYER
                Debug.Log("Normal Stuff m8");
                GameProperties.MoveToNextPlayer();
            }
            else
            {
                //IS ON ROYAL ROUND, IS BEING CHALLENGED BY OTHER PLAYER'S ROYAL CARD!!1
                CardsBeforeMove--;
                CurrentRoyalRoundTimes--;
                if(CardsBeforeMove == 0)
                {
                    //COULDNT PLAY ROYAL CARD DESPITE THE OTHER PLAYER'S ROYAL CHALLENGE, REWARD THE OTHER PLAYER THE CARDS.
                    Debug.Log("Sheet man he or she or it or helicopter has failed to draw a royal card!");
                    OnRoyalRound = false;
                    CardsBeforeMove = 1;
                    LastPlayedType = type;
                    GameProperties.ResetNumberOfCardsOnTheTable();
                    GameProperties.ResetAssignableZPosition();
                    Player.Players[(GameProperties.GetCurrentPlayerNumberID() == 1) ? ((GameProperties.isGame2Player()) ? NumbertoPlayer[2].PlayerID : NumbertoPlayer[4].PlayerID) :
                        NumbertoPlayer[GameProperties.GetCurrentPlayerNumberID() - 1].PlayerID].PlayerSet.ClaimMiddleCardsInASecond();
                    //We will move to the player before after the shuffle.
                    CurrentRoyalRoundTimes = 1;
                    return;
                }
                Debug.Log("Still Can win!");
                return;  //OUR CURRENT PLAYER STILL HAS CHANCES TO PLAY MORE IN ORDER TO WIN THE CHALLENGE.
            }
        }
        else
        {
            //A ROYAL CARD HAS BEEN PLAYED
            Debug.Log("Ow shit son a royal is among us!");
            OnRoyalRound = true;
            CardsBeforeMove = Card.isRoyalCard(type);
            GameProperties.MoveToNextPlayer();
            CurrentRoyalRoundTimes = CardsBeforeMove;
        }
        LastPlayedType = type;
    }

    public static int CurrentRoyalRoundTimes;
    [Server]
    public static void CommenceCheckToSeeIfPlayerCanPlayOnceAgain(CardTypes type)
    {
        CurrentRoyalRoundTimes--;
        if (Card.isRoyalCard(type) != -1) {
            GameProperties.MakeCardUnplayable();
            return;
        }
        if (!OnRoyalRound)
        {
            GameProperties.MakeCardUnplayable();
        }
        else
        {
            if(CurrentRoyalRoundTimes == 0)
            {
                GameProperties.MakeCardUnplayable();
            }
            else
            {
                GameProperties.MakeCardPlayable();
            }
        }
    }

    //Events1

    //IEnumeration
    [Server]
    public IEnumerator ServeCards(CardTypes[] Randomized)
    {
        for (int i = 0; i < CardCount -1;)
        {
            for (int person = 1; person <= (is2PlayerGame ? 2 : 4); person++)
            {
                NumbertoPlayer[person].PlayerSet.AddCard(Randomized[i],CardReachSeconds);
                i++;
                yield return new WaitForSeconds(SecondsToServe / CardCount);
            }
        }
        yield return new WaitForSeconds(1.5f);
        foreach (Player p in Players.Values) p.PlayerSet.isReady = true;
        GameProperties.MoveToNextPlayer();
    }
}
