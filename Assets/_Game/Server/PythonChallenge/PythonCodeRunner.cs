using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PythonCodeRunner : MonoBehaviour
{
    private string answerText;
    private PythonChallenge pythonChallenge;

    public string QuestionText { get => pythonChallenge.GetQuestionText(); }
    public string AnswerText { get => answerText;}
    public PythonChallenge PythonChallenge { set => pythonChallenge = value; }

    public int ExecuteCode(string codeToExecute)
    {
        answerText = "";
        string fullCode = pythonChallenge.SetupCode(codeToExecute);
        int result = RunPythonProcess(fullCode);
        if (answerText == "")
            answerText = pythonChallenge.AnswerText(codeToExecute, result);
        return result;
    }

    private int RunPythonProcess(string pythonCode)
    {
        string tempFilePath = Path.Combine(Application.temporaryCachePath, "temp_quiz.py");
        File.WriteAllText(tempFilePath, pythonCode);

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python3"; 
        start.Arguments = tempFilePath;
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        string output = "";
        string error = "";

        using (Process process = Process.Start(start))
        {
            int timeoutMilliseconds = 5000;

            if (process.WaitForExit(timeoutMilliseconds))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    output = reader.ReadToEnd();
                }
                using (StreamReader errorReader = process.StandardError)
                {
                    error = errorReader.ReadToEnd();
                }
            }
            else
            {
                process.Kill();
                
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);

                answerText = "Error! The code took too long to execute (likely an infinite loop).";
                return 0;                
            }

        }

        if (File.Exists(tempFilePath)) File.Delete(tempFilePath);

        if (!string.IsNullOrEmpty(error))
        {
            answerText = error.Split('\n')[error.Split('\n').Length - 2];
            return 0;
        }

        if (!int.TryParse(output, out int increment))
            {
                answerText = "Variable not assigned correctly or a print is present! Upgrade failed.";
                return 0;
            }

        return increment;
    }




}