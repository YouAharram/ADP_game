using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AudioToggle : MonoBehaviour
{
    private bool isMuted = false;
    private Image buttonImage;
    
    [Header("Impostazioni Visive")]
    public Color normalColor = Color.white;
    public Color mutedColor = new Color(0.5f, 0.5f, 0.5f, 1f); 

    void Start()
    {
        buttonImage = GetComponent<Image>();
        
        UpdateVisuals(); 
    }

    public void ToggleAudio()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            AudioListener.volume = 0f; // muta tutto l'audio del gioco
        }
        else
        {
            AudioListener.volume = 1f; // alza volume
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (buttonImage != null)
        {
            buttonImage.color = isMuted ? mutedColor : normalColor;
        }
    }
}