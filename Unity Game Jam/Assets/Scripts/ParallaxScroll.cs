using UnityEngine;

// Class implementing parallax scrolling in the background
public class ParallaxScroll : MonoBehaviour
{
    
    private float _startingPos; // starting position of the sprite
    private float _lengthOfSprite; // length of the sprites
    public float AmountOfParallax;  // amount of parallax scroll 
    public Camera MainCamera; 

    private void Start() {
        // Get starting X position of sprite
        _startingPos = transform.position.x;    
        // Get length of the sprites
        _lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update() {
        Vector3 Position = MainCamera.transform.position;
        float Temp = Position.x * (1 - AmountOfParallax);
        float Distance = Position.x * AmountOfParallax;

        Vector3 NewPosition = new Vector3(_startingPos + Distance, transform.position.y, transform.position.z);

        transform.position = NewPosition;
    }
}

