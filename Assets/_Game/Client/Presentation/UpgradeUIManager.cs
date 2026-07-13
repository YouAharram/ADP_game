using UnityEngine;
using TMPro;
using System.Collections; 

public class UpgradeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject bannerPanel;
    [SerializeField] private TextMeshProUGUI commandText;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private PythonCodeRunner pythonCodeRunner;

    private void Awake()
    {
        bannerPanel.SetActive(false);
    }

    public void ShowBanner()
    {
        feedbackText.text = "";
        codeInputField.text = "";

        // 1. Controlliamo l'Orchestrator
        if (GameOrchestrator.Instance == null)
        {
            Debug.LogError("[UI-CRASH-PREVENTION] GameOrchestrator.Instance è NULL sul client!");
            return;
        }

        // 2. Controlliamo il LevelManager dell'Orchestrator
        if (GameOrchestrator.Instance.LevelManager == null)
        {
            Debug.LogError("[UI-CRASH-PREVENTION] Il LevelManager dentro GameOrchestrator è NULL sul client!");
            return;
        }

        // 3. Proviamo a recuperare la sfida in sicurezza
        PythonChallenge currentChallenge = GameOrchestrator.Instance.LevelManager.GetPythonChallenge();
        if (currentChallenge == null)
        {
            Debug.LogError($"[UI-CRASH-PREVENTION] GetPythonChallenge() ha restituito NULL sul client! Livello attuale sincronizzato: {GameOrchestrator.Instance.LevelManager.Level}");
            return;
        }

        // 4. Controlliamo il riferimento locale al componente pythonCodeRunner della UI
        if (pythonCodeRunner == null)
        {
            Debug.LogError("[UI-CRASH-PREVENTION] La variabile 'pythonCodeRunner' non è assegnata nell'Inspector dell'UpgradeUIManager!");
            return;
        }
        pythonCodeRunner.PythonChallenge = GameOrchestrator.Instance.LevelManager.GetPythonChallenge();
        commandText.text = pythonCodeRunner.QuestionText;
        bannerPanel.SetActive(true);
    }

    public void OnSubmitUpgrade()
    {
        string playerCode = codeInputField.text;
        int playerReturnValue = pythonCodeRunner.ExecuteCode(playerCode);

        feedbackText.text = pythonCodeRunner.AnswerText;

        StartCoroutine(DisableBanner(5, playerReturnValue));
    
    }

    private IEnumerator DisableBanner(float delay, int returnValue)
    {
        yield return new WaitForSeconds(delay);

        bannerPanel.SetActive(false); 
        GameOrchestrator.Instance.CmdRegisterPlayerChoiceAndReady(returnValue);

    }
}