using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayLoop : MonoBehaviour
{
    [Header("Script Reference")]
    [SerializeField] private UIController uIController;
    public static OrderManager orderManager;
    GameplayPhase gameplayPhase;

    void Start()
    {
        orderManager = GameObject.Find("ScriptManagers/OrderManager").GetComponent<OrderManager>();
        //First part of game loop. Initialize the cheese we're using
        gameplayPhase = GameplayPhase.AddMilk;
        OrderManager.ongoingCheese = OrderManager.FetchRandomCheese();
        OrderManager.ongoingCheeseAttempt = new CheeseAttempt();

        uIController.SetCurrentOrderUI();
        uIController.RefreshCheeseAttemptsUI();
        uIController.SetPhaseText(gameplayPhase);
    }

    public void AdvanceGamePlayPhase()
    {
        if(gameplayPhase == GameplayPhase.Present)
        {
            SceneManager.LoadScene("Judging");
        }

        gameplayPhase = GetNextGameplayPhase(gameplayPhase);
        uIController.SetPhaseText(gameplayPhase);
    }

    GameplayPhase GetNextGameplayPhase(GameplayPhase phase)
    {
        if (phase == GameplayPhase.Present)
            return GameplayPhase.Present;

        return (GameplayPhase)((int)phase + 1);
    }

    public static void ResetCheeseData()
    {
        OrderManager.usedCheeses.Clear();
        OrderManager.ongoingCheese = null;
        OrderManager.ongoingCheeseAttempt = null;
    }
}


public enum GameplayPhase
{
    AddMilk,
    TurnOnPot,
    AddSalt,
    AddCulture,
    TurnOffPot,
    Age,
    Present
}