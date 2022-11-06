using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour, IGameController
{
    // тут нарушим паттерн MVP - совместим Presener и Viewer
    enum GameStates { BeforeStart, InGame, Paused, BeforeEnd, OnUIScreen }

    public event IGameController.UIGameAction OnStartGame;
    public event IGameController.UIGameAction OnStartMove;
    public event IGameController.UIGameAction OnStopMove;
    public event IGameController.UIGameAction OnPauseGame;
    public event IGameController.UIGameAction OnEndGame;
    public GameObject StartScreenPrefab;
    public GameObject GameOverScreenPrefab;
    public GameObject BallPrefab;
    public GameObject SplashTextPrefab;
    public GameObject UILayer; // для вывода текста 
    public float GameSpeed { get; set; }
    private GameStates gameState;
    private float startGameTime;
    private float startPauseTime;
    private float pausedTime;
    private float totalGameTime;
    private int travelledDistanse;

    Ball ball;

    private void Awake()
    {
        ShowScreen(StartScreenPrefab);
        GameSpeed = 2.0f;
        gameState = GameStates.OnUIScreen;
    }
    IUIScreen ShowScreen(GameObject ScreenPrefab)
    {
        GameObject _currentScreen = Instantiate(ScreenPrefab, this.transform);
        _currentScreen.TryGetComponent<IUIScreen>(out IUIScreen uiscreen);
        if (uiscreen is IUIScreen)
            uiscreen.GameController = this;
        gameState = GameStates.OnUIScreen;
        return uiscreen;
    }

    void CreateSplashText(string atext, float atimetodestroy)
    {   // пишем текст на экране - исчезает через заданное время
        GameObject _spashtext = Instantiate(SplashTextPrefab, UILayer.transform);
        TMPro.TextMeshProUGUI _tmpgui;
        _spashtext.TryGetComponent<TMPro.TextMeshProUGUI>(out _tmpgui);
        if (_tmpgui != null)
        {
            _tmpgui.text = atext;
            Destroy(_spashtext, atimetodestroy);
        }
        else
            Destroy(_spashtext);
    }

    IEnumerator ShowTextBeforeStart()
    {   // показываем отсчет перед стартом
        float TimeToShowText = 1f;
        CreateSplashText("Ready?", TimeToShowText);
        yield return new WaitForSeconds(TimeToShowText);
        for (int i = 3; i > 0; i--)
        {
            CreateSplashText($"Start in {i}", TimeToShowText);
            yield return new WaitForSeconds(TimeToShowText);
        }
        CreateSplashText($"Go!", TimeToShowText / 3f);
        startGameTime = Time.time;
        int totalGamesCount = PlayerPrefs.GetInt("TotalGamesCount", 0) + 1;
        PlayerPrefs.SetInt("TotalGamesCount", totalGamesCount);
        gameState = GameStates.InGame;
        OnStartMove?.Invoke();
    }

    IEnumerator ShowTextAtTheEnd()
    {   // показываем что шарик лопнул 
        // пишем что игра окончена
        float TimeToShowText = 2f;
        CreateSplashText("Игра окончена", TimeToShowText);
        yield return new WaitForSeconds(TimeToShowText);
        EndGame();
    }

    public void StartGame()
    {   // стартуем игру 
        // создаем шарик запускаем обратный отсчет
        GameObject _ballObject = Instantiate(BallPrefab, Vector3.zero, Quaternion.identity);
        _ballObject.TryGetComponent<Ball>(out ball);
        if (ball == null)
            throw new System.Exception("BallPrefab not contains class Ball");
        ball.GameController = this;
        pausedTime = 0;
        gameState = GameStates.BeforeStart;
        OnStartGame?.Invoke();
        StartCoroutine(ShowTextBeforeStart());
    }
    public void EndGame()
    {
        foreach (Transform child in UILayer.transform)
            Destroy(child.gameObject);
        OnEndGame?.Invoke();
        IUIScreen uiscreen = ShowScreen(GameOverScreenPrefab);
        uiscreen.SetText(GetGameStatistics());
        gameState = GameStates.OnUIScreen;
    }
    public void BallBursted()
    {   // шарик лопнул, останавливаем скролл
        // показываем тест 
        gameState = GameStates.BeforeEnd;
        OnStopMove?.Invoke();
        totalGameTime = Time.time - startGameTime - pausedTime;
        travelledDistanse = Mathf.FloorToInt(totalGameTime * GameSpeed);
        StartCoroutine(ShowTextAtTheEnd());
    }

    public string GetGameStatistics()
    {
        int totalGamesCount = PlayerPrefs.GetInt("TotalGamesCount", 0);
        return $"Время {totalGameTime.ToString("0.0")} с\nРасстояние {travelledDistanse}\nВсего игр {totalGamesCount}";
    }

    private void Update()
    {
        // глючит нужно отлаживать еще 
        /*if (gameState == GameStates.BeforeEnd && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            StopCoroutine(ShowTextAtTheEnd());
            EndGame();
        }*/
    }
}
