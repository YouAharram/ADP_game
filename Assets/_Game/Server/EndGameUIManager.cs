using System.Collections;
using TMPro;
using UnityEngine;

public class EndGameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject bannerPanel;
    [SerializeField] private TextMeshProUGUI alertText;

    void Awake()
    {
        bannerPanel.SetActive(false);
    }

    public void ShowGameOverBanner()
    {
        bannerPanel.SetActive(true);
        alertText.text = "Game Over!";

        StartCoroutine(DisableBanner(5f));

    }

    public void ShowGameWonBanner()
    {
        bannerPanel.SetActive(true);
        alertText.text = "Game Won!";

        StartCoroutine(DisableBanner(5f));
    }

    private IEnumerator DisableBanner(float delay)
    {
        yield return new WaitForSeconds(delay);

        bannerPanel.SetActive(false); 
    }
}