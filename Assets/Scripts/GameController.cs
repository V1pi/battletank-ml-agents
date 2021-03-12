using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum GamesState {
    RUNNING,
    RESETTING
}
public class GameController : MonoBehaviour {
    AgentController[] agents;
    [SerializeField] GameObject[] prefabs;
    int countDeadPlayers = 0;
    float timer = 0;
    [SerializeField] private float matchTime = 10f;
    private Transform[] spawns;
    [Range(1, 4)]
    [SerializeField] private int nPlayersInGame = 2;
    GamesState currentGameState = GamesState.RESETTING;
    private Dictionary<AgentType, int> nKillsPerAgent = new Dictionary<AgentType, int>();
    private List<int> positionsSelected = new List<int>();

    private void Start() {
        agents = new AgentController[nPlayersInGame];
        spawns = new Transform[5];
        spawns[0] = this.transform.parent.Find("SpawnA");
        spawns[1] = this.transform.parent.Find("SpawnB");
        spawns[2] = this.transform.parent.Find("SpawnC");
        spawns[3] = this.transform.parent.Find("SpawnD");
        spawns[4] = this.transform.parent.Find("SpawnE");
        DoReset();
    }

    private void Update() {
        timer += Time.deltaTime;
        if(timer >= matchTime && currentGameState == GamesState.RUNNING) {
            foreach (var agent in agents) {
                if(agent.gameObject.activeSelf) {
                    agent.gameObject.SetActive(false);
                }
            }
            ResetEnviroment();
        }

        if(currentGameState == GamesState.RUNNING && countDeadPlayers == nPlayersInGame - 1) {
            foreach (var agent in agents) {
                if (agent.gameObject.activeSelf) {
                    if(nKillsPerAgent[agent.agentType] != 0) {
                        agent.AddReward(1f);
                    }
                    agent.gameObject.SetActive(false);
                }
            }
            ResetEnviroment();
        }

        if(currentGameState == GamesState.RESETTING && timer >= 0.2f) {
            DoReset();
        }
        
    }

    private void ResetEnviroment() {
        currentGameState = GamesState.RESETTING;
        timer = 0;

    }

    private void DoReset() {
        positionsSelected = new List<int>();
        timer = 0;
        countDeadPlayers = 0;
        currentGameState = GamesState.RUNNING;
        InitializeAgents();
    }

    private void InitializeAgents() {
        for (int i = 0; i < nPlayersInGame; i++) {
            Vector3 worldPos = GetRandomSpawnPosition();
            GameObject agent = null;

            if (agents[i] != null) {
                agent = agents[i].gameObject;
                agent.transform.position = worldPos;
                agent.SetActive(true);
            } else {
                agent = Instantiate(prefabs[i], worldPos, Quaternion.identity, this.transform.parent);
            }
            nKillsPerAgent[agent.GetComponent<AgentController>().agentType] = 0;
            agents[i] = agent.GetComponent<AgentController>();
        }
    }

    public void CountDeadPlayers(AgentController deadAgent, AgentController enemy) {
        countDeadPlayers++;
        foreach (var agent in agents) {
            if(agent.agentType == deadAgent.agentType) {
                if (enemy.agentType != deadAgent.agentType) {
                    nKillsPerAgent[enemy.agentType]++;
                }
                agent.gameObject.SetActive(false);
            }
        }
        if (countDeadPlayers >= nPlayersInGame) {
            ResetEnviroment();
        }
    }

    public Vector3 GetRandomSpawnPosition() {
        float x = Random.Range(-3, 3);
        float z = Random.Range(-3, 3);
        int pos = Random.Range(0, spawns.Length);
        while(positionsSelected.Contains(pos)) {
            pos = Random.Range(0, spawns.Length);
        }
        positionsSelected.Add(pos);
        return spawns[pos].position + z * Vector3.forward + x * Vector3.right;
    }
}
