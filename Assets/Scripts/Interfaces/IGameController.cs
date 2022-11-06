

public interface IGameController
{
    delegate void UIGameAction();
    event UIGameAction OnStartGame;
    event UIGameAction OnStartMove;
    event UIGameAction OnStopMove;
    event UIGameAction OnPauseGame;
    event UIGameAction OnEndGame;

    float GameSpeed { get; set; }
    public float vGameSpeed { get; set; }
    public float ElapsedGameTime { get; set; }
    void StartGame();
    void BallBursted();
    string GetGameStatistics();
    public UnityEngine.Vector2Int FieldSize { get; set; }
}
