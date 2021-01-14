using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject LeftWall;
    public GameObject RightWall;
    public GameObject Floor;
    public GameObject Ceiling;

    private bool gameOver = false;
    private int score = 0;

    private float mapX;
    private float mapY;
    private float mapW;
    private float mapH;

    private List<float> GameArea;
    // Start is called before the first frame update
    void Awake()
    {
        GameArea = new List<float>();
        CalculateGameArea();
        Cursor.visible = false;
    }

    void CalculateGameArea()
    {
        float mapX = LeftWall.transform.position.x + LeftWall.GetComponent<Collider>().bounds.size.x/2;
        float mapY = Ceiling.transform.position.y + Ceiling.GetComponent<Collider>().bounds.size.y/2;
        float mapW = RightWall.transform.position.x - RightWall.GetComponent<Collider>().bounds.size.x/2;
        float mapH = Floor.transform.position.y - Floor.GetComponent<Collider>().bounds.size.y/2;
        GameArea.Add(mapX);
        GameArea.Add(mapY);
        GameArea.Add(mapW);
        GameArea.Add(mapH);
    }

    public List<float> GetGameArea()
    {
        return GameArea;
    }

    public int GetScore()
    {
        return score;
    }

    public void AddScore(int addedScore)
    {
        score += addedScore;
    }

    public void EndGame()
    {
        gameOver = true;
        StartCoroutine(EndNow());
        IEnumerator EndNow()
        {
            yield return new WaitForSeconds(3);
            Cursor.visible = true;
            SceneManager.LoadScene(0);
        }
    }

    public bool CheckGameOver()
    {
        return gameOver;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
