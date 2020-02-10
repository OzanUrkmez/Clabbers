using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public partial class CardSet : NetworkBehaviour, IEnumerable
{
    Stack<Card> Cards = new Stack<Card>();
    public Player OwnerPlayer;
    public bool isLocalSet = false;

    [SyncVar(hook = "OnisReadyChanged")]
    public bool isReady = false;

    public void OnisReadyChanged(bool newReady)
    {
        isReady = newReady;
        if (isReady)
        {
            //Place the number of cards Sprite, so that the numbers dont only show up after a player loses or plays a card.
            if(NumberOfCards >= 10)
            {
                NumberSpriteRenderer.transform.position += new Vector3(-0.5f, 0, 0);
                WasOffset = true;
            }
            NumberSpriteRenderer.sprite = GameProperties.GetNumberSpriteByNumber(NumberOfCards - 1);
        }
    }

    public SpriteRenderer NumberSpriteRenderer;

    [SyncVar]
    public int OwnerPlayerID = -1;

    
    public int NumberOfCards = 0;

    private bool isShaking = false;

    private void Start()
    {
        if (OwnerPlayerID != -1)
        {
            OwnerPlayer = Player.Players[OwnerPlayerID];
            OwnerPlayer.PlayerSet = this;
            gameObject.transform.position = OwnerPlayer.transform.position;
            NumberSpriteRenderer.transform.position = new Vector3(-2, (gameObject.transform.position.y > 0) ? -2 : 2, -6.5f);
        }
    }
    [Server]
    public void AddCard( CardTypes type, float cardreachseconds)
    {
        GameObject g = Instantiate(Card.CardObject, this.transform, true);
        NetworkServer.SpawnWithClientAuthority(g, OwnerPlayer.connectionToClient);
        g.GetComponent<Card>().OwnerPlayerID = OwnerPlayerID;
        g.GetComponent<Card>().PreSetCard(OwnerPlayerID,type,cardreachseconds);
    }
    [ClientRpc]
    public void RpcIncreaseNumberofCards()
    {
        IncrementNumberOfCards();
    }

    public void PushCard(Card c,bool isLastCard, bool GameHasntStarted = false)
    {
        Cards.Push(c);
        if(c.gameObject.transform.position != gameObject.transform.position && !GameHasntStarted)
        {
            c.StartCoroutine(c.moveCard(c.gameObject, new Vector2(OwnerPlayer.transform.position.x, OwnerPlayer.transform.position.y),
                c.CardReachSeconds,false, c.transform.position.z));
            if (isServer)
            {
                AddAuthorityToLatestCard();
                if (isLastCard)
                {
                    //If the last card is being recieved from the middle, it's time to shuffle!
                    GameProperties.MakeCardUnplayable();
                    GameProperties.ResetAssignableZPosition();
                    RpcShuffleCards(c.CardReachSeconds,System.DateTime.Now.Millisecond);
                }
            }
        }
        IncrementNumberOfCards();
    }

    public void ShakeSet()
    {
        if (!isShaking)
        {
            isShaking = true;
            Invoke("ChangeToNotShaking", 1);
            StartCoroutine(ShakeObject(gameObject, 1, 7, 0.3f));
        }
    }

    private void ChangeToNotShaking()
    {
        isShaking = false;
    }


    public static IEnumerator ShakeObject(GameObject g,float seconds,float ShakeTimes,float ShakeX)
    {
        float InitialX = g.transform.position.x;
        float XPerSecond = ShakeX * (ShakeTimes)/ seconds;
        bool MoveToRight = true;
        float deltaCoefficient;
        while(seconds > 0)
        {
            deltaCoefficient = seconds - Time.deltaTime > 0 ? Time.deltaTime : seconds;
            seconds -= Time.deltaTime;
            if (MoveToRight)
            {
                if (InitialX + ShakeX / 2 < g.transform.position.x + XPerSecond * deltaCoefficient)
                {
                    g.transform.position = new Vector3(2*(InitialX + ShakeX/2) -(XPerSecond*deltaCoefficient + g.transform.position.x),g.transform.position.y,g.transform.position.z);
                    MoveToRight = false;
                }
                else
                {
                    g.transform.position += new Vector3(XPerSecond * deltaCoefficient, 0, 0);
                }
            }
            else
            {
                if (InitialX - ShakeX / 2 > g.transform.position.x - XPerSecond * deltaCoefficient)
                {
                    g.transform.position = new Vector3(2 * (InitialX - ShakeX / 2) - (g.transform.position.x - XPerSecond * deltaCoefficient), g.transform.position.y, g.transform.position.z);
                    MoveToRight = true;
                }
                else
                {
                    g.transform.position -= new Vector3(XPerSecond * deltaCoefficient, 0, 0);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        g.transform.position = new Vector3(InitialX, g.transform.position.y, g.transform.position.z);
    }
    
    [ClientRpc]
    public void RpcShuffleCards(float WaitBefore,int seed)
    {
        UnityEngine.Random.InitState(seed);
        StartCoroutine(ShuffleCards(WaitBefore));
        Debug.Log("Shuffling the set of player " + OwnerPlayerID);
    }
    IEnumerator ShuffleCards(float WaitBefore)
    {
        yield return new WaitForSeconds(WaitBefore);
        Card[] CardsArray = new Card[Cards.Count];
        int CardsCount = Cards.Count;
        CardsArray = Cards.ToArray();
        Cards.Clear();
        List<int> Used = new List<int>();
        while (Cards.Count != CardsCount) {
            //TODO PLAY ANIMATION IN HERE!
            int i;
            do
            {
                i = UnityEngine.Random.Range(0, CardsCount);
            } while (Used.Contains(i));
            Used.Add(i);
            Cards.Push(CardsArray[i]);
        }
        if (isServer)
            GameProperties.MoveToPlayerBefore();
    }

    public Card PeekCard()
    {
        return Cards.Peek();
    }

    public void SetPlayer(int id)
    {
        CmdSetPlayer(id);
    }

    public void MoveTopCard(Vector2 pos)
    {
        Cards.Peek().gameObject.transform.position = new Vector3(pos.x, pos.y, -7);
        CmdMoveTopCard(pos);
    }
    [Command]
    public void CmdMoveTopCard(Vector2 pos)
    {
        RpcMoveTopCard(pos);
    }

    [ClientRpc]
    public void RpcMoveTopCard(Vector2 pos)
    {
        if(!isLocalSet)
        Cards.Peek().gameObject.transform.position = new Vector3(pos.x, pos.y, -7);
    }

    [Command]
    public void CmdSetPlayer(int id)
    {
        gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(Server.Players[id].connectionToClient);
        RpcSetPlayer(id);
    }
    [ClientRpc]
    public void RpcSetPlayer(int id)
    {
        OwnerPlayer = Player.Players[id];
        OwnerPlayer.PlayerSet = this;
        OwnerPlayerID = OwnerPlayer.PlayerID;
        if (OwnerPlayer.isLocalPlayer)
        {
            isLocalSet = true;
            OwnerPlayer.PlayerSetDone();
        }
        else
        {
            isLocalSet = false;
        }
    }

    [Server]
    public bool AddAuthorityToLatestCard()
    {
        Cards.Peek().gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(Server.Players[Cards.Peek().AuthorityPlayerID].connectionToClient);
        Cards.Peek().AuthorityPlayerID = OwnerPlayerID;
        return Cards.Peek().gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(OwnerPlayer.connectionToClient);
    }

    public Card DrawLocalCard()
    {
            MiddleSet.AddLatestCardFromPlayerCardSet(OwnerPlayerID);
            Cards.Peek().DrawCard();
            CmdPopAllButLocalCard();
            DecrementNumberOfCards();
            return Cards.Pop();
    }

    [Command]
    public void CmdPopAllButLocalCard()
    {
        RpcPopAllButLocalCard();
    }

    [ClientRpc]
    public void RpcPopAllButLocalCard()
    {
        if (!OwnerPlayer.isLocalPlayer)
        {
            MiddleSet.AddLatestCardFromPlayerCardSet(OwnerPlayerID);
            Cards.Pop();
            DecrementNumberOfCards();
        }
    }

    [Server]
    public void ClaimMiddleCards()
    {
        RpcClaimMiddleCards();
    }
    [Server]
    public void ClaimMiddleCardsInASecond()
    {
        GameProperties.MakeCardUnplayable();
        Invoke("ClaimMiddleCards", 0.5f);
    }
    
    [ClientRpc]
    private void RpcClaimMiddleCards()
    {
        MiddleSet.TransferAllToSet(OwnerPlayerID);
    }
    
    public IEnumerator GetEnumerator()
    {
        foreach(Card c in Cards)
        {
            yield return c;
        }
    }

    //For Setting The Pos of the Card Set
    public void SetSetPositionAccordingToOwner()
    {
        gameObject.transform.position = OwnerPlayer.transform.position;
        NumberSpriteRenderer.transform.localPosition = new Vector3(-2, (gameObject.transform.position.y > 0) ? -2 : 2, -6.5f);
    }

    //The dealing with NumberOfCards both held inside the class and outisde on the scene

    private bool WasOffset = false;

    public void IncrementNumberOfCards()
    {
        //The animations and the sprites should only be played if the cardset is ready, as otherwise there would be too much in the screen at once.
        NumberOfCards++;
        if(NumberOfCards == GameProperties.GetCardCount())
        {
            //TODO THE PLAYER HAS WON, give him credits etc. and announce his victory.
        }
        if (isReady)
        {
            NumberSpriteRenderer.sprite = GameProperties.GetNumberSpriteByNumber(NumberOfCards - 1);
            StartCoroutine(TemporaryInflateShrink(NumberSpriteRenderer.gameObject, false, 0.04f,0.5f,true));
            if(!WasOffset && NumberOfCards >= 10)
            {
                WasOffset = true;
                NumberSpriteRenderer.gameObject.transform.position += new Vector3(-0.5f, 0, 0);
                Debug.Log("Offsetting Numbers!");
            }
        }
    }
    public void DecrementNumberOfCards()
    {
        NumberOfCards--;
        //The animations and the sprites should only be played if the cardset is ready, as otherwise there would be too much in the screen at once. Remember that a player may still rejoin if he can
        //take the cards from the middle when two corresponding cards are played one after another.
        if (NumberOfCards == 0)
        {
            //TODO PLAYER HAS LOST, Specify procedure and functions to deal with the loss of the player.
        }
        if (isReady)
        {
            NumberSpriteRenderer.sprite = GameProperties.GetNumberSpriteByNumber(NumberOfCards - 1);
            StartCoroutine(TemporaryInflateShrink(NumberSpriteRenderer.gameObject, true, 0.04f, 0.5f, true));
            if (WasOffset && NumberOfCards < 10)
            {
                WasOffset = false;
                NumberSpriteRenderer.gameObject.transform.position += new Vector3(0.5f,0, 0);
                Debug.Log("Setting Numbers back to their initial -2.0 positions");
            }
        }
    }

    //For playing the largening and shrinking animation on the NumberOfCards Sprite, can also be used for other things

    private static Dictionary<GameObject, float> ComboObjectsToComboWaitTime = new Dictionary<GameObject, float>();


    /// <summary>
    /// Inflates or Shrinks the Object and then slowly changes it back to normal
    /// </summary>
    /// <param name="g"> The GameObject </param>
    /// <param name="Shrink"> Whether The object should be shrinked or not, false means inflation </param>
    /// <param name="ChangeCoefficient"> The Total change in scalse desired, must always be positive </param>
    /// <param name="time"> The time for recovery to occur in seconds </param>
    /// <param name="PartOfCombo"> States if the gameobject is being inflated multiple times within the same time frame to avoid multiple quirky inflation and allow for
    /// build up. </param>
    /// <returns></returns>
    public static IEnumerator TemporaryInflateShrink(GameObject g, bool Shrink, float ChangeCoefficient,float time,bool PartOfCombo)
    {
        Vector3 InitialScale = g.transform.localScale;

        //These 2 floating numbers are only relevant when dealing with combos.
        float InitialTime = time;
        float TimeTakenOff = 0;
        //Setting the scale to inflate or shrink according to the boolean sent first.
        g.transform.localScale = new Vector3(Shrink? g.transform.localScale.x - ChangeCoefficient : g.transform.localScale.x + ChangeCoefficient , 
            Shrink ? g.transform.localScale.y - ChangeCoefficient : g.transform.localScale.y + ChangeCoefficient, 0);

        //Dealing With Combos to create timing.
        float WaitBefore = 0;
        if (PartOfCombo)
        {
            if (ComboObjectsToComboWaitTime.ContainsKey(g))
            {
                WaitBefore = ComboObjectsToComboWaitTime[g];
                ComboObjectsToComboWaitTime[g] += time;
            }
            else
            {
                ComboObjectsToComboWaitTime.Add(g, time);
            }
        }
        //Setting FadePerSecond
        Vector3 FadeInSecond = new Vector3(Shrink ? ChangeCoefficient / time : -ChangeCoefficient / time,
               Shrink ? ChangeCoefficient / time : -ChangeCoefficient / time, 0);

        //Waiting before the fade effect if in combo.
        while(WaitBefore > 0)
        {
            WaitBefore -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //The fading of the shrink or inflate effect
        while (time > 0)
        {
            time -= Time.deltaTime;
            if(time >= 0)
            {
                //Not Last Frame
                g.transform.localScale += FadeInSecond * Time.deltaTime;
                if (PartOfCombo)
                {
                    TimeTakenOff += Time.deltaTime;
                    ComboObjectsToComboWaitTime[g] -= Time.deltaTime;
                }   
                yield return new WaitForEndOfFrame();
            }
            else
            {
                //Last Frame, setting it back to the initial so that there are no errors.
                if (!PartOfCombo) {
                    g.transform.localScale = InitialScale;
                }
                else 
                {
                    TimeTakenOff += time + Time.deltaTime;
                    ComboObjectsToComboWaitTime[g] -= Time.deltaTime + time;
                    g.transform.localScale += FadeInSecond * (time + Time.deltaTime);
                }

            }
           
        }
        if (PartOfCombo)
        {
            ComboObjectsToComboWaitTime[g] -= (InitialTime - TimeTakenOff);
            if (ComboObjectsToComboWaitTime[g] <= 0) ComboObjectsToComboWaitTime.Remove(g);
        }
    }
}

