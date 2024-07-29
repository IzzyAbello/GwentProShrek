using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
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
        //string s = "effect{ Name: \"Damage\", Params: {Prepucio: Number}, Action: (targets, context) => { ii = 455; i = 0; while(i == 0 || i < ii + 5 && true != false){i = i + 2 + 3 * 5 * (3 + 2);}; }}";

        TextAsset tt = (TextAsset)Resources.Load("TEST_CASE");

        string s = tt.text;

        Debug.Log("CODE:\n" + s);

        Lexer lexer = new Lexer(s);

        Parser parser = new Parser(lexer);

        /*for (int i = 0; i < s.Length; i++)
        {
            string ss = "NADA";
            if (!MyTools.IsAlnum(s[i]))
            {
                ss = "ALGO Q VER AQUI***********************";
                if (s[i] == '\0') ss = "VACIO";
                if (s[i] == ' ') ss = "ESPACIO";
                if (s[i] == '\t') ss = "TAB";
                if (s[i] == '\n') ss = "CAMBIO DE LINEA";
                if (s[i] == '\r') ss = "FIN DE LINEA";

                Debug.Log(s[i] + "  :   " + ss);
            }
        }*/

        //lexer.ReadAllText();

        parser.Parse();


        /*Scope global = new Scope();
        Scope inner = new Scope(global);

        global.Set("pinga", ASTType.Type.CARD);

        Debug.Log(inner.IsInScope("pinga"));*/
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
