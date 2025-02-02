using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Mono.Cecil.Cil;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using TMPro.EditorUtilities;


public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    [SerializeField]
    GameObject uiPanel;
    public Text ScoreText;
    public GameObject GameOverText;

    public TextMeshProUGUI[] texts;
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;
    public int bestScore;
    public string playerName;

    // Start is called before the first frame update
    void Start()
    {
        LoadColor();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SaveColor();

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            string s = "Name: "+ playerName + " Best Score: " + bestScore.ToString();
            texts[i].text = s;
            
        }
    }

    public void updatename(string s)
    {
        playerName = s;
    }
    public void StartGame()
    {
      
        uiPanel.SetActive(false);
      
    }

    public void ExitGame()
    {
        SaveColor();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
    
#endif

    }
    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }




 
    [System.Serializable]
    class SaveData
    {
        public int bestScore;
        public string playerName;
    }

    public void SaveColor()
    {
        SaveData data = new SaveData();
        bestScore = bestScore> m_Points?bestScore:m_Points;
        data.bestScore = bestScore;
        data.playerName = playerName;
        Debug.Log(data);
        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadColor()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        Debug.Log(path);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bestScore  = data.bestScore;
            playerName  = data.playerName;
        }
    }

}
