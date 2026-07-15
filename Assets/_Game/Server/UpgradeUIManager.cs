using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UpgradeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject bannerPanel;
    [SerializeField] private TextMeshProUGUI commandText;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private PythonCodeRunner pythonCodeRunner;
    private float timeOut;
    private Coroutine timerCoroutine;
    private bool answered = false;

    private void Awake()
    {
        bannerPanel.SetActive(false);
    }

    public void ShowBanner()
    {
        feedbackText.text = "";
        codeInputField.text = "";

        pythonCodeRunner.PythonChallenge = GameOrchestrator.Instance.LevelManager.GetPythonChallenge();
        commandText.text = pythonCodeRunner.QuestionText;
        timeOut = pythonCodeRunner.TimeOut;

        timerCoroutine = StartCoroutine(TimerCountdown(timeOut));

        bannerPanel.SetActive(true);
        button.interactable = true;

        answered = false;
    }

    public void OnSubmitUpgrade()
    {
        answered = true;

        StopCoroutine(timerCoroutine);
        timeText.text = "";

        string playerCode = codeInputField.text;
        int playerReturnValue = pythonCodeRunner.ExecuteCode(playerCode);

        feedbackText.text = pythonCodeRunner.AnswerText;

        button.interactable = false;
        StartCoroutine(DisableBanner(5, playerReturnValue));
    
    }

    private IEnumerator TimerCountdown(float timeRemaining)
    {
        while (timeRemaining > 0)
        {
            timeText.text = "Time remaining: " + Mathf.CeilToInt(timeRemaining) + "s";
            
            yield return null;
            timeRemaining -= Time.deltaTime;
        }

        if (!answered)
        {
            timeText.text = "";
            feedbackText.text = "Time's up! Upgrade failed.";
            button.interactable = false;
            StartCoroutine(DisableBanner(5, 0));

        }
    }

    private IEnumerator DisableBanner(float delay, int returnValue)
    {
        yield return new WaitForSeconds(delay);

        bannerPanel.SetActive(false); 
        GameOrchestrator.Instance.CmdRegisterPlayerChoiceAndReady(returnValue);

    }
}