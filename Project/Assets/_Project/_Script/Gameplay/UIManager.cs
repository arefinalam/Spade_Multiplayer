using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text trickMessageText;
    [SerializeField] private GameObject endResultPopup;
    // Start is called before the first frame update
    void Start()
    {
        endResultPopup.SetActive(false);
    }

    public void ReceiveGameplayMessage(string message)
    {
        trickMessageText.text = message;
        StartCoroutine(ClearMessageAfterDelay(1.5f)); // Start coroutine to clear the message
    }
    // Coroutine to clear the message after a delay
    private IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        trickMessageText.text = string.Empty; // Clear the message
    }

    public void SetEndResult(Dictionary<PlayerController, int> scores)
    {
        endResultPopup.SetActive(true);
    }


}
