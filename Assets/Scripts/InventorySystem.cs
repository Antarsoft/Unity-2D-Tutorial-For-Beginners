using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [Header("General Fields")]
    //List of items picked up
    public List<GameObject> items= new List<GameObject>();
    //flag indicates if the inventory is open or not
    public bool isOpen;
    [Header("UI Items Section")]
    //Inventory System Window
    public GameObject ui_Window;
    public Image[] items_images;
    [Header("UI Item Description")]
    public GameObject ui_Description_Window;
    public Image description_Image;
    public Text description_Title;
    public Text description_Text;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        ui_Window.SetActive(isOpen);

        Update_UI();
    }

    //Add the item to the items list
    public void PickUp(GameObject item)
    {
        items.Add(item);
        Update_UI();
    }

    //Refresh the UI elements in the inventory window    
    void Update_UI()
    {
        HideAll();
        //For each item in the "items" list 
        //Show it in the respective slot in the "items_images"
        for(int i=0;i<items.Count;i++)
        {
            items_images[i].sprite = items[i].GetComponent<SpriteRenderer>().sprite;
            items_images[i].gameObject.SetActive(true);
        }
    }

    //Hide all the items ui images
    void HideAll() 
    { 
        foreach (var i in items_images) { i.gameObject.SetActive(false); }

        HideDescription();
    }
    
    public void ShowDescription(int id)
    {
        //Set the Image
        description_Image.sprite = items_images[id].sprite;
        //Set the Title
        description_Title.text = items[id].name;
        //Show the description
        description_Text.text = items[id].GetComponent<Item>().descriptionText;
        //Show the elements
        description_Image.gameObject.SetActive(true);
        description_Title.gameObject.SetActive(true);
        description_Text.gameObject.SetActive(true);
    }

    public void HideDescription()
    {
        description_Image.gameObject.SetActive(false);
        description_Title.gameObject.SetActive(false);
        description_Text.gameObject.SetActive(false);
    }

    public void Consume(int id)
    {
        if(items[id].GetComponent<Item>().type== Item.ItemType.Consumables)
        {
            Debug.Log($"CONSUMED {items[id].name}");
            //Invoke the cunsume custome event
            items[id].GetComponent<Item>().consumeEvent.Invoke();
            //Destroy the item in very tiny time
            Destroy(items[id], 0.1f);
            //Clear the item from the list
            items.RemoveAt(id);
            //Update UI
            Update_UI();
        }
    }
}
