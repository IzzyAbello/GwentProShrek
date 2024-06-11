using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EditorButton : MonoBehaviour
{
    public Vector2 ogPosition;
    Vector2 aux = new Vector2(-3000, -3000);
    public GameObject editor;

    private void Start()
    {
        ogPosition = editor.transform.position;
        editor.transform.position = aux;
    }

    public void OnClickGoToEditor()
    {
        editor.transform.position = ogPosition;
    }

    public void OnClickQuitEditor()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
