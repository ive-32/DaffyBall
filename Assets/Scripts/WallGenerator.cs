using UnityEngine;

public class WallGenerator : MonoBehaviour
{

    public GameObject WallPrefab;
    public IGameController gamecontroller;
    float TravelledDistance;
    int LastBarrierDistance;
    bool generatorEnabled;

    Vector2Int fieldSize;
    private void Awake()
    {
        // ��������� �������� ������� ������ � �������� Unity, �������� �� 2 ����� �� �����������, ����� ���������� �� ��������� ������
        // �� ��������� ������� �� ����� �����, �� ������� � ������ ��������� � ����������� ���� ������
        Camera _camera = Camera.main;
        Vector3 afieldSize = _camera.ViewportToWorldPoint(Vector2.one);
        fieldSize = Vector2Int.RoundToInt(afieldSize) + new Vector2Int(2, -1);
        // ������� ������� �����������
        this.TryGetComponent<IGameController>(out gamecontroller);
        if (!(gamecontroller is IGameController))
            throw new System.Exception("This Unity Object not contain class IGameController");
        generatorEnabled = false;
        gamecontroller.OnStartGame += StartGame;
        gamecontroller.OnStartMove += StartGenerator;
        gamecontroller.OnStopMove += StopGenerator;
    }

    void StartGame()
    {
        TravelledDistance = 0;
        LastBarrierDistance = -1;
        for (int i = -fieldSize.x; i <= fieldSize.x; i++)
        {
            Instantiate(WallPrefab, new Vector3(i, fieldSize.y, 0), Quaternion.identity, this.transform);
            Instantiate(WallPrefab, new Vector3(i, -fieldSize.y, 0), Quaternion.identity, this.transform);
        }
        generatorEnabled = false;
    }

    void StartGenerator() => generatorEnabled = true;
    void StopGenerator() => generatorEnabled = false;

    void SetNewBarrier()
    {
        LastBarrierDistance = Mathf.FloorToInt(TravelledDistance);
        int height = Mathf.RoundToInt(Random.Range(fieldSize.y * 0.2f, fieldSize.y * 1.3f));
        int IsTopOrBottom = Random.Range(0,100) <50 ? 1: -1;
        for (int i = 1; i <= height; i++ )
        {
            Vector3 BarrierTilePos = new Vector3(fieldSize.x, (-fieldSize.y + i) * IsTopOrBottom, 0);
            Instantiate(WallPrefab, BarrierTilePos, Quaternion.identity, this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!generatorEnabled) return;
        float gameSpeed = 1f;
        int BarrierDistance = 4; // fieldSize.x + 2;
        TravelledDistance += gameSpeed *Time.deltaTime;
        foreach (Transform wall in this.transform)
        {
            // ��������
            wall.position -= new Vector3(gameSpeed * Time.deltaTime, 0, 0);
            // �������� ������ ������������, ������ ������� ������� ��������
            if ( wall.position.x < -fieldSize.x)
            {
                if (wall.position.y == fieldSize.y || wall.position.y == -fieldSize.y)
                {
                    wall.position = new Vector3(fieldSize.x, wall.position.y, 0);
                    
                }
                else
                    Destroy(wall.gameObject);
            }
        }
        // �����������
        if (Mathf.FloorToInt(TravelledDistance) % BarrierDistance == 0 && LastBarrierDistance != Mathf.FloorToInt(TravelledDistance))
            SetNewBarrier();
    }
}
