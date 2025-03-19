
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private StyleSheet _styleSheet;

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

        // var titleLable = new Label("Main Menu");
        // root.Add(titleLable);

        // var playButton = new Button(() => { 
        //     Debug.Log("Play Button Clicked"); 
        // });
        // playButton.text = "TestBtn";
        // playButton.AddToClassList("TestBtn");
        // root.Add(playButton);

        var container = CreateElement<VisualElement>("container");

        var viewBox = CreateElement<VisualElement>("view-box");
        container.Add(viewBox);

        var controlbox = CreateElement<VisualElement>("control-box");
        container.Add(controlbox);

        root.Add(container);

        yield break;
    }

    T CreateElement<T>(string className) where T : VisualElement, new()
    {
        var element = new T();
        element.AddToClassList(className);
        return element;
    }
}
