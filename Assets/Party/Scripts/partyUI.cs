using TMPro;
using UnityEngine;

public class PartyUI : MonoBehaviour
{
    public static PartyUI Instance;

    public TMP_Text partyCodeText;

    void Awake()
    {
        Instance = this;
    }

    public void SetCode(string code)
    {
        if (partyCodeText != null)
            partyCodeText.text = "Party Code: " + code;
    }
}