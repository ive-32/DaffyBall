using UnityEngine;

public class GameController : MonoBehaviour, IGameController
{
    // тут нарушим паттерн MVP - совместим Presener и Viewer

    public event IGameController.UIGameAction OnStartGame;
    public event IGameController.UIGameAction OnStartMove;
    public event IGameController.UIGameAction OnStopMove;
    public event IGameController.UIGameAction OnPauseGame;
    public event IGameController.UIGameAction OnEndGame;
    public GameObject StartScreenPrefab;

    private void Start()
    {
        GameObject _currentScreen = Instantiate(StartScreenPrefab, this.transform);
        _currentScreen.TryGetComponent<IUIScreen>(out IUIScreen uiscreen);
        if (uiscreen is IUIScreen)
            uiscreen.gameController = this;
    }

    public void StartGame()
    {
        OnStartGame?.Invoke();
        OnStartMove?.Invoke();
    }

}
