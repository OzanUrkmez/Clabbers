using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offlineGameController : MonoBehaviour {

    public static int peopleCount = 4;
    public GameObject cardBackPrefab;
    public GameObject[] twoSpawn, fourSpawn;
    public List<string> cards, randomizedCards;
    public List<string> p1c, p2c, p3c, p4c;
    public string typeOfCard, cardNumbers;
    public string lastCard;
    // alttaki int karakterin kaç tane kart atması gerektiğine bakıcak ve ona g,re kartlari toplayacak
    public int nextPersonShouldGoAndIfEnd;
    bool CardOutGo;

    public GameObject per1, per2, person3, person4;
    public GameObject[] cardP, cardL, cardF, cardC;
    public int round;
    public GameObject[] playerTouch, arrowsFour, arrowsTwo;
    public GameObject fourPAll, twoPAll;

    public List<GameObject> cardf1, cardf2, cardf3, cardf4;
    public int[] personPoints;
    public int DeckCount;
    public GameObject buttonsOfPoeple, middleDeckCOntainer;
    

    bool checkSwith;
    int which;
    bool cardCollectStarted;

    // kart atılırken hangi kartın atılıcağını seçiyo *bitmedi*
    public void cardSpawn(int pers)
    {
        int whicCard = 0;
        string man = "00";
        // kimin kartının atılcağını seçiyo
        switch (pers)
        {
            case 0:
                if (p1c == null)
                {
                    round = 1;
                    break;
                }
                bool goAgain;
                man = p1c[0];
                Debug.LogError(man[1]);
                if (lastCard == "")
                {
                    lastCard = man;
                    if (man[1] == 'j')
                    {
                        nextPersonShouldGoAndIfEnd = 1;
                    }
                    else if (man[1] == 'q')
                    {
                        nextPersonShouldGoAndIfEnd = 2;  
                    }
                    else if (man[1] == 'k')
                    {
                        nextPersonShouldGoAndIfEnd = 3;
                    }
                    else if (man[1] == '1')
                    {
                        nextPersonShouldGoAndIfEnd = 4;
                    }
                    else
                    {
                    }
                    round = 1;

                }else
                {
                    if (man[1] == lastCard[1]) { buttonsOfPoeple.SetActive(true); } else { buttonsOfPoeple.SetActive(false); }
                    if (man[1] == 'j')
                    {
                        nextPersonShouldGoAndIfEnd = 1;
                        cardCollectStarted = true;
                        round = 1;
                    }
                    else if (man[1] == 'q')
                    {
                        nextPersonShouldGoAndIfEnd = 2;
                        cardCollectStarted = true;
                    }
                    else if (man[1] == 'k')
                    {
                        nextPersonShouldGoAndIfEnd = 3;
                        cardCollectStarted = true;
                    }
                    else if (man[1] == '1')
                    {
                        nextPersonShouldGoAndIfEnd = 4;
                        cardCollectStarted = true;
                    }
                    else
                    {
                        if (nextPersonShouldGoAndIfEnd > 0)
                        {
                            round = 0;
                            nextPersonShouldGoAndIfEnd -= 1;
                        } else if (cardCollectStarted == true && nextPersonShouldGoAndIfEnd == 0)
                        {
                            if (peopleCount == 2)
                            {
                                round = 0;
                                GetTheCards(1);
                            }
                            else if (peopleCount == 4)
                            {
                                GetTheCards(3);
                                round = 2;
                            } 
                        }
                    }
                    round = 1;
                }
                p1c.RemoveAt(0);
                break;
            case 1:
                man = p2c[0];
                p2c.RemoveAt(0);
                if (peopleCount == 2)
                {
                    round = 0;
                }
                else if (peopleCount == 4)
                {
                    round = 2;
                }
                break;
            case 2:
                man = p3c[0];
                p3c.RemoveAt(0);
                round = 3;
                break;
            case 3:
                man = p4c[0];
                p4c.RemoveAt(0);
                round = 0;
                break;
        }
        


        //kartı belirleyip spawn yapıyo
        switch (man[1])
        {
            case '1': whicCard = 0; break;
            case '2': whicCard = 1; break;
            case '3': whicCard = 2; break;
            case '4': whicCard = 3; break;
            case '5': whicCard = 4; break;
            case '6': whicCard = 5; break;
            case '7': whicCard = 6; break;
            case '8': whicCard = 7; break;
            case '9': whicCard = 8; break;
            case '0': whicCard = 9; break;
            case 'j': whicCard = 10; break;
            case 'q': whicCard = 11; break;
            case 'k': whicCard = 12; break;
        }
        GameObject cardDrawn;
        switch (man[0])
        {
            case 'p': cardDrawn = Instantiate(cardP[whicCard]); break;
            case 'l': cardDrawn = Instantiate(cardL[whicCard]); break;
            case 'f': cardDrawn = Instantiate(cardF[whicCard]); break;
            case 'c': cardDrawn = Instantiate(cardC[whicCard]); break;
            default: cardDrawn = Instantiate(cardBackPrefab);  break;

        }
        cardDrawn.transform.SetParent(middleDeckCOntainer.transform);
        DeckCount += 1;
    }


    public void GetTheCards(int whoGets)
    {
        lastCard = "";

    }

    void Start() {
        twoPAll.SetActive(false);
        fourPAll.SetActive(false);
        if (peopleCount == 2)
        {
            personPoints = new int[2];
            twoPAll.SetActive(true);
            fourPAll.SetActive(false);
        }
        else if (peopleCount == 4)
        {
            personPoints = new int[4];
            fourPAll.SetActive(true);
            twoPAll.SetActive(false);
        }
        else
        {
            Debug.Log("what that shouldn't have happened");
        }
        CardRandomise();

    }

    // kimin sırası olduğunu ve ona göre kart knotrollörünü yapıyo &&& kart dağıtma algoritmasını çağırıyor
    void FixedUpdate()
    {
        // kimin sırası olduğunu ve ona göre kart knotrollörünü yapıyo
        switch (round)
        {
            case 0:
                if(p1c == null)
                {
                    round = 1;
                    break;
                }
                arrowsTwo[0].SetActive(true);
                arrowsTwo[1].SetActive(false);
                for (int i = 0; i < 4; i++)
                {
                    playerTouch[i].SetActive(false); arrowsFour[i].SetActive(false);
                }
                arrowsFour[0].SetActive(true);
                playerTouch[0].SetActive(true);
                break;
            case 1:
                if (p2c == null)
                {
                    if (peopleCount == 2)
                    {
                        round = 0;
                    }
                    else if (peopleCount == 4)
                    {
                        round = 2;
                    }
                    break;
                }

                arrowsTwo[0].SetActive(false);
                arrowsTwo[1].SetActive(true);
                for (int i = 0; i < 4; i++)
                {
                    playerTouch[i].SetActive(false); arrowsFour[i].SetActive(false);
                   
                }
                arrowsFour[1].SetActive(true);
                playerTouch[1].SetActive(true);
                break;
            case 2:
                for (int i = 0; i < 4; i++)
                {
                    playerTouch[i].SetActive(false); arrowsFour[i].SetActive(false);
                    
                }
                arrowsFour[2].SetActive(true);
                playerTouch[2].SetActive(true);
                break;
            case 3:
                for (int i = 0; i < 4; i++)
                {
                    playerTouch[i].SetActive(false); arrowsFour[i].SetActive(false);
                }
                arrowsFour[3].SetActive(true);
                playerTouch[3].SetActive(true);
                break;
        }
        // kart dağıtma algoritmasını çağırıyor
        if (CardOutGo)
        {
            Cardout();
            CardOutGo = false;
        }
    }
    
    // kartları dağıtma animasyonu ve hareketi çağırma
    void CardAnimation(int count) {
        
        if (count >= 52)
        {
            CardOutGo = true;
            return;
        }
        Vector3 posOf = new Vector3 (0,0,0+(((float)count) / 100));
        Quaternion rotOF = new Quaternion(0, 0, 0, 0);
        GameObject instantiatedCard = Instantiate(cardBackPrefab, posOf, rotOF);
        StartCoroutine(CardMove(instantiatedCard, count * 0.25f));
        CardAnimation(count + 1);
    }
      
    // söylenen objenin 0.5 saniyede bir yere gitmesini sağlıyo kartlar için
    IEnumerator move(int count, int divide, Vector3 goTo, GameObject myCard)
    {
        if (count > 0)
        {
            yield return new WaitForSeconds(0.01f);
            Vector3 needToMoveTotal = new Vector3(0f + goTo.x, 0f + goTo.y,0f);
            myCard.transform.Translate(needToMoveTotal.x / divide, needToMoveTotal.y / divide, 0);   
            StartCoroutine(move(count - 1, divide, goTo, myCard));
        }
    }

    IEnumerator GetCards(int count, int divide, Vector3 goTo, GameObject myCard)
    {
        if (count > 0)
        {
            yield return new WaitForSeconds(0.01f);
            Vector3 needToMoveTotal = new Vector3(0f + goTo.x, 0f + goTo.y, 0f);
            myCard.transform.Translate(needToMoveTotal.x / divide, needToMoveTotal.y / divide, 0);

            StartCoroutine(move(count - 1, divide, goTo, myCard));
        }
    }

    // kartlardan hangisin hareket ediceğine karar veriyo ilk başta
    IEnumerator CardMove(GameObject card, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (peopleCount == 2)
        {
            if (checkSwith)
            {
                cardf1.Add(card);
                StartCoroutine(move(50, 50, twoSpawn[0].transform.position, card));
                checkSwith = false;
            }
            else
            {
                cardf2.Add(card);
                StartCoroutine(move(50, 50, twoSpawn[1].transform.position, card));
                checkSwith = true;
            }
        }else if (peopleCount == 4)
        {
            if (which == 0)
            {
                cardf1.Add(card);
                StartCoroutine(move(50, 50, fourSpawn[0].transform.position, card));
                which = 1;
            }
            else if (which == 1)
            {
                cardf2.Add(card);
                StartCoroutine(move(50, 50, fourSpawn[1].transform.position, card));
                which = 2;
            }
            else if (which == 2)
            {
                cardf3.Add(card);
                StartCoroutine(move(50, 50, fourSpawn[2].transform.position, card));
                which = 3;
            }
            else if (which == 3)
            {
                cardf4.Add(card);
                StartCoroutine(move(50, 50, fourSpawn[3].transform.position, card));
                which = 0;
            }
        }
    }

    // kartların insanlara tanımlamasını yapıyo
    public void Cardout()
    {
        // kartların insanlara tanımlamasını yapıyo
        if (peopleCount == 2)
        {
            for (int i = 0; i < 26; i++)
            {
               
                p1c.Add(randomizedCards[0]);
                randomizedCards.RemoveAt(0);
            }
            for (int i = 0; i < 26; i++)
            {
                p2c.Add(randomizedCards[0]);
                randomizedCards.RemoveAt(0);

            }
        }
        else if (peopleCount == 4)
        {
            for (int i = 0; i < 13; i++)
            {
              
                p1c.Add(randomizedCards[0]);
                randomizedCards.RemoveAt(0);
            }
            for (int i = 0; i < 13; i++)
            {
                p2c.Add(randomizedCards[0]);
                randomizedCards.RemoveAt(0);
            }
            for (int i = 0; i < 13; i++)
            {
                p3c.Add(randomizedCards[0]);
                randomizedCards.RemoveAt(0);
            }
            for (int i = 0; i < 13; i++)
            {
                p4c.Add(randomizedCards[0]);
                randomizedCards.RemoveAt(0);
            }
        }
    }

    // randomisin the cards	and the deck
    public void CardRandomise()
    {
        // randomisin the cards	and the deck
        for (int i = 0; i < 4; i++)
        {
            for (int a = 0; a < 13; a++)
            {
                string newCard = "" + typeOfCard[i] + cardNumbers[a];
                cards.Add(newCard);
                Debug.Log(newCard);
            }
        }
        for (int i = 0; i < cards.Count;)
        {
            int random = Random.Range(0, cards.Count);
            randomizedCards.Add(cards[random]);
            cards.RemoveAt(random);
        }
        CardAnimation(0);
    }
}
