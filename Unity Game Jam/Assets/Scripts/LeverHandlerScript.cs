using System.Collections;
using UnityEngine;

// Script to handle the level-handler in level 3
public class LeverHandlerScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Collider2D platformCollider;  
    private bool isCatalyzed = false;
    private bool isPlayerInRange = false;
    private GameObject playerObj;
    public GameObject movableWall;
    public GameObject inactiveLever; //Lever to be a part of bg after usage


    // Functionality is very similar to plant script in terms of collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerObj = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerObj = null;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.C))
        {
            FixedJoint2D playerJoint = playerObj.GetComponent<FixedJoint2D>();

            if (playerJoint != null && playerJoint.connectedBody != null)
            {
                GameObject heldItem = playerJoint.connectedBody.gameObject;
                if (heldItem.CompareTag(gameObject.tag) &&
                    heldItem.layer == LayerMask.NameToLayer("Item"))
                {
                    Debug.Log("Catalyzing?");
                    if (!isCatalyzed)
                    {
                        AudioManager.Instance.LeverSound();
                        TriggerSpriteAnimation(true);
                        isCatalyzed = true;
                        StartCoroutine(ResetGrowthAnimation());
                    }
                }
            }
        }
    }

    private IEnumerator ResetGrowthAnimation() {
        yield return new WaitForSeconds(1f);
        OnLeverPulled();
    }

    public void OnLeverPulled()
    {
        Debug.Log("on lever pulled called");
        inactiveLever.SetActive(true);

        //Catalyzed: Logic to move the MovableWall object out of the screen
        Rigidbody2D rb = movableWall.AddComponent<Rigidbody2D>();
        TriggerSpriteAnimation(false);
        AudioManager.Instance.WallSound();
    }


    private void TriggerSpriteAnimation(bool state) {
        SpriteScript spriteScript = playerObj.GetComponent<SpriteScript>();
        if(spriteScript != null)
        {
            spriteScript.TriggerUsingItem(state);
        }
    }
}
