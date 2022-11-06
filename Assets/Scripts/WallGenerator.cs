using UnityEngine;

public class WallGenerator : MonoBehaviour
{

    public GameObject WallPrefab;
    public IGameController gamecontroller;
    public GameObject WallsLayer;
    int BarrierDistance; // ������� ����������� 
    float TravelledDistance;
    int LastBarrierDistance;
    int LastBarrierEdgeYPos; // ���������� ����� ��� ��������� ������ ����� �� ���� ������������ ��������
    bool generatorEnabled;

    Vector2Int fieldSize;
    private void Awake()
    {
        // ������� ������� �����������
        this.TryGetComponent<IGameController>(out gamecontroller);
        if (!(gamecontroller is IGameController))
            throw new System.Exception("This Unity Object not contain class IGameController");
        generatorEnabled = false;
        gamecontroller.OnStartGame += StartGame;
        gamecontroller.OnStartMove += StartGenerator;
        gamecontroller.OnStopMove += StopGenerator;
        gamecontroller.OnEndGame += EndGame;
    }

    void StartGame()
    {
        fieldSize = gamecontroller.FieldSize;
        BarrierDistance = fieldSize.x / 2; // ������� ����������� - �������� ������ ����
        TravelledDistance = 0;
        LastBarrierDistance = -1;
        for (int i = -fieldSize.x; i <= fieldSize.x; i++)
        {
            Instantiate(WallPrefab, new Vector3(i, fieldSize.y, 0), Quaternion.identity, WallsLayer.transform);
            Instantiate(WallPrefab, new Vector3(i, -fieldSize.y, 0), Quaternion.identity, WallsLayer.transform);
        }
        generatorEnabled = false;
    }
    void EndGame()
    {
        generatorEnabled = false;
        foreach(Transform child in WallsLayer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void StartGenerator() => generatorEnabled = true;
    void StopGenerator() => generatorEnabled = false;

    void SetNewBarrier()
    {   // ������ ����� ������ 
        // ���� ����� ������ ���� - �� ���� ������ ������
        float lastTilePos = -fieldSize.x;
        foreach (Transform wall in WallsLayer.transform)
        {
            if (wall.position.x > lastTilePos)
                lastTilePos = wall.position.x;
        }
        // ������ 20% - 65% �� ������ ���� 
        // ��������� - ����������� ���-�� ���� ���� ������� -����� ����� ������� ���� �������� ����� �������� �����������
        // ���� ������ ����� ������������ ������� �������� ������ ������� ������
        LastBarrierDistance = Mathf.FloorToInt(TravelledDistance);
        int height;
        int numiteration = 0;
        Vector2Int thisBarrierEdge;
        Vector2Int lastBarrierEdge = new Vector2Int(0, LastBarrierEdgeYPos);
        do
        {
            numiteration++; // ����� �� ����������
            height = Mathf.RoundToInt(Random.Range(fieldSize.y * 0.4f, fieldSize.y * 1.3f));
            thisBarrierEdge = new Vector2Int(BarrierDistance, LastBarrierEdgeYPos);
        }
        while (Vector2Int.Distance(thisBarrierEdge, lastBarrierEdge) > new Vector2(gamecontroller.GameSpeed, gamecontroller.vGameSpeed).magnitude * 1.3f &&
            numiteration < 100);
        int IsTopOrBottom = Random.Range(0,100) <50 ? 1: -1;
        for (int i = 1; i <= height; i++ )
        {
            Vector3 BarrierTilePos = new Vector3(lastTilePos, (-fieldSize.y + i) * IsTopOrBottom, 0);
            Instantiate(WallPrefab, BarrierTilePos, Quaternion.identity, WallsLayer.transform);
        }
        LastBarrierEdgeYPos = (-fieldSize.y + height + 1) * IsTopOrBottom;
    }

    void Update()
    {
        if (!generatorEnabled) return;
        float gameSpeed = gamecontroller.GameSpeed;
         
        TravelledDistance += gameSpeed *Time.deltaTime;
        

        foreach (Transform wall in WallsLayer.transform)
        {
            // ��������
            wall.position -= new Vector3(gameSpeed * Time.deltaTime, 0, 0);
            // �������� ������ ������������
            if ( wall.position.x < -fieldSize.x)
            {
                if (wall.position.y == fieldSize.y || wall.position.y == -fieldSize.y)
                    //wall.position = new Vector3(lastTilePos + 1, wall.position.y, 0);
                    wall.position += new Vector3(fieldSize.x * 2, 0, 0);
                else
                    Destroy(wall.gameObject);
            }
        }
        // �����������
        if (Mathf.FloorToInt(TravelledDistance) % BarrierDistance == 0 && LastBarrierDistance != Mathf.FloorToInt(TravelledDistance))
            SetNewBarrier();
    }
}
