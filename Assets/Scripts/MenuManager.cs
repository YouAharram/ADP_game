using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Inserisci Pannelli dal Canvas")]
    public GameObject panelMenuPrincipale;
    public GameObject panelSelezioneLobby;
    public GameObject panelStanzaAttesa;

    private void SpegniTuttiIPannelli()
    {
        panelMenuPrincipale.SetActive(false);
        panelSelezioneLobby.SetActive(false);
        panelStanzaAttesa.SetActive(false);
    }

    public void ApriMenuPrincipale()
    {
        SpegniTuttiIPannelli();
        panelMenuPrincipale.SetActive(true);
    }

    public void ApriSelezioneLobby()
    {
        SpegniTuttiIPannelli();
        panelSelezioneLobby.SetActive(true);
    }

    public void ApriStanzaAttesa()
    {
        SpegniTuttiIPannelli();
        panelStanzaAttesa.SetActive(true);
    }
    
    public void EsciDalGioco()
    {
        Application.Quit();
    }
}