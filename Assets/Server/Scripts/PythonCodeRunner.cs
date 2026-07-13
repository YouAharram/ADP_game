using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PythonCodeRunner : MonoBehaviour
{
    private string questionText;
    private string answerText;
    private int questionNumber = 0;

    public string QuestionText { get => questionText; }
    public string AnswerText { get => answerText;}

    public void SelectQuestionByLevel(int level)
    {
        questionNumber = level;
        questionText = "Se vuoi migliorare il tuo attacco, risolvi la sfida seguente: \n";
        switch (questionNumber)
        {
            case 1:
                questionText += "Sono dichiarate due variabili a e b, in una c'è un valore pari e in un'altra un valore dispari. " 
                    + "Scrivi un codice python per inserire il valore pari dentro la variabile 'increment' (già dichiarata)."
                    + "Se corretto, il tuo attacco aumenterà di increment!";
                break;
            case 2:
                questionText += "Sono dichiarate due variabili a e b, in a c'è 0 e in b il valore di incremento. " 
                    + "Scrivi un codice python per scambiare il contenuto delle due variabili."
                    + "Se corretto, il tuo attacco aumenterà di b!";
                break;
            case 3:
                questionText += "È dichiarata una variabile x. Scrivi un codice python per calcolare il 20% di x "
                    + "e salvalo nella variabile increment. Se corretto, "
                    + "il tuo attacco aumenterà del numero trovato!";
                break;
            case 4:
                questionText += "È dichiarata una variabile n. Scrivi un codice python per calcolare la somma "
                    + "di tutti i numeri da 1 fino ad n e salvala nella variabile increment. Se corretto, "
                    + "il tuo attacco aumenterà del numero trovato!";
                break;
            case 5:
                questionText += "È dichiarata e istanziata una lista di numeri chiamata lista. "
                    + "Scrivi un codice python per trovare il valore massimo di questa lista e "
                    + "salvalo nella variabile increment. Il tuo attacco aumenterà del numero trovato! "
                    + "Attenzione: non utilizzare la funzione max().";
                break;
        }
    }

    public int ExecuteCode(string codeToExecute)
    {
        switch (questionNumber)
        {
            case 1: 
                return ExecuteCode_EvenOdd(codeToExecute);
            case 2:
                return ExecuteCode_SwapVariables(codeToExecute);
            case 3:
                return ExecuteCode_Percentage(codeToExecute);
            case 4:
                return ExecuteCode_Summary(codeToExecute);
            case 5:
                return ExecuteCode_MaxList(codeToExecute);
            default:
                throw new System.Exception("Errore nell'assegnazione del livello nel PythonCodeRunner");
        }
    }

    private string RunPythonProcess(string pythonCode)
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

                throw new System.Exception("Errore! Il codice ci ha messo troppo tempo ad eseguire (probabile loop infinito).");                
            }

        }

        if (File.Exists(tempFilePath)) File.Delete(tempFilePath);

        if (!string.IsNullOrEmpty(error))
        {
            throw new System.Exception(error.Split('\n')[error.Split('\n').Length - 2]);
        }

        return output;
    }


    private int ResultToInt(string result)
    {
        UnityEngine.Debug.Log("Stringa di risultato: " + result);
        if (!int.TryParse(result, out int increment))
            {
                return 0;
            }
        return increment;
    }


    private int ExecuteCode_MaxList(string codeToExecute)
    {
        try
        {
            List<int> list = new List<int>();
            int listLength = 20;

            for (int i = 0; i < listLength; i++)
            {
                list.Add(Random.Range(-30, 31));
            }

            string listAsPythonArray = "[" + string.Join(", ", list) + "]";
            
            string fullScript = 
                $"list = {listAsPythonArray}\n" +
                $"increment = 0\n" +
                $"{codeToExecute}\n\n" +
                $"print(increment)\n";
            
            int increment = ResultToInt(RunPythonProcess(fullScript));

            if (!list.Contains(increment) || codeToExecute.Contains("max"))
            {
                answerText = "Codice non corretto! Incremento non avvenuto.";
                return 0;
            }

            answerText = "Il codice compila! Il valore trovato è " + increment + ". Il tuo parametro aumenterà di " + increment;
            return increment;            
        }
        catch (System.Exception ex)
        {
            answerText = $"Errore Python: {ex.Message}. Incremento non avvenuto.";
            return 0;
        }
    }

    private int ExecuteCode_Summary(string codeToExecute)
    {
        try
        {
            int n = Random.Range(4, 7);

            string fullScript = 
                $"increment = 0\n" +
                $"n = {n}\n" +
                $"{codeToExecute}\n\n" +
                $"print(increment)\n";

            int increment = ResultToInt(RunPythonProcess(fullScript));

            if (increment != n * (n + 1) / 2)
            {
                answerText = "Codice errato! Incremento non avvenuto";
                return 0;
            }

            answerText = "Il codice compila! Il valore trovato è " + increment + ". Il tuo parametro aumenterà di " + increment;
            return increment;
        }
        catch (System.Exception ex)
        {
            answerText = $"Errore Python: {ex.Message}. Incremento non avvenuto.";
            return 0;
        }
    }

    private int ExecuteCode_SwapVariables(string codeToExecute)
    {
        try
        {
            int b = Random.Range(5, 11);

            string fullScript = 
                $"a = 0\n" +
                $"b = {b}\n" +
                $"{codeToExecute}\n\n" +
                $"print(a)\n";

            int increment = ResultToInt(RunPythonProcess(fullScript));

            if (increment != b)
            {
                answerText = "Codice errato! Incremento non avvenuto";
                return 0;
            }

            answerText = "Codice corretto! Il tuo parametro aumenterà di " + increment;
            return increment;
        }
        catch (System.Exception ex)
        {
            answerText = $"Errore Python: {ex.Message}. Incremento non avvenuto.";
            return 0;
        }
    }

    private int ExecuteCode_EvenOdd(string codeToExecute)
    {
        try
        {
            int a = Random.Range(1, 5)*2;
            int b = Random.Range(1, 5)*2 + 1;

            string fullScript = 
                $"a = {a}\n" +
                $"b = {b}\n" +
                $"increment = 0\n"+
                $"{codeToExecute}\n\n" +
                $"print(increment)\n";

            int increment = ResultToInt(RunPythonProcess(fullScript));

            int even = 0;
            if (a % 2 == 0)
                even = a;
            else 
                even = b;

            if (increment != even)
            {
                answerText = "Codice errato! Incremento non avvenuto";
                return 0;
            }

            answerText = "Codice corretto! Il tuo parametro aumenterà di " + increment;
            return increment;
        }
        catch (System.Exception ex)
        {
            answerText = $"Errore Python: {ex.Message}. Incremento non avvenuto.";
            return 0;
        }
    }

    private int ExecuteCode_Percentage(string codeToExecute)
    {
        try
        {
            int x = 50;

            string fullScript = 
                $"x = {x}\n" +
                $"increment = 0\n"+
                $"{codeToExecute}\n\n" +
                $"print(increment)\n";

            int increment = ResultToInt(RunPythonProcess(fullScript));

            if (increment != x*20/100)
            {
                answerText = "Codice errato! Incremento non avvenuto";
                return 0;
            }

            answerText = "Codice corretto! Il tuo parametro aumenterà di " + increment;
            return increment;
        }
        catch (System.Exception ex)
        {
            answerText = $"Errore Python: {ex.Message}. Incremento non avvenuto.";
            return 0;
        }
    }
}