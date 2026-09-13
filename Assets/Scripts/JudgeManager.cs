using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class JudgeManager : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private JudgeUI judgeUI;
    public Cheese temporaryCheese;
    public CheeseAttempt temporaryCheeseAttempt;

    [Header("Timing")]
    public float startDelay = 1f;
    public float judgeSwitchDelay = 1f;
    public float scoringDelay = 1f;
    public float arrowAnimationDelay = 2f;
    public float dialogDelay = 3f;
    public float judgeEndDelay = 1f;

    void Start()
    {
        //VERY TEMPORARY
        OrderManager.ongoingCheese = temporaryCheese;
        OrderManager.ongoingCheeseAttempt = temporaryCheeseAttempt;


        judgeUI.OpenJudgingScene();
        judgeUI.SetJudgeIndicator(-1); //None
        judgeUI.SetScoringIndicator(-1);

        //Spawn the three cheeses in as the scene starts
        float sceneDelay = startDelay;
        ExecuteAfterTime(startDelay, () => judgeUI.TurnCheesesOn());

        //Move through the judges, first moving the indicator, then the rating wheel, then the cheese nibbling,
        for (int judgeIdx = 0; judgeIdx < 3; judgeIdx++)
        {
            sceneDelay += judgeSwitchDelay;

            int judgeNumber = judgeIdx;
            //Move the arrow
            ExecuteAfterTime(
                sceneDelay,
                () => judgeUI.SetJudgeIndicator(judgeNumber)
            );

            //Move the block with its arrow.
            //Reset the arrow to its centred position
            judgeUI.SetArrowPosition(0);
            sceneDelay += scoringDelay;
            ExecuteAfterTime(
                sceneDelay, 
                ()=>judgeUI.SetScoringIndicator(judgeNumber)
            );

            //Calculate score difference 
            int scoreDifference = GetDifference(judgeNumber);

            // Delay setting the actual feedback data
            ExecuteAfterTime(sceneDelay, () => SetFeedback(judgeNumber));

            sceneDelay += arrowAnimationDelay;
            ExecuteAfterTime(
                sceneDelay, 
                ()=>judgeUI.AnimateArrow(scoreDifference)
            );

            sceneDelay += dialogDelay;
            ExecuteAfterTime(
                sceneDelay, 
                ()=>judgeUI.SetYourAttempt()
            );

            sceneDelay += judgeEndDelay;           
            

            //Make a judgement, do dialog and move the arrow (go up and down then settle in the spot)
            
        }
        //Then move through dialog, set the rating, and add the feedback to the board


     //   ExecuteAfterTime(2, )
    }

    public void SetFeedback(int category)
    {
        if(category==0){
        OrderManager.ongoingCheeseAttempt.saltFeedback =
            GetFeedback(0);
        }

        if(category==1){
        OrderManager.ongoingCheeseAttempt.cultureFeedback =
            GetFeedback(1);
        }

        if(category==2){
        OrderManager.ongoingCheeseAttempt.ageFeedback =
            GetFeedback(2);
        }
    }

    public int GetDifference(int category)
    {
        switch (category)
        {
            case 0:
                return OrderManager.ongoingCheeseAttempt.amountOfSalt
                    - OrderManager.ongoingCheese.saltiness;

            case 1:
                return OrderManager.ongoingCheeseAttempt.amountOfCulture
                    - OrderManager.ongoingCheese.stinkiness;

            case 2:
                return OrderManager.ongoingCheeseAttempt.amountOfAge
                    - OrderManager.ongoingCheese.dryness;

            default:
                return 0;
        }
    }

    public string GetFeedback(int category)
    {
        int difference = GetDifference(category);

        if (difference == 0)
            return "Perfect";

        if (difference < 0)
        {
            switch (category)
            {
                case 0:
                    return "Not salty enough";

                case 1:
                    return "Not stinky enough";

                case 2:
                    return "Not dry enough";
            }
        }
        else
        {
            switch (category)
            {
                case 0:
                    return "Too salty";

                case 1:
                    return "Too stinky";

                case 2:
                    return "Too dry";
            }
        }

        return "";
    }

    public void ExecuteAfterTime(float time, Action action)
    {
        StartCoroutine(ExecuteAfterTimeCoroutine(time, action));
    }

    private IEnumerator ExecuteAfterTimeCoroutine(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
}
