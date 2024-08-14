using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using TMPro;

public class EditorButton : MonoBehaviour
{
    public TMP_InputField inputField;

    public Vector2 ogPosition;
    Vector2 aux = new Vector2(-3000, -3000);
    public GameObject editor;

    private void Start()
    {
        ogPosition = editor.transform.position;
        editor.transform.position = aux;
    }

    public void OnClickReadText()
    {
        TextAsset tt = (TextAsset)Resources.Load("TEST_CASE");

        string s = tt.text;

        Debug.Log("CODE:\n" + s);

        Lexer lexer = new Lexer(s);
        Parser parser = new Parser(lexer);
        Interpreter interpreter = new Interpreter(parser);
    }

    public void OnClickGoToEditor()
    {
        editor.transform.position = ogPosition;
    }

    public void OnClickExitEditor()
    {
        editor.transform.position = aux;
    }

    public void OnClickQuitEditor()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
