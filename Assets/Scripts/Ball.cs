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
        {   // ��� ������� ����������� ����������� ������� 
            gameController = value;
            gameController.OnStartGame += StartGame;
            gameController.OnStartMove += StartMove;
            gameController.OnStopMove += StopMove;
        } 
    }
    float verticalVelosity;
    bool MoveAvailable = false;

    void StartGame()
    {
        verticalVelosity = gameController.GameSpeed;
        MainSprite.SetActive(false);
    }
    void StartMove()
    {
        MainSprite.SetActive(true);
        MoveAvailable = true;
    }
    void StopMove() => MoveAvailable = false;

    IEnumerator BurstBall()
    {   // ������ ����� - �������� �� �������� 
        MoveAvailable = false;
        for (int i = 0; i < BurstTextures.Length; i++)
        {
            MainSprite.GetComponent<SpriteRenderer>().sprite = BurstTextures[i];
            yield return new WaitForSeconds(0.01f);
        }
        // �������� ����������� ��� ������
        // ������������ �� ������� 
        // ������� ������
        gameController.BallBursted();
        gameController.OnStartGame -= StartGame;
        gameController.OnStartMove -= StartMove;
        gameController.OnStopMove -= StopMove;
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!MoveAvailable) return; // ����� ������� �� ���������� ���� ���� �������� ������� ����
        StartCoroutine(BurstBall());
    }


    void Update()
    {
        // �� �������� MVP ����� ����� ��� ���� ���-������ � ���������� - �� ��� ���� ������� ����� ���� ������������ ���
        if (!MoveAvailable) return;
        if (Input.touchCount >0)
        {
            this.transform.position += new Vector3(0, verticalVelosity * Time.deltaTime, 0); 
        }
        else
            this.transform.position += new Vector3(0, -verticalVelosity * Time.deltaTime, 0);
    }
}
