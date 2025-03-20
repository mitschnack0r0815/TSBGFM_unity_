
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private StyleSheet _styleSheet;

    public static event Action OnTestBtnClicked;

    private void Start()
    {
        StartCoroutine(Generate());
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        /* This will allow us to see the changes in the editor */
        // StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        var root = _uiDocument.rootVisualElement;
        root.styleSheets.Add(_styleSheet);

   
        var container = CreateElement<VisualElement>("container");

        // var viewBox = CreateElement<VisualElement>("view-box");
        // container.Add(viewBox);

        var controlbox = CreateElement<VisualElement>("control-box","bordered-box");
        container.Add(controlbox);

        var titleLable = CreateElement<Label>("main-lable");
        titleLable.text = "Main Menu";
        controlbox.Add(titleLable);

        var testBtn_0 = CreateElement<Button>("main-btn");
        testBtn_0.clicked += () => Debug.Log("testBtn_0 clicked");
        testBtn_0.clicked += OnTestBtnClicked;
        testBtn_0.text = "testBtn_0";
        controlbox.Add(testBtn_0);

        var testBtn_1 = CreateElement<Button>("main-btn");
 
        testBtn_1.text = "testBtn_1";
        controlbox.Add(testBtn_1);

        root.Add(container);

        yield break;
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
