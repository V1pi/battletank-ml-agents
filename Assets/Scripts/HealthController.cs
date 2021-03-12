using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour {
    [Header("Health Settings")]
    [SerializeField] private int maxLife = 2000;
    [SerializeField] private float currentLife;
    [Tooltip("Quantidade de dano infligida ao oponente")]
    [SerializeField] private float damageInflicted;
    [Tooltip("Quantidade de dano recebida")]
    [SerializeField] private float damageReceived;
    private AgentController agentController;
    [SerializeField] private GameController gameController;

    private void Start() {
        agentController = GetComponent<AgentController>();
        gameController = this.transform.parent.GetComponentInChildren<GameController>();
    }

    public void Reset() {
        damageInflicted = 0;
        damageReceived = 0;
        currentLife = maxLife;
    }

    public void TakeDamage(float damage, AgentController enemy) {
        if(currentLife > 0) {
            this.currentLife -= damage;
            this.damageReceived += damage;
            if (currentLife <= 0) {
                gameController.CountDeadPlayers(this.agentController, enemy);
            }
        }
    }

    public void InflictDamage(float damage) {
        this.damageInflicted += damage;
    }
}
