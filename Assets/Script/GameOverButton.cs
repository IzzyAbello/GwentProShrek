using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverButton : MonoBehaviour
{
    public TextMeshProUGUI winner;
    public Vector2 ogPosition;
    Vector2 aux = new Vector2(-1000, -1000);

    private void Start()
    {
        ogPosition = transform.position;
        transform.position = aux;
    }

    public void ChangePosition(string win)
    {
        transform.position = ogPosition;
        winner.text = win;
    }

    public void OnClickRestartGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
