using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCircle : MonoBehaviour {

    public Image CircleImage;

    public IEnumerator InitiateReload(float seconds)
    {
        CircleImage.fillAmount = 1;
        float SubtractAmountPerSecond = 1 / seconds;
        while(seconds >= 0 && CircleImage.fillAmount > 0)
        {
            seconds -= Time.deltaTime;
            if(CircleImage.fillAmount >= SubtractAmountPerSecond * Time.deltaTime)
            {
                CircleImage.fillAmount -= SubtractAmountPerSecond * Time.deltaTime;
            }
            else
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        CircleImage.fillAmount = 0;
    }
	
}
