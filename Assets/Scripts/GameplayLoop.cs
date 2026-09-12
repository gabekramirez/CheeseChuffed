using UnityEngine;

public class GameplayLoop : MonoBehaviour
{
    [Header("Script Reference")]
    [SerializeField] private UIController uIController;
    [SerializeField] private OrderManager orderManager;
    public GameplayPhase gameplayPhase;

    void Start()
    {
        //First part of game loop. Initialize the cheese we're using
        gameplayPhase = GameplayPhase.AddMilk;
        OrderManager.ongoingCheese = orderManager.FetchRandomCheese();
        OrderManager.ongoingCheeseAttempt = new CheeseAttempt();

        uIController.SetCurrentOrderUI();
        uIController.RefreshCheeseAttemptsUI();
        uIController.SetPhaseText(gameplayPhase);
    }

    public void AdvanceGamePlayPhase()
    {
        gameplayPhase = GetNextGameplayPhase(gameplayPhase);
        uIController.SetPhaseText(gameplayPhase);
    }

    GameplayPhase GetNextGameplayPhase(GameplayPhase phase)
    {
        if (phase == GameplayPhase.Present)
            return GameplayPhase.Present;

        return (GameplayPhase)((int)phase + 1);
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