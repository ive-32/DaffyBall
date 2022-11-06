using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour, IGameController
{
    // тут нарушим паттерн MVP - совместим Presener и Viewer
    enum GameStates { BeforeStart, InGame, Paused, BeforeEnd, OnUIScreen }
    const float TimeToInceaseSpeed = 15;


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
    public float vGameSpeed { get; set; }
    public float ElapsedGameTime { get; set; }
    public Vector2Int FieldSize { get; set; }
    
    private GameStates gameState;
    private float startGameTime;
    private float startPauseTime;
    private float pausedTime;
    private int travelledDistanse;
    float lastVeticalVelosityIncreaseTime;

    Ball ball;

    private void Awake()
    {
        // определим половину размера экрана в единицах Unity, прибавим по 2 тайла по горизонтали, чтобы генерились за пределами экрана
        // по вертикали отнимем по тайлу чтобы, не попасть в камеру смартфона и закруленные углы экрана
        Camera _camera = Camera.main;
        Vector3 afieldSize = _camera.ViewportToWorldPoint(Vector2.one);
        FieldSize = Vector2Int.RoundToInt(afieldSize) + new Vector2Int(2, -1);

        GameSpeed = 2.0f;
        gameState = GameStates.OnUIScreen;
        ShowScreen(StartScreenPrefab);
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
        ElapsedGameTime = 0;
        lastVeticalVelosityIncreaseTime = 0;
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
        GameObject _ballObject = Instantiate(BallPrefab, new Vector3(- FieldSize.x / 3,  0, 0), Quaternion.identity);
        _ballObject.TryGetComponent<Ball>(out ball);
        if (ball == null)
            throw new System.Exception("BallPrefab not contains class Ball");
        ball.GameController = this;
        pausedTime = 0;
        gameState = GameStates.BeforeStart;
        vGameSpeed = GameSpeed * 2;
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
        // убираем игровой текст 
        foreach (Transform child in UILayer.transform)
            Destroy(child.gameObject);
        // показываем тест 
        gameState = GameStates.BeforeEnd;
        OnStopMove?.Invoke();
        // для проверки. В целом не важно - игрок с секундомером сидеть не будет. Но хочется, чтобы совпали методы вычисления времени.
        if (ElapsedGameTime - (Time.time - startGameTime - pausedTime) > 0.5f)
            Debug.LogWarning($"Time counter is wrong. By Update counter: {ElapsedGameTime}, start-stop counter: {Time.time - startGameTime - pausedTime}");
        ElapsedGameTime = Time.time - startGameTime - pausedTime;
        StartCoroutine(ShowTextAtTheEnd());
    }

    public string GetGameStatistics()
    {
        int totalGamesCount = PlayerPrefs.GetInt("TotalGamesCount", 0);
        travelledDistanse = Mathf.FloorToInt(ElapsedGameTime * GameSpeed);
        return $"Время {ElapsedGameTime.ToString("0.0")} с\nРасстояние {travelledDistanse}\nВсего игр {totalGamesCount}";
    }

    private void Update()
    {
        if (gameState == GameStates.InGame) ElapsedGameTime += Time.deltaTime;
        if (ElapsedGameTime - lastVeticalVelosityIncreaseTime > TimeToInceaseSpeed)
        {
            lastVeticalVelosityIncreaseTime = ElapsedGameTime;
            vGameSpeed = GameSpeed * 2 + Mathf.RoundToInt(ElapsedGameTime / TimeToInceaseSpeed) * GameSpeed * 0.5f;
            CreateSplashText("+15 с. увеличиваем скорость!", 1f);
        }
        // глючит нужно отлаживать еще 
        /*if (gameState == GameStates.BeforeEnd && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            StopCoroutine(ShowTextAtTheEnd());
            EndGame();
        }*/
    }
}
