
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

    private Label _unitInfo;

    public DropdownField PlayerDropdown { get; set; }
    public static event Action OnPlayerDropdownChoose;

    public static event Action OnTestBtnClicked;

    public static event Action OnExecuteMoveBtnClicked;
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

        CreateUnitInfo(container);

        root.Add(container);

        yield break;
    }

    public void SetUnitInfo(PlayerUnit playerUnit)
    {
        if (playerUnit == null)
        {
            _unitInfo.text = "No unit selected.";
            return;
        }

        // Use StringBuilder for efficient string concatenation
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"<b>Name:</b> {playerUnit.Unit.name}");
        sb.AppendLine($"<b>Armor:</b> {playerUnit.Unit.armor}");
        sb.AppendLine($"<b>Life:</b> {playerUnit.Unit.life}");
        sb.AppendLine($"<b>Attack:</b> {playerUnit.Unit.moveDistance}");
        sb.AppendLine($"<b>Actions:</b> {playerUnit.ActionsLeft}/2");

        // Update the UnitInfo label
        _unitInfo.text = sb.ToString();
    }

    private void CreateUnitInfo(VisualElement ele) {
        var container = ele;

        var controlbox = CreateElement<VisualElement>("control-box","bordered-box");
        container.Add(controlbox);

        var titleLable = CreateElement<Label>("main-lable");
        titleLable.text = "Unit Info";
        controlbox.Add(titleLable);

        _unitInfo = CreateElement<Label>("unit-info");
        controlbox.Add(_unitInfo);  
    }

    private void CreateMenu(VisualElement ele) {
        var container = ele;

        var controlbox = CreateElement<VisualElement>("control-box","bordered-box");
        container.Add(controlbox);

        var titleLable = CreateElement<Label>("main-lable");
        titleLable.text = "Game Menu";
        controlbox.Add(titleLable);

        var btnExecuteMove = CreateElement<Button>("main-btn");
        btnExecuteMove.text = "Execute Move";
        btnExecuteMove.clicked += () => {
            btnExecuteMove.SetEnabled(false);
            ExampleUnitManager.Instance.LogInPlayerUnit.ExecuteMove();
            btnExecuteMove.SetEnabled(true);
        };
        controlbox.Add(btnExecuteMove);

        var btnBoxLabel = CreateElement<Label>("btn-box-label");
        btnBoxLabel.text = "Switch Unit";
        controlbox.Add(btnBoxLabel);

        var btnBox = CreateElement<VisualElement>("btn-box");
        controlbox.Add(btnBox);

        var switchLeftBtn = CreateElement<Button>("btn");
        switchLeftBtn.text = "<";
        switchLeftBtn.clicked += () => {
            Debug.Log("switchLeftBtn clicked");
            if (ExampleUnitManager.Instance.LogInPlayerUnit.actionStartPosition.x != 
                ExampleGameManager.Instance.LoginUnit.position.x ||
                ExampleUnitManager.Instance.LogInPlayerUnit.actionStartPosition.y != 
                ExampleGameManager.Instance.LoginUnit.position.y) 
            {
                Debug.Log("Confirm your move first!");
                return;
            }
            ExampleGameManager.Instance.SwitchLoginPlayerUnit(-1);
        };
        btnBox.Add(switchLeftBtn);

        var switchRightBtn = CreateElement<Button>("btn");
        switchRightBtn.clicked += () => Debug.Log("switchRightBtn clicked");
        switchRightBtn.text = ">";
        switchRightBtn.clicked += () => {
            Debug.Log("switchLeftBtn clicked");
            if (ExampleUnitManager.Instance.LogInPlayerUnit.actionStartPosition.x != 
                ExampleGameManager.Instance.LoginUnit.position.x ||
                ExampleUnitManager.Instance.LogInPlayerUnit.actionStartPosition.y != 
                ExampleGameManager.Instance.LoginUnit.position.y) 
            {
                Debug.Log("Confirm your move first!");
                return;
            }
            ExampleGameManager.Instance.SwitchLoginPlayerUnit(1);
        };
        btnBox.Add(switchRightBtn);

        var endTurnBtn = CreateElement<Button>("main-btn");
        endTurnBtn.clicked += () => {
            Debug.Log("endTurnBtn clicked");
            ExampleGameManager.Instance.EndTurn();
        };
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
