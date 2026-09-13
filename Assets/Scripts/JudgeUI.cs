using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    [SerializeField] private TMP_Text yourIngredientsText;
    [SerializeField] private TMP_Text yourFeedbackText;

    [Header("Judging Process")]
    [SerializeField] private GameObject currentlyJudgingIndicator;
    public float judgeSpacing = 4f;
    private Vector3 judgeIndicatorStartPosition;

    [Header("Scoring Indicator")]
    [SerializeField] private GameObject scoringBlockObject;
    [SerializeField] private GameObject scoringIndicatorObject;
    public float scoringScale = 1f;
    private float scoreIndicatorStartLocalX;
    private Vector3 scoreBlockStartPosition;

    [Header("Table Cheeses")]
    public List<GameObject> tableCheese;
    public float cheeseTiming = 1f;


    private void Awake()
    {
        judgeIndicatorStartPosition =
            currentlyJudgingIndicator.transform.position;

        // Store the arrow's starting local X.
        scoreIndicatorStartLocalX =
            scoringIndicatorObject.transform.localPosition.x;

        // Store the scoring block's starting position.
        scoreBlockStartPosition =
            scoringBlockObject.transform.position;

    }


    public void OpenJudgingScene()
    {
        recievedCheeseNameTXT.text = OrderManager.ongoingCheese.name;

        recievedCheeseDescriptionTXT.text =
            OrderManager.ongoingCheese.description;


        SetYourAttempt();

        // Set the plate cheeses off initially then turn on.
        foreach (GameObject cheese in tableCheese)
        {
            cheese.SetActive(false);
        }
    }


    public void TurnCheesesOn()
    {
        ExecuteAfterTime(
            0.3f * cheeseTiming,
            () => tableCheese[0].SetActive(true)
        );

        ExecuteAfterTime(
            0.6f * cheeseTiming,
            () => tableCheese[1].SetActive(true)
        );

        ExecuteAfterTime(
            0.9f * cheeseTiming,
            () => tableCheese[2].SetActive(true)
        );
    }


    public void SetYourAttempt()
    {
        string saltAmount =
            OrderManager.ongoingCheeseAttempt.amountOfSalt.ToString("N0");

        string cultureAmount =
            OrderManager.ongoingCheeseAttempt.amountOfCulture.ToString("N0");

        string agingAmount =
            OrderManager.ongoingCheeseAttempt.amountOfAge.ToString("N0");

        yourIngredientsText.text =
            $"{saltAmount}\n{cultureAmount}\n{agingAmount}\n";

        Debug.Log(OrderManager.ongoingCheeseAttempt.saltFeedback);
        yourFeedbackText.text =
            $"{OrderManager.ongoingCheeseAttempt.saltFeedback}\n" +
            $"{OrderManager.ongoingCheeseAttempt.cultureFeedback}\n" +
            $"{OrderManager.ongoingCheeseAttempt.ageFeedback}";

    }


    public void SetJudgeIndicator(int judgeNumber)
    {
        SetIndicator(
            currentlyJudgingIndicator,
            judgeIndicatorStartPosition,
            judgeSpacing,
            judgeNumber
        );
    }


    public void SetScoringIndicator(int judgeNumber)
    {
        SetIndicator(
            scoringBlockObject,
            scoreBlockStartPosition,
            judgeSpacing,
            judgeNumber
        );

        // Reset the arrow to the center of the scoring block.
        SetArrowPosition(0f);
    }


    private void SetIndicator(
        GameObject indicator,
        Vector3 startPosition,
        float spacing,
        int judgeNumber)
    {
        Fade fade = indicator.GetComponent<Fade>();

        if (judgeNumber == -1)
        {
            fade.SetAlpha(0);
            indicator.SetActive(false);
            return;
        }

        indicator.SetActive(true);
        fade.SetAlpha(0);

        Vector3 position = startPosition;
        position.x += spacing * judgeNumber;

        indicator.transform.position = position;

        fade.FadeIn();
    }


    public void SetArrowPosition(float position)
    {
        position = Mathf.Clamp(position, -5f, 5f);

        Vector3 localPosition =
            scoringIndicatorObject.transform.localPosition;

        localPosition.x = scoreIndicatorStartLocalX + position;

        scoringIndicatorObject.transform.localPosition =
            localPosition;
    }


    public void AnimateArrow(float finalPosition)
    {
        StartCoroutine(
            AnimateArrowCoroutine(finalPosition)
        );
    }


    private IEnumerator AnimateArrowCoroutine(float finalPosition)
    {
        finalPosition = Mathf.Clamp(
            finalPosition * scoringScale,
            -5f * scoringScale,
            5f * scoringScale
        );

        // Start at the center.
        SetArrowPosition(0f);

        // Move to +5.
        yield return MoveArrow(5f * scoringScale);

        // Pause.
        yield return new WaitForSeconds(0.5f);

        // Move to -5.
        yield return MoveArrow(-5f * scoringScale);

        // Move to the actual final position.
        yield return MoveArrow(finalPosition);
    }


    private IEnumerator MoveArrow(float targetPosition)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        float startX =
            scoringIndicatorObject.transform.localPosition.x;

        float targetX =
            scoreIndicatorStartLocalX + targetPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            float currentX =
                Mathf.Lerp(startX, targetX, t);

            Vector3 localPosition =
                scoringIndicatorObject.transform.localPosition;

            localPosition.x = currentX;

            scoringIndicatorObject.transform.localPosition =
                localPosition;

            yield return null;
        }

        // Snap to the exact final X.
        Vector3 finalLocalPosition =
            scoringIndicatorObject.transform.localPosition;

        finalLocalPosition.x = targetX;

        scoringIndicatorObject.transform.localPosition =
            finalLocalPosition;
    }


    public void OpenSuccessPanel()
    {
        successPanel.SetActive(true);
        requestTXT.text = "";
    }


    public void AcceptNewCheese()
    {
        // Go back to the kitchen with a new cheese.
    }


    public void EndGame()
    {
        // Pull up final results panel.
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


    public void OpenFailurePanel()
    {
        failurePanel.SetActive(true);
        feedbackTXT.text = "";
    }


    public void ReturnToTryAgain()
    {
        SceneManager.LoadScene("CheeseMaker");
    }


    public void ExecuteAfterTime(float time, Action action)
    {
        StartCoroutine(
            ExecuteAfterTimeCoroutine(time, action)
        );
    }


    private IEnumerator ExecuteAfterTimeCoroutine(
        float time,
        Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
}