using UnityEngine;

// Class handling item behavior and player collisions
public class ItemScript : MonoBehaviour
{
    //Detects a collision with the player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteScript player = other.GetComponent<SpriteScript>();
            if (player != null)
            {
                player.OnItemEnter(this);
            }
        }
    }

    // Disables collision interaction flags
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteScript player = other.GetComponent<SpriteScript>();
            if (player != null)
            {
                player.OnItemExit(this);
            }
        }
    }

    //Instantiates a game object prefab that was assigned to the sprite in the inspector for the level
    public static GameObject InstantiateItem(GameObject itemPrefab, Transform transform) {
            GameObject spawnedItem = Instantiate(itemPrefab);
            AudioManager.Instance.PickupSound();
            spawnedItem.SetActive(true);
            spawnedItem.transform.position = new Vector3(
                transform.position.x + 2,
                transform.position.y - 1,
                transform.position.z
            );
            Debug.Log("Spawned new item");
            return spawnedItem;
    }
}
