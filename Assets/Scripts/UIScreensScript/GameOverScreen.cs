using UnityEngine;
using TMPro;

public class GameOverScreen : StartScreen
{
    public GameObject SelectSpeedCanvas;
    [SerializeField] private GameObject SpeedSelectButon;
    [SerializeField] private TextMeshProUGUI GameInfoText;

    public void OnSpeedSelectClick()
    {
        Destroy(SpeedSelectButon);
        SelectSpeedCanvas.SetActive(true);
    }


    public override void SetText(string atext)
    {
        base.SetText(atext);
        if (GameInfoText == null)
            throw new System.Exception("This Unity Object not contain text field for game statistics");
        GameInfoText.text = atext;
    }
}
