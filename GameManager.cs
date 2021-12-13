using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
//Megan McNamee

public class GameManager : MonoBehaviour
{
    //access game manager from any script
    public static GameManager instance;

    public int maxPlayers;
    //list of players we can update as more enter game
    public List<PlayerController> activePlayers = new List<PlayerController>();

    public GameObject playerSpawnEffect;

    public bool canFight;

    public string[] allLevels;

    //randomly fill level order, removing one by one then load in sequence 
    private List<string> levelOrder = new List<string>();

    [HideInInspector]
    public int lastPlayerNumber;

    public int pointsToWin;
    private List<int> roundWins = new List<int>();

    private bool gameWon;

    public string winLevel;

    //initialize objects and set them up to be used by other scripts
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            GoToNextArena();
        }
    }

    //check if the player is brought into the world
    public void AddPlayer(PlayerController newPlayer)
    {
        //checks if there's enough players to add to list
        if (activePlayers.Count < maxPlayers)
        {
            activePlayers.Add(newPlayer);
            Instantiate(playerSpawnEffect, newPlayer.transform.position, newPlayer.transform.rotation);
        } else
        {
            Destroy(newPlayer.gameObject);
        }
    }

    public void ActivatePlayers()
    {
        foreach(PlayerController player in activePlayers)
        {
            player.gameObject.SetActive(true);
            player.GetComponent<PlayerHealthController>().FillHealth();
        }
    }

    public int CheckActivePlayers()
    {
        int playerAliveCount = 0;

        //allows to actively check how many players are in scene 
        for (int i = 0; i < activePlayers.Count; i++)
        {
            if(activePlayers[i].gameObject.activeInHierarchy)
            {
                playerAliveCount++;
                lastPlayerNumber = i;
            }
        }

        return playerAliveCount;
    }

    public void GoToNextArena()
    
    {
        //array of names of levels to load into
        if (!gameWon)
        {
            if (levelOrder.Count == 0)
            {
                List<string> allLevelList = new List<string>();
                allLevelList.AddRange(allLevels);

                for (int i = 0; i < allLevels.Length; i++)
                {
                    int selected = Random.Range(0, allLevelList.Count);

                    levelOrder.Add(allLevelList[selected]);
                    allLevelList.RemoveAt(selected);
                }
            }

            string levelToLoad = levelOrder[0];
            levelOrder.RemoveAt(0);

            foreach (PlayerController player in activePlayers)
            {
                player.gameObject.SetActive(true);
                player.GetComponent<PlayerHealthController>().FillHealth();
            }

            SceneManager.LoadScene(levelToLoad);

        } else
        {
            foreach (PlayerController player in activePlayers)
            {
                player.gameObject.SetActive(false);
                player.GetComponent<PlayerHealthController>().FillHealth();
            }

            SceneManager.LoadScene(winLevel);
        }
    }

    public void StartFirstRound()
    {
        roundWins.Clear();
        foreach(PlayerController player in activePlayers)
        {
            roundWins.Add(0);
        }

        gameWon = false;

        GoToNextArena();
    }

    public void AddRoundWin()
    {
        if(CheckActivePlayers() == 1)
        {
            roundWins[lastPlayerNumber]++;

            if(roundWins[lastPlayerNumber] >= pointsToWin)
            {
                gameWon = true;
            }
        }
    }
}
