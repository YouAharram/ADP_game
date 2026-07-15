using System.Collections.Generic;
using UnityEngine;
public abstract class PythonChallenge
{
    public string GetQuestionText()
    {
        return "If you want to improve your attack, solve the following challenge: \n"
            + GetSpecificQuestionText() +  "\n"
            + "Important: do not use the print() function to print the results!";

    }

    public string SetupCode(string codeToExecute)
    {
        return  "increment = 0\n" +
                SetupVariables() + "\n" +
                $"{codeToExecute}\n\n" +
                $"print(increment)\n";
    }

    public string AnswerText(string code, int result)
    {
        if (ValidateResult(code, result))
        {
            return "The code compiles! The value found is " + result + ". Your attack will increase by " + result;
        }

        return "Incorrect code! Upgrade failed.";
    }

    public abstract float GetTimeOut();

    protected abstract string SetupVariables();
    protected abstract string GetSpecificQuestionText();
    protected abstract bool ValidateResult(string code, int result);
    
}




public class EvenOddChallenge : PythonChallenge
{
    private int a, b;

    public override float GetTimeOut()
    {
        return 60f;
    }

    protected override string GetSpecificQuestionText()
    {
        return "Two variables, a and b, are declared; one holds an even value and the other an odd value. "
                    + "Write Python code to place the even value into the variable 'increment' (which is already declared)."
                    + "If correct, your attack will increase by 'increment'!";
    }

    protected override string SetupVariables()
    {
        a = Random.Range(1, 5)*2;
        b = Random.Range(1, 5)*2 + 1;

        return $"a = {a}\n" +
               $"b = {b}\n";
    }

    protected override bool ValidateResult(string code, int result)
    {
        int even = 0;
        if (a % 2 == 0)
            even = a;
        else 
            even = b;

        return result == even;
    }
}





public class PercentageChallenge : PythonChallenge
{
    private int x;

    public override float GetTimeOut()
    {
        return 60f;
    }

    protected override string GetSpecificQuestionText()
    {
        return "A variable 'x' is declared. Write Python code to calculate 20% of x "
                    + "and save it in the variable 'increment'. If correct, "
                    + "your attack will increase by the calculated amount!";
    }

    protected override string SetupVariables()
    {
        x = 50;
        return $"x = {x}\n";
    }

    protected override bool ValidateResult(string code, int result)
    {
        return result == x*20/100;
    }
}






public class SumChallenge : PythonChallenge
{
    private int n;

    public override float GetTimeOut()
    {
        return 60f;
    }

    protected override string GetSpecificQuestionText()
    {
        return "A variable 'n' is declared. Write Python code to calculate the sum "
                    + "of all numbers from 1 to 'n' and store it in the variable 'increment'. If correct, "
                    + "your attack will increase by the number found!";
    }

    protected override string SetupVariables()
    {
        n = Random.Range(4, 7);
        return $"n = {n}\n";
    }

    protected override bool ValidateResult(string code, int result)
    {
        return result == n*(n+1)/2;
    }
}






public class SumListChallenge : PythonChallenge
{
    private List<int> list;

    public override float GetTimeOut()
    {
        return 90f;
    }

    protected override string GetSpecificQuestionText()
    {
        return "A list of numbers named 'list' has been declared and instantiated. "
                    + "Write Python code to calculate the sum of all the numbers in this list and "
                    + "save it in the variable 'increment'. Your attack will increase by the number found! "
                    + "Note: do not use the sum() function.";
    }

    protected override string SetupVariables()
    {
        list = new List<int>();
        int listLength = 10;

        for (int i = 0; i < listLength; i++)
        {
            list.Add(Random.Range(0, 4));
        }

        string listAsPythonArray = "[" + string.Join(", ", list) + "]";
        return $"list = {listAsPythonArray}\n";
    }

    protected override bool ValidateResult(string code, int result)
    {
        int expectedSum = 0;
        foreach (int num in list)
        {
            expectedSum += num;
        }

        return result == expectedSum && !code.Contains("sum");
    }
}





public class ProductListChallenge : PythonChallenge
{
    private List<int> list;

    public override float GetTimeOut()
    {
        return 90f;
    }

    protected override string GetSpecificQuestionText()
    {
        return "A list of numbers named 'list' has been declared and instantiated. "
                    + "Write Python code to calculate the product of all the numbers in this list and "
                    + "save it in the variable 'increment'. Your attack will increase by the number found! "
                    + "Note: do not use the prod() function.";
    }

    protected override string SetupVariables()
    {
        list = new List<int>();
        int listLength = 5;

        for (int i = 0; i < listLength; i++)
        {
            list.Add(Random.Range(1, 8));
        }

        string listAsPythonArray = "[" + string.Join(", ", list) + "]";
        return $"list = {listAsPythonArray}\n";
    }

    protected override bool ValidateResult(string code, int result)
    {
        int expectedProduct = 1;
        foreach (int num in list)
        {
            expectedProduct *= num;
        }

        return result == expectedProduct && !code.Contains("prod");
    }
}





public class MaxListChallenge : PythonChallenge
{
    private List<int> list;

    public override float GetTimeOut()
    {
        return 90f;
    }

    protected override string GetSpecificQuestionText()
    {
        return  "A list of numbers named 'list' has been declared and instantiated. "
                    + "Write Python code to find the maximum value in this list and "
                    + "save it in the variable 'increment'. Your attack will increase by the number found! "
                    + "Note: do not use the max() function.";
    }

    protected override string SetupVariables()
    {
        list = new List<int>();
        int listLength = 20;

        for (int i = 0; i < listLength; i++)
        {
            list.Add(Random.Range(-30, 31));
        }

        string listAsPythonArray = "[" + string.Join(", ", list) + "]";

        return $"list = {listAsPythonArray}\n";

    }

    protected override bool ValidateResult(string code, int result)
    {
        return list.Contains(result) && !code.Contains("max");
    }
}