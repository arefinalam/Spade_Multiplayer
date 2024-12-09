using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Cards", menuName = "SO/Cards")]
public class CardSO : ScriptableObject
{
    public Card[] cards;

    [ButtonLUFI]
    void SetupID()
    {        
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].CardNumber = i;
        }
    }
}
