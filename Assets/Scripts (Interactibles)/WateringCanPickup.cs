using UnityEngine;
using UnityEngine.EventSystems;

public class WateringCanPickup : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerTools.Instance.PickUpWateringCan();
        Destroy(gameObject);
    }
}