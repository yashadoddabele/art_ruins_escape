using System.Collections;
using UnityEngine;

// Class that handles sprite behavior
public class SpriteScript : MonoBehaviour
{
    // Movement and respawn variables
    public Rigidbody2D rigidBody;
    public float jumpForce = 18f;
    public float speed = 17f;
    public Animator animator;
    private bool isFacingRight = true;
    public float fallThreshold = -10f;
    public float minX, maxX;
    public Vector3 respawnPoint;
    public bool hasTriggeredExit;

    // Raycasting variables 
    public float castDistance;
    public LayerMask groundLayer;
    public float runThreshold = 0.1f;
    
    // Painting and Item interaction flags
    public bool isOverlappingPainting = false;
    public bool canPickUpItem = false;
    private ItemScript currentItem = null; // reference to item collided with
    public bool itemSpawned = false;
    public bool usedItem = false;

    // Prefab to spawn a new item 
    public GameObject itemPrefab;
    private GameObject spawnedItem;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Respawn();
    }

    void Update() 
    {
        // Respawn if fallen too far
        if (transform.position.y < fallThreshold)
        {
            AudioManager.Instance.DyingSound();
            LevelManager.Instance.incrementDeathCount();
            Respawn();
        }

        //Update velocity, jump, and movement
        UpdateVelocity();
        MirrorSprite();
        HandleJump();

        // If overlapping a painting and player presses X, spawn a new item (if not already spawned)
        if (isOverlappingPainting && Input.GetKeyDown(KeyCode.X) && !itemSpawned)
        {
            if (itemPrefab != null) {
                spawnedItem = ItemScript.InstantiateItem(itemPrefab, transform);
                Debug.Log("instantiated item");
                itemSpawned = true;
            }
        }

        // If overlapping an item and pressing X, pick it up
        PickUpItem();

        // Clamp player's movement to the current screen ounds
        ClampPosition();
    }

    private void UpdateVelocity() {
        float moveInput = Input.GetAxis("Horizontal");
        float nextXPosition = transform.position.x + moveInput * speed * Time.deltaTime;

        // Update the horizontal and vertical velocity of the sprite from key input
        if (nextXPosition >= minX && nextXPosition <= maxX)
        {
            rigidBody.linearVelocity = new Vector2(moveInput * speed, rigidBody.linearVelocity.y);
        }
        else
        {
            rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
        }

        // Set animator flags
        animator.SetFloat("xVelocity", Mathf.Abs(rigidBody.linearVelocity.x));
        animator.SetFloat("yVelocity", rigidBody.linearVelocity.y);

        // Loop run audio if moving
        if (Mathf.Abs(moveInput) > runThreshold) {
            AudioManager.Instance.RunSound();
        }
        else { //Stop the clip if not moving
            AudioManager.Instance.StopRunSound();
        }
    }

    // Flip's the sprite's image based on which direction it's facing
    private void MirrorSprite() {
        if (rigidBody.linearVelocity.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (rigidBody.linearVelocity.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void HandleJump() {
        // Jump if space is pressed and not already in air
        bool grounded = isGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);
            animator.SetBool("inAir", true);

            // Play jump audio
            AudioManager.Instance.StopRunSound();
            AudioManager.Instance.JumpSound();

        }
        else if (grounded) {
            animator.SetBool("inAir", false);
        }
    }

    // Method handling item pick up interactions by the sprite
    private void PickUpItem() {
        if (canPickUpItem && Input.GetKeyDown(KeyCode.X))
        {
            // Stop run sound and initiate pickup sound
            AudioManager.Instance.StopRunSound();
            AudioManager.Instance.PickupSound();

            // Initiate pickup animation
            animator.SetBool("isPickingUp", true); 

            Rigidbody2D itemRb = currentItem.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                itemRb.bodyType = RigidbodyType2D.Dynamic;
                itemRb.gravityScale = 0f;

                // Attach a joint to the sprite to "hold" the item 
                FixedJoint2D joint = GetComponent<FixedJoint2D>();

                if (joint == null)
                {
                    joint = gameObject.AddComponent<FixedJoint2D>();
                }
                joint.autoConfigureConnectedAnchor = false;

                // Custom joint params
                joint.connectedAnchor = new Vector2(1f, 0f);  
                joint.breakForce = float.PositiveInfinity;
                joint.breakTorque = float.PositiveInfinity;
                joint.connectedBody = itemRb;
                canPickUpItem = false;
            }
            StartCoroutine(ResetPickupAnimation());
        }
    }

    // Coroutine to play the pickup animation
    private IEnumerator ResetPickupAnimation() {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isPickingUp", false);
    }

    // Called by PaintingScript when the player enters a painting trigger
    public void OnPaintingEnter(string paintingTag)
    {
        isOverlappingPainting = true;
    }

    // Called by PaintingScript when the player exits a painting trigger
    public void OnPaintingExit()
    {
        isOverlappingPainting = false;
    }

    // Called by ItemScript when the player enters an item trigger
    public void OnItemEnter(ItemScript item)
    {
        canPickUpItem = true;
        currentItem = item;
    }

    // Called by ItemScript when the player exits an item trigger
    public void OnItemExit(ItemScript item)
    {
        if (currentItem == item)
        {
            canPickUpItem = false;
            currentItem = null;
        }
    }

    // Checks if user has reached the exit point of the level and loads the next one
     private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggeredExit && other.CompareTag("Exit"))
        {
            hasTriggeredExit = true;
            other.gameObject.GetComponent<Collider2D>().enabled = false;
            LevelManager.Instance.LoadNextLevel();
        }
    }

    // Raycasting method to check if the user is on Ground 
    public bool isGrounded() {
        // Calculate the origin at the bottom of the sprite
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, castDistance, groundLayer);
        //For debugging
        //Debug.DrawRay(transform.position, Vector2.down * castDistance, Color.red);
    
        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            if (Vector2.Dot(hit.normal, Vector2.up) >= 0.9f)
            {
                return true;
            }
        }
        return false;
    }

    //Helper method to actually flip the sprite renderer
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Respawns sprite at designated respawn area for level
    private void Respawn()
    {
        Vector3 adjustedRespawn = respawnPoint;
        adjustedRespawn.y += 0.5f;

        Debug.Log($"Respawning to: {adjustedRespawn.x}, {adjustedRespawn.y}");

        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;

        transform.position = adjustedRespawn;

        animator.SetBool("inAir", !isGrounded());

        //Delete an item if it spawned and was never used
        if (spawnedItem != null) {
            Destroy(spawnedItem);
        }
        //Reset item if gotten
        currentItem = null;
        itemSpawned = false;
        canPickUpItem = false;
    }

    private void ClampPosition()
    {
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    //Triggers the item use animation for the player
    public void TriggerUsingItem(bool state)
    {
        usedItem = true;
        string tagName = null;
        animator.SetBool("usingItem", state);
        
        if (itemPrefab != null && state) {
            animator.SetBool(itemPrefab.tag, state);
            tagName = itemPrefab.tag;
        }

        // Destroy joint as the item can be discarded
        FixedJoint2D joint = GetComponent<FixedJoint2D>();
        if (joint != null)
        {
            GameObject heldItemInstance = joint.connectedBody.gameObject;
            Destroy(heldItemInstance);
            Destroy(joint);
        }
        StartCoroutine(ResetItemUseAnimation(tagName));
    }

    // Resets the item animation
     private IEnumerator ResetItemUseAnimation(string tagName) {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("usingItem", false);
        if (tagName != null) {
            animator.SetBool(itemPrefab.tag, false);
        }
        itemPrefab = null;
    }

}
