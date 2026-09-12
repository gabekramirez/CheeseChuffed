using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JudgeUI : MonoBehaviour
{
    [Header("Success Panel")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private TMP_Text requestTXT;

    [Header("Failure Panel")]
    [SerializeField] private GameObject failurePanel;
    [SerializeField] private TMP_Text feedbackTXT;

    [Header("Final Results Panel")]
    [SerializeField] private GameObject finalResultsPanel;
    [SerializeField] private TMP_Text cheesesMadeTXT;
    [SerializeField] private TMP_Text averageAttemptsTXT;
    [SerializeField] private TMP_Text timeTakenTXT;

    [Header("Order Recieved")]
    [SerializeField] private TMP_Text recievedCheeseNameTXT;
    [SerializeField] private TMP_Text recievedCheeseDescriptionTXT;

    [Header("Order Recieved")]
    [SerializeField] private TMP_Text yourSaltAmountTXT;
    [SerializeField] private TMP_Text yourCultureAmountTXT;
    [SerializeField] private TMP_Text yourAgingAmountTXT;
    [SerializeField] private TMP_Text yourSaltFeedbackTXT;
    [SerializeField] private TMP_Text yourCultureFeedbackTXT;
    [SerializeField] private TMP_Text yourAgingFeedbackTXT;

    [Header("Judging Process")]
    [SerializeField] private TMP_Text dialogTXT;
    [SerializeField] private Transform currentlyJudgingIndicator;
    public List<Transform> judgeLeadingTransforms;


    public void OpenJudgingScene()
    {
        recievedCheeseNameTXT.text = OrderManager.ongoingCheese.name;
        recievedCheeseDescriptionTXT.text = OrderManager.ongoingCheese.description;
        
        SetYourAttempt();
        dialogTXT.text = "";
    }

    public void SetYourAttempt()
    {
        //Your amounts
        yourSaltAmountTXT.text = OrderManager.ongoingCheeseAttempt.amountOfSalt.ToString("N0");
        yourCultureAmountTXT.text = OrderManager.ongoingCheeseAttempt.amountOfCulture.ToString("N0");
        yourAgingAmountTXT.text = OrderManager.ongoingCheeseAttempt.amountOfAge.ToString("N0");

        //Feedback
        yourSaltFeedbackTXT.text = OrderManager.ongoingCheeseAttempt.saltFeedback;
        yourSaltFeedbackTXT.text = OrderManager.ongoingCheeseAttempt.cultureFeedback;
        yourSaltFeedbackTXT.text = OrderManager.ongoingCheeseAttempt.ageFeedback;
    }


    public void OpenSuccessPanel()
    {
        successPanel.SetActive(true);
        requestTXT.text = "";
    }

    public void AcceptNewCheese()
    {
       //Go back to the kitchen with a new cheese   
    }

    public void EndGame() //Retire
    {
        //Pull up final results panel
        finalResultsPanel.SetActive(true);
        cheesesMadeTXT.text = "";
        averageAttemptsTXT.text = "";
        timeTakenTXT.text = "";
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        GameplayLoop.ResetCheeseData();
        SceneManager.LoadScene("CheeseMaker");
    }


    //Now the failure methods
    public void OpenFailurePanel()
    {
        failurePanel.SetActive(true);
        feedbackTXT.text = "";
    }

    public void ReturnToTryAgain()
    {
        SceneManager.LoadScene("CheeseMaker");
    }
}
