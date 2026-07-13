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

    public void ShowBanner(int level)
    {
        feedbackText.text = "";
        codeInputField.text = "";
        pythonCodeRunner.SelectQuestionByLevel(level);
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