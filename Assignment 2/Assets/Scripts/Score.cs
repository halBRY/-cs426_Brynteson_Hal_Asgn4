using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Score : NetworkBehaviour
{
    public TMP_Text scoreText1;
    public TMP_Text scoreText2;
    public GameObject endgame;
    public int maxScore = 5;

    private NetworkVariable<int> score1 = new NetworkVariable<int>();
    private NetworkVariable<int> score2 = new NetworkVariable<int>();

    private void Start()
    {
        scoreText1.text = "Cat: 0";
        scoreText2.text = "Mice: 0";

        score1.OnValueChanged += OnScoreChanged;
        score2.OnValueChanged += OnScoreChanged;
    }

    private void OnScoreChanged(int oldValue, int newValue)
    {
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (IsClient)
        {
            scoreText1.text = "Cat: " + score1.Value;
            scoreText2.text = "Mice: " + score2.Value;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPointServerRpc(string team)
    {
        if (team == "Cat")
        {
            score1.Value++;
        }
        if (team == "Mouse")
        {
            score2.Value++;
        }

        CheckForEndGame();
    }

    [ServerRpc(RequireOwnership = false)]
    public void LosePointServerRpc(string team)
    {
        if (team == "Cat")
        {
            score1.Value--;
        }

        if (team == "Mouse")
        {
            score2.Value--;
        }
    }
    
    private void CheckForEndGame()
    {
        if (score1.Value >= maxScore || score2.Value >= maxScore)
        {
            TriggerEndGame();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TriggerEndGameServerRpc()
    {
        if (IsServer) 
        {
        TriggerEndGame();
        }
    }

    private void TriggerEndGame()
    {
        EndGameServerRpc();
    }


    [ServerRpc]
    // method name must end with ServerRPC
    private void EndGameServerRpc()
    {
        EndGameClientRpc();
    }

    [ClientRpc]
    private void EndGameClientRpc()
    {
        GameObject endgameUI = Instantiate(endgame);
    }
}
