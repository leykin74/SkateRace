using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class g_manager : MonoBehaviour
{
    // Start is called before the first frame update
    NavMeshAgent[] agents;
    GameObject[] numbers;
    public bool gameOver = false;
    bool gameFinished = false;
    public GameObject nextLevelButton;
    private void Awake() {
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        } else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
        agents = transform.GetComponentsInChildren<NavMeshAgent>();
    }

    private void InitCallback ()
    {
        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            var tutParams = new Dictionary<string, object>();
            tutParams[AppEventParameterName.ContentID] = "Level_1_Started";
            tutParams[AppEventParameterName.Description] = "User starter level 1";
            tutParams[AppEventParameterName.Success] = "1";

            FB.LogAppEvent (
                AppEventName.CompletedTutorial,
                parameters: tutParams
            );
        } else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity (bool isGameShown)
    {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        } else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            for (int r=0; r < agents.Length; r++) {
                List<GameObject> numbers = agents[r].GetComponent<getNumbers>().numbers;
                bool _isFinished = false;
                if (agents[r].GetComponent<agentMovingMain>() && agents[r].GetComponent<agentMovingMain>().isFinished) {
                    _isFinished = true;
                }
                if (agents[r].GetComponent<agentMoving>() && agents[r].GetComponent<agentMoving>().isFinished) {
                    _isFinished = true;
                }
                if (agents[r].GetComponent<agentMovingMain>() && !agents[r].enabled && !_isFinished) {
                    foreach (GameObject number in numbers)  {
                        number.SetActive(false);
                    }
                    numbers[agents[r].GetComponent<agentMovingMain>().Place].SetActive(true);
                    numbers[agents[r].GetComponent<agentMovingMain>().Place].transform.parent.LookAt(Camera.main.transform);
                    numbers[agents[r].GetComponent<agentMovingMain>().Place].transform.parent.Rotate(20, 160, 0);
                } else if (!_isFinished) {
                    int num = agents.Length - 1;
                    
                    for (int i=0; i < agents.Length; i++) {
                        if (RemainingDistance(agents[r].path.corners) < RemainingDistance(agents[i].path.corners) && agents[i].GetComponent<Rigidbody>().isKinematic) {
                            num --;
                        }
                    }
                    foreach (GameObject number in numbers)  {
                        number.SetActive(false);
                    }
                    numbers[num].SetActive(true);
                    numbers[num].transform.parent.LookAt(Camera.main.transform);
                    numbers[num].transform.parent.Rotate(20, 160, 0);
                    if (agents[r].GetComponent<agentMovingMain>()) agents[r].GetComponent<agentMovingMain>().Place = num;
                } else if (_isFinished && agents[r].GetComponent<agentMoving>()) agents[r].GetComponent<agentMoving>().enabled = false;
                
            }
        }
    }

    private float RemainingDistance(Vector3[] points)
    {
        if (points.Length < 2) return 0;
        float distance = 0;
        for (int i = 0; i < points.Length - 1; i++)
            distance += Vector3.Distance(points[i], points[i + 1]);
        return distance;
    }

    public void GameOver() {
        nextLevelButton.SetActive(true);
        // gameFinished = true;
        // for (int r=0; r < agents.Length; r++) {
        //     agents[r].GetComponent<NavMeshAgent>().enabled = false;
        // }
        for (int r=0; r < agents.Length; r++) {
            List<GameObject> numbers = agents[r].GetComponent<getNumbers>().numbers;
            if (!agents[r].GetComponent<agentMovingMain>()) {
                foreach (GameObject number in numbers)  {
                    number.SetActive(false);
                }
            }else{
                numbers[agents[r].GetComponent<agentMovingMain>().Place].transform.parent.LookAt(Camera.main.transform);
                numbers[agents[r].GetComponent<agentMovingMain>().Place].transform.parent.Rotate(20, 160, 0);
            }
        }
        var tutParams = new Dictionary<string, object>();
        bool win = false;
        for (int r=0; r < agents.Length; r++) {
            if (agents[r].GetComponent<agentMovingMain>() && agents[r].GetComponent<agentMovingMain>().Place == 0) {
                win = true;
            }
        }
        tutParams[AppEventParameterName.ContentID] = win ? "Win" : "GameOver";
        tutParams[AppEventParameterName.Description] = "User clicked next level button";
        tutParams[AppEventParameterName.Success] = "1";

        FB.LogAppEvent (
            AppEventName.CompletedTutorial,
            parameters: tutParams
        );
        gameOver = true;
    }
    public void restartLevel () {
        var tutParams = new Dictionary<string, object>();
        tutParams[AppEventParameterName.ContentID] = "Restart_button_clicked";
        tutParams[AppEventParameterName.Description] = "User clicked restart button";
        tutParams[AppEventParameterName.Success] = "1";

        FB.LogAppEvent (
            AppEventName.CompletedTutorial,
            parameters: tutParams
        );
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void goNextLevel () {
        Debug.Log("next level");
        var tutParams = new Dictionary<string, object>();
        tutParams[AppEventParameterName.ContentID] = "Next_level_button_clicked";
        tutParams[AppEventParameterName.Description] = "User clicked next level button";
        tutParams[AppEventParameterName.Success] = "1";

        FB.LogAppEvent (
            AppEventName.CompletedTutorial,
            parameters: tutParams
        );
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    private void Start() {
        // var tutParams = new Dictionary<string, object>();
        // tutParams[AppEventParameterName.ContentID] = "Level_1_Started";
        // tutParams[AppEventParameterName.Description] = "User starter level 1";
        // tutParams[AppEventParameterName.Success] = "1";

        // FB.LogAppEvent (
        //     AppEventName.CompletedTutorial,
        //     parameters: tutParams
        // );
    }
}
