using System.Collections;
using UnityEngine;

// Class that handles plant object behavior
public class PlantScript : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Sprite finalPlantSprite;   
    public Collider2D platformCollider;  
    private bool isCatalyzed = false;
    private bool isPlayerInRange = false;
    private GameObject playerObj;
    public GameObject cInstruction; //Since the plant is in level 2, it may have mechanic instructions

    // Checks for collision with a sprite and triggers the instruction (if non null)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteScript player = other.GetComponent<SpriteScript>();
            isPlayerInRange = true;
            playerObj = other.gameObject;
            if (cInstruction != null && !isCatalyzed && !player.canPickUpItem && !player.usedItem
            && player.itemSpawned) {
                    cInstruction.SetActive(true);
            }
        }
    }

    // Resets instruction and flags after player has left collision
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerObj = null;
            if (cInstruction != null) {
                cInstruction.SetActive(false);
            }
        }
    }

    // Maintains updates of the instruction
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (cInstruction != null) {
            cInstruction.SetActive(false);
        }
    }

    private void Update()
    {
        //If player in range of this catalyst item and presses C...
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.C))
        {
            FixedJoint2D playerJoint = playerObj.GetComponent<FixedJoint2D>();

            if (playerJoint != null && playerJoint.connectedBody != null)
            {
                //Get the item attached the player with the joint and check if their tags are the same. This means
                //the item in hand can be used on the current object.
                GameObject heldItem = playerJoint.connectedBody.gameObject;
                if (heldItem.CompareTag(gameObject.tag) &&
                    heldItem.layer == LayerMask.NameToLayer("Item"))
                {
                    Debug.Log("Catalyzing?");
                    if (!isCatalyzed)
                    {
                        // Plant is catalyzed
                        AudioManager.Instance.WaterSound();
                        TriggerSpriteAnimation(true);
                        isCatalyzed = true;
                        if (cInstruction != null)
                        {
                            cInstruction.SetActive(false);
                        }
                        animator.SetBool("isCatalyzed", isCatalyzed);
                        StartCoroutine(ResetGrowthAnimation());
                    }
                }
            }
        }
    }

    // Resets the object change/growth animation
    private IEnumerator ResetGrowthAnimation() {
        yield return new WaitForSeconds(1f);
        animator.SetBool("isCatalyzed", false);
        OnPlantGrown();
    }

    public void OnPlantGrown()
    {
        Debug.Log("on plant grown called");
        animator.enabled = false;
        spriteRenderer.sprite = finalPlantSprite;
        
        // For the plant, the catalyst grows the collider as the plant grows
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y*0.7f);
            box.size = newSize;
            box.offset = spriteRenderer.sprite.bounds.center;
            box.enabled = true;
            box.isTrigger = false;
            Debug.Log("Collider isTrigger set to: " + box.isTrigger);
        }
        
        // Plant can now be stood on: change its layer/tag names to Ground
        gameObject.layer = LayerMask.NameToLayer("Ground");
        gameObject.tag = "Ground";
        TriggerSpriteAnimation(false);
    }

    //Trigger item use animation in SpriteScript
    private void TriggerSpriteAnimation(bool state) {
        SpriteScript spriteScript = playerObj.GetComponent<SpriteScript>();
        if(spriteScript != null)
        {
            spriteScript.TriggerUsingItem(state);
        }
    }
}
