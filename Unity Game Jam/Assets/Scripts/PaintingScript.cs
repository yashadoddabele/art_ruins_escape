using UnityEngine;

// Class handling painting behavior
public class PaintingScript : MonoBehaviour
{
    public GameObject xInstruction; //Reference to xInstruction, if it has one
    public GameObject pickupInstruction; //Same as above: reference to pickup instruction
    
    // Detects an overlap of the sprite and painting. If this overlap exists, a sprite can spawn the item from the painting
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteScript player = other.GetComponent<SpriteScript>();
            if (player != null)
            {
                player.OnPaintingEnter(gameObject.tag);
            }
            // Enable to X Instruction, if the level has one
            if (xInstruction != null && !player.itemSpawned && player.itemPrefab != null)
            {
                Debug.Log("enabled X inst");
                xInstruction.SetActive(true);
            }

            //Enable pickup instruction, if level has one
            if (pickupInstruction != null && player.canPickUpItem && !player.usedItem) {
                Debug.Log("enabling pickup instructions");
                pickupInstruction.SetActive(true);
            }
        }
    }

    //Ensures instruction is still disabled even if still overlapping with player
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteScript player = other.GetComponent<SpriteScript>();
            if (player != null && xInstruction != null)
            {
                if (xInstruction != null && (player.itemSpawned || player.itemPrefab == null))
                {
                    xInstruction.SetActive(false);
                }
                if (pickupInstruction != null && (!player.canPickUpItem || player.usedItem)) {
                    pickupInstruction.SetActive(false);
                }
                else {
                    pickupInstruction.SetActive(true);
                }
            }
        }
    }

    // Disables instructions and triggers if player not in front of painting anymore.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteScript player = other.GetComponent<SpriteScript>();
            if (player != null)
            {
                player.OnPaintingExit();
            }
            // Disable the X Instruction, if the level has one
            if (xInstruction != null)
            {
                xInstruction.SetActive(false);
            }
            if (pickupInstruction != null) {
                pickupInstruction.SetActive(false);
            }
        }
    }
}

