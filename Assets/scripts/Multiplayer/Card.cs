using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Card : NetworkBehaviour{

    public SpriteRenderer CardSpriteRenderer;
    public static GameObject CardObject;

    [SyncVar]
    public bool OutOfHand = false;

    [SyncVar]
    public int AuthorityPlayerID;

    public CardSet OwnerSet;
    public CardTypes Type;
    public float CardReachSeconds;

    public bool isInMiddle = false;

    [SyncVar]
    public int OwnerPlayerID;

    public void SetCard(CardTypes type, float cardreachseconds)
    {
        Type = type;
        gameObject.name = type.ToString();
        CardReachSeconds = cardreachseconds;
        gameObject.transform.SetParent(OwnerSet.transform);
    }

    [Server]
    public void PreSetCard(int id,CardTypes type, float cardreachseconds)
    {
        AuthorityPlayerID = id;
        RpcPreSetCard(id,type,cardreachseconds);
    }

    [ClientRpc]
    public void RpcPreSetCard(int id, CardTypes type, float cardreachseconds)
    {
        OwnerSet = Player.Players[id].PlayerSet;
        SetCard(type, cardreachseconds);
        if (isServer)
        {
            RpcServeCard();
        }
        OwnerSet.PushCard(this,false,true);

    }

   
    [ClientRpc]
    public void RpcServeCard()
    {
        StartCoroutine(moveCard(gameObject, OwnerSet.OwnerPlayer.gameObject.transform.position, CardReachSeconds));
    }
    public void DrawCard()
    {
        CmdDrawCard();
    }
    [Command]
    public void CmdDrawCard()
    {
        RpcDrawCard(GameProperties.FetchAssignableCardZPosition());
    }


    [ClientRpc]
    public void RpcDrawCard(float z)
    {
        Debug.Log("Drawing Card...");
        OutOfHand = true;
        StartCoroutine(moveCard(gameObject, new Vector2(0, 0), 0.3f, true,z));
    }

    public IEnumerator moveCard(GameObject g, Vector2 pos, float seconds, bool PlayedCard = false, float finalZ = 0)
    {
        if (isServer && PlayedCard) Server.CommenceCheckToSeeIfPlayerCanPlayOnceAgain(Type);
        Vector2 GoPerFrame = (pos - new Vector2(g.transform.position.x, g.transform.position.y)) /  (seconds * 60);
        int FramesLeft = (int)(seconds * 60);
        g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, finalZ);
        while (FramesLeft >= 0)
        {
            FramesLeft--;
            g.transform.position += new Vector3(GoPerFrame.x,GoPerFrame.y,0);
            yield return new WaitForFixedUpdate();
        }
        g.transform.position = new Vector3(pos.x, pos.y, finalZ);
        if (PlayedCard)
        {
            CardSpriteRenderer.sprite = GameProperties.GetTypeSprite(Type);
            isInMiddle = true;
            OutOfHand = false;
            if (OwnerSet.OwnerPlayer.isLocalPlayer)
            {
                CmdPlayedCard(OwnerSet.OwnerPlayerID, Type);
            }
        }
    }
    //Sending Message to the server that a card by the Player has been played, the server shall take care of the rest according to the data.
    [Command]
    private void CmdPlayedCard(int PlayerID, CardTypes type)
    {
        Server.PlayedCard(PlayerID, type,true);
    }

    //INFORMATIVE FUNCTIONS THAT HAVE TO DO WITH CARDS RATHER THAN A SINGLE CARD

    /// <returns>
    /// Returns -1 if the card type is not one of the royalty,
    /// Returns the number of Cards that the type demands to be put on the table if the card is royalty.
    /// </returns>
    public static int isRoyalCard(CardTypes Type)
    {
        if (Type == CardTypes.p1 ||
            Type == CardTypes.c1 ||
            Type == CardTypes.l1 ||
            Type == CardTypes.f1) return 4;


        if (Type == CardTypes.pk ||
            Type == CardTypes.ck ||
            Type == CardTypes.lk ||
            Type == CardTypes.fk) return 3;

        if (Type == CardTypes.pq ||
            Type == CardTypes.cq ||
            Type == CardTypes.lq ||
            Type == CardTypes.fq) return 2;

        if (Type == CardTypes.pj ||
            Type == CardTypes.cj ||
            Type == CardTypes.lj ||
            Type == CardTypes.fj) return 1;

        return -1;
    }

    //Yes, I know this is messy as hell but I WANT to get this game done already it's 4PM and neither Tuğkan or Ferhat has come.
    public static bool isSameType(CardTypes t1, CardTypes t2)
    {
        if ((t1 == CardTypes.p0 || t1 == CardTypes.l0 || t1 == CardTypes.f0 || t1 == CardTypes.c0) && (t2 == CardTypes.p0 || t2 == CardTypes.l0 || t2 == CardTypes.f0 || t2 == CardTypes.c0)) return true;
        if ((t1 == CardTypes.p1 || t1 == CardTypes.l1 || t1 == CardTypes.f1 || t1 == CardTypes.c1) && (t2 == CardTypes.p1 || t2 == CardTypes.l1 || t2 == CardTypes.f1 || t2 == CardTypes.c1)) return true;
        if ((t1 == CardTypes.p2 || t1 == CardTypes.l2 || t1 == CardTypes.f2 || t1 == CardTypes.c2) && (t2 == CardTypes.p2 || t2 == CardTypes.l2 || t2 == CardTypes.f2 || t2 == CardTypes.c2)) return true;
        if ((t1 == CardTypes.p3 || t1 == CardTypes.l3 || t1 == CardTypes.f3 || t1 == CardTypes.c3) && (t2 == CardTypes.p3 || t2 == CardTypes.l3 || t2 == CardTypes.f3 || t2 == CardTypes.c3)) return true;
        if ((t1 == CardTypes.p4 || t1 == CardTypes.l4 || t1 == CardTypes.f4 || t1 == CardTypes.c4) && (t2 == CardTypes.p4 || t2 == CardTypes.l4 || t2 == CardTypes.f4 || t2 == CardTypes.c4)) return true;
        if ((t1 == CardTypes.p5 || t1 == CardTypes.l5 || t1 == CardTypes.f5 || t1 == CardTypes.c5) && (t2 == CardTypes.p5 || t2 == CardTypes.l5 || t2 == CardTypes.f5 || t2 == CardTypes.c5)) return true;
        if ((t1 == CardTypes.p6 || t1 == CardTypes.l6 || t1 == CardTypes.f6 || t1 == CardTypes.c6) && (t2 == CardTypes.p6 || t2 == CardTypes.l6 || t2 == CardTypes.f6 || t2 == CardTypes.c6)) return true;
        if ((t1 == CardTypes.p7 || t1 == CardTypes.l7 || t1 == CardTypes.f7 || t1 == CardTypes.c7) && (t2 == CardTypes.p7 || t2 == CardTypes.l7 || t2 == CardTypes.f7 || t2 == CardTypes.c7)) return true;
        if ((t1 == CardTypes.p8 || t1 == CardTypes.l8 || t1 == CardTypes.f8 || t1 == CardTypes.c8) && (t2 == CardTypes.p8 || t2 == CardTypes.l8 || t2 == CardTypes.f8 || t2 == CardTypes.c8)) return true;
        if ((t1 == CardTypes.p9 || t1 == CardTypes.l9 || t1 == CardTypes.f9 || t1 == CardTypes.c9) && (t2 == CardTypes.p9 || t2 == CardTypes.l9 || t2 == CardTypes.f9 || t2 == CardTypes.c9)) return true;
        if ((t1 == CardTypes.pj || t1 == CardTypes.lj || t1 == CardTypes.fj || t1 == CardTypes.cj) && (t2 == CardTypes.pj || t2 == CardTypes.lj || t2 == CardTypes.fj || t2 == CardTypes.cj)) return true;
        if ((t1 == CardTypes.pq || t1 == CardTypes.lq || t1 == CardTypes.fq || t1 == CardTypes.cq) && (t2 == CardTypes.pq || t2 == CardTypes.lq || t2 == CardTypes.fq || t2 == CardTypes.cq)) return true;
        if ((t1 == CardTypes.pk || t1 == CardTypes.lk || t1 == CardTypes.fk || t1 == CardTypes.ck) && (t2 == CardTypes.pk || t2 == CardTypes.lk || t2 == CardTypes.fk || t2 == CardTypes.ck)) return true;

        return false;
    }
}

public enum CardTypes
{
    p1, p2, p3, p4, p5, p6, p7, p8, p9, p0, pj, pq, pk,
    l1, l2, l3, l4, l5, l6, l7, l8, l9, l0, lj, lq, lk,
    f1, f2, f3, f4, f5, f6, f7, f8, f9, f0, fj, fq, fk,
    c1, c2, c3, c4, c5, c6, c7, c8, c9, c0, cj, cq, ck
}
