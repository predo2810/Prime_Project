using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CalcEngine;
using System;

public class PrimeCalculator : MonoBehaviour
{
    public TMPro.TMP_Text FunctionText;
    public TMPro.TMP_Text PrimeCounter;
    public TMPro.TMP_Text CurrentNumber;
    public TMPro.TMP_Text ExpressionText;

    public TMPro.TMP_InputField LoopInput;
    public TMPro.TMP_InputField TimeInput;

    public bool isPrime;

    public int TimesToDo = 50;
    public int LastPrime;

    public float TimeToWait = 0.4f;

    public GameObject[] ObjectsToHide;
    public GameObject[] ObjectsToShow;

    public GameObject StartButt, RestartButt;

    public string Expression;
    public string Formula;

    public List<string> Operators = new List<string> {"+", "-", "*", "/", "^"};

    void Start()
    {
        FunctionText.text = "Fórmula: " + Expression;

        TimesToDo = PlayerPrefs.GetInt("Loops", 50);
        TimeToWait = PlayerPrefs.GetFloat("TimeToWait", 0.4f);

        TimeInput.text = TimeToWait.ToString();
        LoopInput.text = TimesToDo.ToString();
    }

    public void SetTime(string time)
    {
        if (time != null)
        {
            TimeToWait = float.Parse(time);
        }
        else
        {
            TimeToWait = 0.4f;
        }

        PlayerPrefs.SetFloat("TimeToWait", TimeToWait);
        PlayerPrefs.Save();
    }

    public void SetLoop(string loop)
    {
        if (loop != null)
        {
            TimesToDo = int.Parse(loop);
        }
        else
        {
            TimesToDo = 50;
        }

        PlayerPrefs.SetInt("Loops", TimesToDo);
        PlayerPrefs.Save();
    }

    public void StartCalculation()
    {
        AcceptExpression();
        StartButt.SetActive(false);
        StartCoroutine(PrimeMaster());


        foreach (GameObject GO in ObjectsToHide)
        {
            GO.SetActive(false);
        }

        foreach (GameObject GO in ObjectsToShow)
        {
            GO.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    bool VerifyPrime(int Prime)
    {
        bool Primey = true;

        for (int i = 2; i <= Mathf.Sqrt(Prime); i++)
        {
            if (Prime % i == 0)
            {
                Primey = false;
                break;
            }
            else
            {
                Primey = true;
            }
        }

        if (Primey)
        {
            LastPrime = Prime;
        }

        return Primey;
    }

    int CalcPrime(int N)
    {
        Formula = "";

        foreach (char c in Expression)
        {
            // if (c == '!' && Formula.Length - 1 <= 0)
            // {
            //     Debug.LogError("ERROR.\nINVALID FACTORIAL POSITION.");
            //     CurrentNumber.text = "Erro.\nPosição inválida de fatorial(!).";
            //     RestartButt.SetActive(true);
            // }
            // else
            {
                Debug.Log(c);
                if (c == 'n')
                {
                    if (Formula.Length - 1 >= 0)
                    {
                        if (Operators.Contains(Formula[Formula.Length - 1].ToString()))
                        {
                            Formula += N.ToString();
                        }
                        else
                        {
                            Formula += "*";
                            Formula += N.ToString();
                            Debug.Log("Adding Multiplication Before N.\nFormula: " + Formula);
                        }
                    }
                    else
                    {
                        Formula += N.ToString();
                        Debug.Log("Before Formula is null");
                    }
                }
                else if (c == '!')
                {
                    Debug.Log("Factorial");
                    Debug.Log(Formula[Formula.Length - 1].ToString());
                    int F = int.Parse(Formula[Formula.Length - 1].ToString());
                    Formula = Formula.Remove(Formula.Length - 1);
                    Formula += CalcFactorial(F);
                }
                else
                {
                    Formula += c;
                }
            }
        }

        var ce = new CalcEngine.CalcEngine();
        Debug.Log(Formula);
        var x = ce.Parse(Formula);
        Debug.Log(x);
        var value = x.Evaluate();
        Debug.Log(value);

        return int.Parse(value.ToString());
    }

    int CalcFactorial(int InitialValue)
    {
        int Fact = InitialValue;

        for (int i = 1; i < InitialValue; i++)
        {
            Fact *= i;
        }

        return Fact;
    }

    IEnumerator PrimeMaster()
    {
        int TimesDone = 0;

        for (int i = 1; i <= TimesToDo; i++)
        {
            TimesDone = i;
            yield return new WaitForSeconds(TimeToWait);

            if (isPrime)
            {
                yield return new WaitForSeconds(TimeToWait);
                int Prime = CalcPrime(i);
                
                isPrime = VerifyPrime(Prime);

                if (isPrime)
                {
                    CurrentNumber.text = Prime.ToString();
                    PrimeCounter.text = i.ToString();
                }
            }
            else
            {
                CurrentNumber.text = "Último Primo: " + LastPrime.ToString();
                RestartButt.SetActive(true);
                break;
            }
        }

        if (TimesDone >= TimesToDo)
        {
            CurrentNumber.text = "Fim do Cálculo.\nÚltimo Primo: " + LastPrime.ToString();
            RestartButt.SetActive(true);
        }
    }

    public void AddNumber(string Number)
    {
        Expression += Number;
        ExpressionText.text = Expression;
    }

    public void RemoveNumber()
    {
        if (Expression.Length > 0)
        {
            Expression = Expression.Remove(Expression.Length - 1);
            ExpressionText.text = Expression;
        }
    }

    public void AcceptExpression()
    {
        if (Expression == null || Expression == "" || Expression == " ")
        {
            Expression = "n+n*n";
        }
        else
        {
            FunctionText.text = "Fórmula: " + Expression;
        }
    }
}
