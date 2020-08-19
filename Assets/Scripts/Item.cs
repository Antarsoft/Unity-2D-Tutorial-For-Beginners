using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Item : MonoBehaviour
{
    public enum InteractionType { NONE,PickUp,Examine}
    public InteractionType type;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        gameObject.layer = 10;
    }

    public void Interact()
    {
        switch(type)
        {
            case InteractionType.PickUp:
                //Add the object to the PickedUpItems list
                FindObjectOfType<InteractionSystem>().PickUpItem(gameObject);
                //Disable
                gameObject.SetActive(false);
                break;
            case InteractionType.Examine:
                Debug.Log("EXAMINE");
                break;
            default:
                Debug.Log("NULL ITEM");
                break;
        }
    }
}
