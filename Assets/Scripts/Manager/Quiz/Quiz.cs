using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Quiz
{
    int quizNumber;
    string quizSentence;
    string quizType;
    List<string> optionsList = new List<string>();
    List<int> correctIndexList = new List<int>();

    public bool IsCorrect { get; set; }
    public bool IsAnswered { get; set; }

    public float TimeLimit { get; set; }

    public Quiz(int quizNumber, string quizType, string quizSentence, List<string> optionsList, List<int> correctIndexList) { 
        this.quizNumber = quizNumber;
        this.quizType = quizType;
        this.quizSentence = quizSentence;
        this.optionsList = optionsList;
        this.correctIndexList = correctIndexList;
    }

    public int GetQuizNumber() { 
        return quizNumber;
    }

    public void SetQuizNumber(int quizNumber)
    {
        this.quizNumber = quizNumber;
    }

    public string GetQuizType()
    {
        return quizType;
    }

    public void SetQuizType(string quizType)
    {
        this.quizType = quizType;
    }

    public string GetQuizSentence()
    {
        return quizSentence;
    }

    public void SetQuizSentence(string quizSentence)
    {
        this.quizSentence = quizSentence;
    }

    public List<string> GetOptionsList() {
        return optionsList;
    }

    public void SetOptionsList(List<string> optionsList)
    {
        this.optionsList = optionsList;
    }

    public List<int> GetCorrectIndexList()
    {
        return correctIndexList;
    }

    public void SetCorrectIndexList(List<int> correctIndexList)
    {
        this.correctIndexList = correctIndexList;
    }
}
