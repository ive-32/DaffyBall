interface IUIScreen
{
    IGameController GameController { get; set; }
    void SetText(string atext);
}
