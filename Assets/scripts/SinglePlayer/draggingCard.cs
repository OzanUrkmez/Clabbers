using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class draggingCard : MonoBehaviour, IDragHandler,IPointerDownHandler, IPointerUpHandler {

    public Image back;
    public Image inside;
    public GameObject insSayd;
    public Vector2 insidePos;
    public int person;
    public offlineGameController contrllergame;
    string man;
    public void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(back.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            //pos.x = pos.x / (back.rectTransform.sizeDelta.x * 2);
            //pos.y = pos.y / (back.rectTransform.sizeDelta.y * 2);
            switch (person)
            {
                case 0:
                    contrllergame.cardf1[0].transform.localPosition = new Vector3(pos.x / 3, pos.y/ 3, 0);
                    break;                                                                        
                case 1:                                                                           
                    contrllergame.cardf2[0].transform.localPosition = new Vector3(pos.x / 3, pos.y/ 3, 0);
                    break;                                                                        
                case 2:                                                                            
                    contrllergame.cardf3[0].transform.localPosition = new Vector3(pos.x / 3, pos.y/ 3, 0);
                    break;                                                                        
                case 3:                                                                           
                    contrllergame.cardf4[0].transform.localPosition = new Vector3(pos.x / 3, pos.y / 3, 0);
                    break;

            }
            //insSayd.transform.localPosition = new Vector3 (pos.x, pos.y, 0);
        }
    }
    public void OnPointerDown(PointerEventData yes) {

    }
    public void OnPointerUp(PointerEventData yes)
    {
        switch (person)
        {
            case 0:
                
                Destroy(contrllergame.cardf1[0]); contrllergame.cardf1.RemoveAt(0);
                break;
            case 1:
                
                Destroy(contrllergame.cardf2[0]); contrllergame.cardf2.RemoveAt(0);
                break;
            case 2:
                
                Destroy(contrllergame.cardf3[0]); contrllergame.cardf3.RemoveAt(0);
                break;
            case 3:

                Destroy(contrllergame.cardf4[0]); contrllergame.cardf4.RemoveAt(0);
                break;

        }
        contrllergame.cardSpawn(person);
        if (offlineGameController.peopleCount == 4)
        {

        }
        if (person == 3)
        {
            person = 0;
        }
        else
        {
            person += 1;
        }
    }

}
