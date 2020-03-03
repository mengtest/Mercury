using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectControlSelf : MonoBehaviour
{
    private void OnAnimPlayEnd()
    {
        Mercury.GameManager.Instance.EffCont.AddEffectToQueue(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
