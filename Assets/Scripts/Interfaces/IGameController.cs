

public interface IGameController
{
    delegate void UIGameAction();
    event UIGameAction OnStartGame;
    event UIGameAction OnStartMove;
    event UIGameAction OnStopMove;
    event UIGameAction OnPauseGame;
    event UIGameAction OnEndGame;

    void StartGame();
    
}
