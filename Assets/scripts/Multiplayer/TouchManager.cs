using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TouchManager : MonoBehaviour, IDragHandler,IPointerDownHandler,IPointerUpHandler {
enum PointerType { none,onOwnCard,onMiddleCard,onElseCard}
    private static PointerType CurrentPointerType;

    private static RaycastHit hit;

    private static Card SelectedCard;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(CurrentPointerType == PointerType.none) { 
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.transform.gameObject.GetComponent<Card>() != null)
                {
                    //PLAYER HAS HIT A CARD
                    if (hit.transform.gameObject.GetComponent<Card>().isInMiddle)
                    {
                        //PLAYER HAS HIT A CARD THATS IN THE MIDDLE
                        CurrentPointerType = PointerType.onMiddleCard;
                        return;
                    }
                    else if (hit.transform.gameObject.GetComponent<Card>().OwnerSet.isLocalSet && !hit.transform.gameObject.GetComponent<Card>().OutOfHand)
                    {
                        //PLAYER HAS HIT HIS OWN CARD
                        if (Server.LocalPlayer.PlayerID == GameProperties.GetCurrentPlayerID())
                        {
                            //IT IS RIGHT NOW THE PLAYERS TURN.
                            Server.LocalPlayer.ActivateOwnCardIfItsPlayerTurn();
                        }
                        else
                        {
                            //IT IS NOT THE PLAYERS TURN M8, WARN HIM!!
                            Player.Players[GameProperties.GetCurrentPlayerID()].ShakePlayer();
                            Debug.Log("It is Player " + GameProperties.GetCurrentPlayerID() + "'s turn right now!");
                        }
                        return;
                    }
                    else
                    {
                        //PLAYER HAS HIT SOMEBODY ELSE'S CARD!
                        CurrentPointerType = PointerType.onElseCard;
                        return;
                    }
                }
                else
                {
                    //PLAYER HAS HIT SOMETHING ELSE

                    //TODO this none setter is bound to change when you add other clickable colliders.
                    CurrentPointerType = PointerType.none;
                }
            }
        }
    }
    public static void ActivatePlayerOwnCardHit()
    {
        CurrentPointerType = PointerType.onOwnCard;
        SelectedCard = hit.transform.gameObject.GetComponent<Card>();
        SelectedCard.transform.position = new Vector3(SelectedCard.transform.position.x, SelectedCard.transform.position.y, -7);
        Debug.Log("Player " + GameProperties.GetCurrentPlayerID() + " is playing his card!");
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (CurrentPointerType)
        {
            case PointerType.none:

                break;
            case PointerType.onOwnCard:
                //Pointer is on our card and is being dragged.
                SelectedCard.OwnerSet.MoveTopCard(Camera.main.ScreenToWorldPoint(eventData.position));
                break;
            case PointerType.onMiddleCard:
                //Pointer is on the middle card and is being dragged.

                break;
            case PointerType.onElseCard:
                //Pointer is on somebody else's card and is being dragged.

                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Throw that focking card m8
        switch (CurrentPointerType)
        {
            case PointerType.none:

                break;
            case PointerType.onOwnCard:
                //Pointer was on our card
                SelectedCard.OwnerSet.DrawLocalCard();
                break;
            case PointerType.onMiddleCard:
                //Pointer was on the middle card
                Server.LocalPlayer.MiddleClick();
                break;
            case PointerType.onElseCard:
                //Pointer was on somebody else's card.

                break;
        }

        CurrentPointerType = PointerType.none;
    }

    
}