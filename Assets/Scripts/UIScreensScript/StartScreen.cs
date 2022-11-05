using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour, IUIScreen
{
    public IGameController gameController { get; set; }

    public void OnStartGameClick()
    {
        if (gameController is IGameController)
            gameController.StartGame();
        else
            throw new System.Exception("gameController not initialized");
        Destroy(this.gameObject);
    }
}
