using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private IGameController gameController;
    [SerializeField] private GameObject MainSprite;
    [SerializeField] private Sprite[] BurstTextures;
    public IGameController GameController 
    { 
        get => gameController;
        set 
        {   // при задании контроллера привязываем события 
            gameController = value;
            gameController.OnStartGame += StartGame;
            gameController.OnStartMove += StartMove;
            gameController.OnStopMove += StopMove;
        } 
    }
    bool MoveAvailable = false;

    void StartGame()
    {
        MainSprite.SetActive(false);
    }
    void StartMove()
    {
        MainSprite.SetActive(true);
        MoveAvailable = true;
    }
    void StopMove() => MoveAvailable = false;

    IEnumerator BurstBall()
    {   // лопаем шарик - анимация из спрайтов 
        MoveAvailable = false;
        for (int i = 0; i < BurstTextures.Length; i++)
        {
            MainSprite.GetComponent<SpriteRenderer>().sprite = BurstTextures[i];
            yield return new WaitForSeconds(0.01f);
        }
        // сообщаем контроллеру что лопнул
        // отписываемся от событий 
        // удаляем объект
        gameController.BallBursted();
        gameController.OnStartGame -= StartGame;
        gameController.OnStartMove -= StartMove;
        gameController.OnStopMove -= StopMove;
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!MoveAvailable) return; // чтобы событие не генерилось пока идет анимация лопания шара
        StartCoroutine(BurstBall());
    }


    void Update()
    {
        
        // для хорошего MVP нужно чтобы события обрабатывались где-нибудь в презенторе - но тут одно событие ловлю ввод пользователя тут
        if (!MoveAvailable) return;
        

        if (Input.touchCount >0)
        {
            this.transform.position += new Vector3(0, gameController.vGameSpeed * Time.deltaTime, 0); 
        }
        else
            this.transform.position += new Vector3(0, -gameController.vGameSpeed * Time.deltaTime, 0);
    }
}
