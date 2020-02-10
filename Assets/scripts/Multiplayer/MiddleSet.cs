using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MiddleSet : NetworkBehaviour {

    public static Stack<Card> Cards = new Stack<Card>();

    public static MiddleSet DefaultMiddleSet;

    public static GameObject StandardMiddleCardObject;

    public static Sprite StandardCardSprite;

    public Sprite standardcardsprite;

    public static int NumberOfCards;

    //Unity Functions
    private void Start()
    {
        StandardMiddleCardObject = GameObject.Find("MiddleSet");
        StandardCardSprite = standardcardsprite;
    }

    //--------------------------------------------------------------------------------------------------------------------------------
    //Adding and Managing of Cards

    /// <summary>
    /// Adds the ID's set's uppermost card to the stack of the middle set.
    /// </summary>
    public static void AddLatestCardFromPlayerCardSet(int PlayerID)
    {
        Cards.Push(Player.Players[PlayerID].PlayerSet.PeekCard());
        Cards.Peek().gameObject.transform.SetParent(StandardMiddleCardObject.transform);
        Cards.Peek().transform.position = new Vector3(Cards.Peek().transform.position.x
            , Cards.Peek().transform.position.y, 0);
        IncreaseNumberofCards();
    }

    /// <summary>
    /// Transfers all of the cards in the middle to the stack of the chosen Player ID. This is done locally
    /// so it is preferable to put this in a ClientRPC. No judging though, just a suggestion!
    /// </summary>
    public static void TransferAllToSet(int OwnerPlayerID)
    {
        Debug.Log("Time for one card to trave will be: " + 0.6f / Mathf.Log(Cards.Count));
        StandardMiddleCardObject.GetComponent<MiddleSet>().StartCoroutine(IEnumerateTransferAllToSet(OwnerPlayerID, 0.6f/ Mathf.Log(Cards.Count)));
    }

    //The process should be
    private static IEnumerator IEnumerateTransferAllToSet(int OwnerPlayerID,float TimeBetween)
    { 
        while (Cards.Count != 0)
        {
            float time = TimeBetween;
            while(time > 0)
            {
                time -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            Cards.Peek().isInMiddle = false;
            Cards.Peek().CardSpriteRenderer.sprite = StandardCardSprite;
            Cards.Peek().gameObject.transform.SetParent(Player.Players[OwnerPlayerID].PlayerSet.transform);
            Cards.Peek().OwnerSet = Player.Players[OwnerPlayerID].PlayerSet;
            Cards.Peek().OwnerPlayerID = OwnerPlayerID;
            if (Cards.Count != 1)
            {
                Player.Players[OwnerPlayerID].PlayerSet.PushCard(Cards.Pop(), false);
            }
            else
            {
                //If the card count is one, we are at the last card so sending a message to shuffle cards would make sense.
                Player.Players[OwnerPlayerID].PlayerSet.PushCard(Cards.Pop(), true);
            }
            DecreaseNumberofCards();
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------
    //The managing of the variable Number of Cards, this is done so that the sprite may be changed each time the number is.

    private static void DecreaseNumberofCards()
    {
        NumberOfCards--;
        //TODO add animation and stuff.
    }

    private static void IncreaseNumberofCards()
    {
        NumberOfCards++;
        //TODO Add Animation and Stuff.
    }

    [Server]
    public static void ShakeMiddleSet(float seconds)
    {
        StandardMiddleCardObject.GetComponent<MiddleSet>().RpcShakeMiddleSet(seconds);
    }
    [ClientRpc]
    public void RpcShakeMiddleSet(float seconds)
    {
        StartCoroutine(CardSet.ShakeObject(StandardMiddleCardObject, seconds, 10, 0.3f));
    }
}
