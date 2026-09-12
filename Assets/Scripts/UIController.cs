using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Script Reference")]
    [SerializeField] private OrderManager orderManager;

    [Header("Current Order UI")]
    [SerializeField] private TMP_Text cheeseOrderedTXT;
    [SerializeField] private TMP_Text descriptionOrderedTXT;

    [Header("Previous Orders UI")]
    [SerializeField] private Transform ordersParent;
    [SerializeField] private GameObject orderMinimizedPrefab;
    [SerializeField] private GameObject orderMaximizedPrefab;

    public void SetCurrentOrderUI()
    {
        cheeseOrderedTXT.text = orderManager.ongoingCheese.name;
        descriptionOrderedTXT.text = $"\"{orderManager.ongoingCheese.description}\"";
    }

    public void RefreshCheeseAttemptsUI()
    {
        // Clear the UI
        DestroyAllChildren(ordersParent);

        int totalAttempts = orderManager.previousCheeseAttempts.Count;

        if(totalAttempts == 0) return;

        //Go through in reverse order so latest is at the top
        for (int i = totalAttempts - 1; i >= 0; i--)
        {
            CheeseAttempt cheeseAttempt = orderManager.previousCheeseAttempts[i];

            //Minimized
            GameObject newMinimizedAttemptObject = Instantiate(orderMinimizedPrefab);
            newMinimizedAttemptObject.transform.SetParent(ordersParent);
            newMinimizedAttemptObject.transform.localScale = new Vector3(1f, 1f, 1f);

            // Set texts and onClicks
            newMinimizedAttemptObject.transform.Find("HeaderTXT").GetComponent<TMP_Text>().text = GetAttemptName(i, totalAttempts);
            newMinimizedAttemptObject.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => OpenCloseCheeseAttempt(i));

            //Maximized
            GameObject newMaximizedAttempt = Instantiate(orderMaximizedPrefab);
            newMaximizedAttempt.transform.SetParent(ordersParent);
            newMaximizedAttempt.transform.localScale = new Vector3(1f, 1f, 1f);

            //Set texts and onClicks
            newMinimizedAttemptObject.transform.Find("Top/HeaderTXT").GetComponent<TMP_Text>().text = GetAttemptName(i, totalAttempts);
            newMinimizedAttemptObject.transform.Find("Top/Button").GetComponent<Button>().onClick.AddListener(() => OpenCloseCheeseAttempt(i));

                //Salt
            newMinimizedAttemptObject.transform.Find("Bottom/Ingredients/Salt/NumericalTXT").GetComponent<TMP_Text>().text = 
            cheeseAttempt.amountOfSalt.ToString();
            newMinimizedAttemptObject.transform.Find("Bottom/Feedback/Salt/FeedbackTXT").GetComponent<TMP_Text>().text = 
            cheeseAttempt.saltFeedback;
                
                //Stink
            newMinimizedAttemptObject.transform.Find("Bottom/Ingredients/Culture/NumericalTXT").GetComponent<TMP_Text>().text = 
            cheeseAttempt.amountOfCulture.ToString();
            newMinimizedAttemptObject.transform.Find("Bottom/Feedback/Stink/FeedbackTXT").GetComponent<TMP_Text>().text = 
            cheeseAttempt.cultureFeedback;
              
                //Age
            newMinimizedAttemptObject.transform.Find("Bottom/Ingredients/Aging/NumericalTXT").GetComponent<TMP_Text>().text = 
            cheeseAttempt.amountOfAge.ToString();
            newMinimizedAttemptObject.transform.Find("Bottom/Feedback/Dryness/FeedbackTXT").GetComponent<TMP_Text>().text = 
            cheeseAttempt.ageFeedback;


        }
    }

    private string GetAttemptName(int index, int totalAttempts)
    {
        if (index == totalAttempts - 1)
            return "Latest";

        int attemptNumber = totalAttempts - index;

        string suffix = "th";

        if (attemptNumber % 100 < 11 || attemptNumber % 100 > 13)
        {
            switch (attemptNumber % 10)
            {
                case 1:
                    suffix = "st";
                    break;
                case 2:
                    suffix = "nd";
                    break;
                case 3:
                    suffix = "rd";
                    break;
            }
        }

        return $"{attemptNumber}{suffix} Attempt";
    }

    public void OpenCloseCheeseAttempt(int index)
    {
        Transform attempt = ordersParent.GetChild(index);

        Transform minimized = attempt.Find("Minimized");
        Transform maximized = attempt.Find("Maximized");

        minimized.gameObject.SetActive(!minimized.gameObject.activeSelf);
        maximized.gameObject.SetActive(!maximized.gameObject.activeSelf);
    }

    // Helper method to recursively delete all the children of a transform
    public void DestroyAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            DestroyAllChildren(child);
            Destroy(child.gameObject);
        }
    }
}
