using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SwitchTurn : MonoBehaviour
{
    public GameObject handShrek;
    public GameObject handBad;
    public GameObject leaderShrek;
    public GameObject leaderBad;
    public GameObject pointsShrek;
    public GameObject pointsBad;
    public GameObject button;


    public TextMeshProUGUI toRoundImage;

    public Vector2 playPositionHand;
    Vector2 outSidePositionHand;

    Vector2 playPositionLeader;
    Vector2 outSidePositionLeader;

    public int round = 1;


    int livesShrek = 2;
    int livesBad = 2;

    private AudioManager audioM;

    private void Start()
    {
        float xx = handShrek.transform.position.x;
        float yy1 = handShrek.transform.position.y;
        float yy2 = handBad.transform.position.y;

        float xxx1 = leaderShrek.transform.position.x;
        float xxx2 = leaderBad.transform.position.x;
        float yyy = leaderShrek.transform.position.y;

        playPositionHand = new Vector2(xx, yy1);
        outSidePositionHand = new Vector2 (xx, yy2);
        playPositionLeader = new Vector2 (xxx1, yyy);
        outSidePositionLeader = new Vector2 (xxx2, yyy);

        audioM = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void SwitchTurnPlayer (GameObject playerHand)
    {
        audioM.PlaySound(audioM.turnAudio);

        if (playerHand == handShrek)
        {
            handShrek.transform.position = outSidePositionHand;
            leaderShrek.transform.position = outSidePositionLeader;

            handBad.transform.position = playPositionHand;
            leaderBad.transform.position = playPositionLeader;

            if (handBad.GetComponent<Hand>().isFirstRound)
            {
                button = GameObject.Find("StartGameButton");
                button.transform.position = button.GetComponent<StartButton>().boardPosition;
            }
        }
        else
        {
            handBad.transform.position = outSidePositionHand;
            leaderBad.transform.position = outSidePositionLeader;

            handShrek.transform.position = playPositionHand;
            leaderShrek.transform.position = playPositionLeader;
        }
    }


    public void OnClickPass ()
    {
        Hand inHandShrek = handShrek.GetComponent<Hand>();
        Hand inHandBad = handBad.GetComponent<Hand>();
        Vector3 aux = new Vector3(playPositionHand.x, playPositionHand.y);
        GameObject hand = (handShrek.transform.position == aux) ? handShrek : handBad;
        Hand inHand = hand.GetComponent<Hand>();        
        inHand.isPass = true;

        if (inHandShrek.isPass && inHandBad.isPass)
        {
            audioM.PlaySound(audioM.roundAudio);

            GameObject looser = handShrek;
            inHandShrek.isPass = false;
            inHandBad.isPass = false;

            GetPoints inPointsShrek = pointsShrek.GetComponent<GetPoints>();
            GetPoints inPointsBad = pointsBad.GetComponent<GetPoints>();

            GameObject livesToDestroy = GameObject.Find("LiveBorderShrek");

            if (inPointsShrek.points >= inPointsBad.points)
            {
                looser = handBad;
                livesToDestroy = GameObject.Find("LiveBorderBad");
                livesBad--;
            }
            else
            {
                livesShrek--;
            }
            DestroyLives(livesToDestroy);

            GameObject gameOver = GameObject.Find("GameOver");
            GameOverButton inGameOver = gameOver.GetComponent<GameOverButton>();

            if (livesShrek == 0)
            {
                inGameOver.ChangePosition("Lord Farquaad WINS");
            }
            else if (livesBad == 0)
            {
                inGameOver.ChangePosition("Shrek WINS");
            }
            else
            {
                round++;
                toRoundImage.text = round.ToString();
                if (looser.transform.position == aux)
                {
                    SwitchTurnPlayer(looser);
                }
                inHandShrek.OnClickTakeFromDeck(2);
                inHandBad.OnClickTakeFromDeck(2);
            }
        }
        else
        {
            if (hand == handShrek)
            {
                inHandBad.isPass = true;
            }
            else
            {
                inHandShrek.isPass = true;
            }

            SwitchTurnPlayer(hand);
        }
    }


    public void DestroyLives(GameObject liveBorder)
    {
        GameObject.Find("Clear").GetComponent<ClearAllField>().Clear();

        foreach (Transform child in liveBorder.transform)
        {
            Destroy(child.gameObject);
            break;
        }
    }

}
