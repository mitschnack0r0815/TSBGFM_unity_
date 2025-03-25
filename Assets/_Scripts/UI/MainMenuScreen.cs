
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuScreen : MonoBehaviour
{
    public static MainMenuScreen Instance;
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private StyleSheet _styleSheet;

    public Label CurrPlayerLable { get; set; }

    public DropdownField PlayerDropdown { get; set; }
    public static event Action OnPlayerDropdownChoose;

    public static event Action OnTestBtnClicked;

    public static event Action OnEndTurnBtnClicked;



    private void Start()
    {
        // StartCoroutine(Generate());
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        /* This will allow us to see the changes in the editor */
        // StartCoroutine(Generate());
    }

    void Awake() {
        Instance = this;
    }

    public void GenerateUI()
    {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        var root = _uiDocument.rootVisualElement;
        root.styleSheets.Add(_styleSheet);

   
        var container = CreateElement<VisualElement>("container");

        CreateTestMenu(container);

        CreateMenu(container);

        root.Add(container);

        yield break;
    }

    private void CreateMenu(VisualElement ele) {
        var container = ele;

        var controlbox = CreateElement<VisualElement>("control-box","bordered-box");
        container.Add(controlbox);

        var titleLable = CreateElement<Label>("main-lable");
        titleLable.text = "Game Menu";
        controlbox.Add(titleLable);

        var endTurnBtn = CreateElement<Button>("main-btn");
        endTurnBtn.clicked += () => Debug.Log("endTurnBtn clicked");
        endTurnBtn.clicked += OnEndTurnBtnClicked;
        endTurnBtn.text = "End Turn";
        controlbox.Add(endTurnBtn);
    }

    private void CreateTestMenu(VisualElement ele) {
        var container = ele;
        // var viewBox = CreateElement<VisualElement>("view-box");
        // container.Add(viewBox);

        var controlbox = CreateElement<VisualElement>("control-box","bordered-box");
        container.Add(controlbox);

        var titleLable = CreateElement<Label>("main-lable");
        titleLable.text = "Test Menu";
        controlbox.Add(titleLable);

        CurrPlayerLable = CreateElement<Label>("player-info");
        controlbox.Add(CurrPlayerLable);

        var testBtn_0 = CreateElement<Button>("main-btn");
        testBtn_0.clicked += () => Debug.Log("testBtn_0 clicked");
        testBtn_0.clicked += OnTestBtnClicked;
        testBtn_0.text = "Test Button";
        controlbox.Add(testBtn_0);

        var reloadBtn = CreateElement<Button>("main-btn");
        reloadBtn.clicked += () => {
            Debug.Log("Reload GameState");
            ExampleGameManager.Instance.ChangeState(GameState.Starting);
        };
        reloadBtn.text = "Reload";
        controlbox.Add(reloadBtn);

        // var player_1 = CreateElement<Button>("main-btn");
        // player_1.text = "Player 1";
        // player_1.clicked += () => {
        //     Debug.Log("Player 1 clicked");
        // };
        // controlbox.Add(player_1);

        // var player_2 = CreateElement<Button>("main-btn");
        // player_2.text = "Player 2";
        // player_2.clicked += () => {
        //     Debug.Log("Player 2 clicked");
        // };
        // controlbox.Add(player_2);

        PlayerDropdown = CreateElement<DropdownField>("player-dropdown");
        PlayerDropdown.RegisterValueChangedCallback(evt => OnPlayerDropdownChoose?.Invoke());
        controlbox.Add(PlayerDropdown);
    }

    T CreateElement<T>(params string[] classNames) where T : VisualElement, new()
    {
        var element = new T();
        foreach (var className in classNames)
        {
            element.AddToClassList(className);
        }
        return element;
    }
}
