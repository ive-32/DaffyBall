using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour, IUIScreen
{
    public virtual IGameController GameController { get; set; }
    public Slider GameSpeedSlider;
    public virtual void Start()
    {
        GameSpeedSlider.value = GameController.GameSpeed / 2;    
    }

    public virtual void OnStartGameClick()
    {
        if (GameController is IGameController)
        {
            GameController.GameSpeed = GameSpeedSlider.value * 2;
            GameController.StartGame();
        }
        else
            throw new System.Exception("gameController not initialized");
        Destroy(this.gameObject);
    }

    public virtual void SetText(string atext)
    {
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu))
            Application.Quit();
    }
}
