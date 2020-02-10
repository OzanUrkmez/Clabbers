using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class serverControlOfCard : NetworkBehaviour {

	public List<string> cards, randomizedCards, person1Cards, person2Cards, person3Cards, person4Cards;
	public string typeOfCard, cardNumbers;
	public int peopleCount;
    public bool[] ready;
    void Start()
    {
        ready = new bool[peopleCount];     
    }
    void FixedUpdate()
    {
          // this is for remind ozan yes
    }
    [Server]
	public void ServerCardRandomise (){
		// randomisin the cards	
		for (int i = 0;i < 4; i++){
			for(int a = 0;a < 13;a++){
				string newCard = "" + typeOfCard [i] + cardNumbers[a];
				cards.Add (newCard);
				Debug.Log (newCard);
			}
		}
		for (int i = 0; i < cards.Count;)
		{
			int random = Random.Range (0, cards.Count);
			randomizedCards.Add (cards [random]);
			cards.RemoveAt(random);
		}
	}

	[Server]
	public void ServerCardGive (){
			
	}
}
