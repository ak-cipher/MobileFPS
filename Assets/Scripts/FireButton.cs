
using UnityEngine;
using UnityEngine.EventSystems;

public class FireButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public bool isFiring;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
  
        isFiring = true;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {

        isFiring = false;
    }
}
