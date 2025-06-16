using UnityEngine;
using TMPro;

// Class managing display of the death count
public class TextScript : MonoBehaviour
{
    private TextMeshProUGUI deathCountText;

    void Awake()
    {
        deathCountText = GetComponent<TextMeshProUGUI>();

        if (deathCountText == null)
        {
            Debug.LogError("text not found to update");
        }
    }

    void Update()
    {
        deathCountText.text = LevelManager.Instance.getDeathCount().ToString();
    }
}
