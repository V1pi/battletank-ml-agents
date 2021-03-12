using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using System.Linq;
public enum RotationState {
    NONE,
    RIGTH,
    LEFT
}

public enum MovementState {
    NONE,
    FORWARD,
    BACKWARD
}

public enum AttackState {
    NONE,
    ATTACK
}
public enum AgentType {
    AGENT_1,
    AGENT_2,
    AGENT_3,
    AGENT_4
}
[RequireComponent(typeof(HealthController))]
public class AgentController : Agent {
    private MovementState currentMovementState = MovementState.NONE;
    private RotationState currentRotationState = RotationState.NONE;
    private AttackState currentAttackState = AttackState.NONE;
    private BehaviorParameters behaviorParameters;
    [Header("Movement Settings")]
    public float velocityRotateY = 3.0f;
    public float velocityMovement = 6.0f;
    [Header("Attack Settings")]
    [SerializeField] private float rateFire = 5.0f;
    [SerializeField] private Transform spawnBullet;
    [SerializeField] private GameObject shellPrefab;
    public AgentType agentType;
    private List<GameObject> shells = new List<GameObject>();
    private HealthController healthController;
    private float timeLastBullet = 0;
    private GameController gameController;
    private Transform environment;


    private Rigidbody rb;
    private void Awake() {
        healthController = GetComponent<HealthController>();
        rb = GetComponent<Rigidbody>();
        environment = this.transform.parent;
        gameController = environment.GetComponentInChildren<GameController>();
        behaviorParameters = GetComponent<BehaviorParameters>();


        for (int i = 0; i < rateFire; i++) {
            GameObject newShell = Instantiate<GameObject>(shellPrefab, this.transform.parent);
            newShell.SetActive(false);
            shells.Add(newShell);
        }
    }

    public override void OnEpisodeBegin() {
        this.gameObject.SetActive(true);
        currentRotationState = RotationState.NONE;
        currentMovementState = MovementState.NONE;
        currentAttackState = AttackState.NONE;
        timeLastBullet = 0;
        healthController.Reset();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers) {
        ActionSegment<int> actions = actionBuffers.DiscreteActions;
        int rotation = actions[0];
        int movement = actions[1];
        int attack = actions[2];

        currentAttackState = (AttackState) attack;
        currentMovementState = (MovementState) movement;
        currentRotationState = (RotationState) rotation;

        switch (currentRotationState) {
            case RotationState.LEFT:
                rb.transform.Rotate(Vector3.down, velocityRotateY);
                break;
            case RotationState.RIGTH:
                rb.transform.Rotate(Vector3.up, velocityRotateY);
                break;
        }

        switch (currentMovementState) {
            case MovementState.FORWARD:
                rb.velocity = this.transform.forward * velocityMovement;
                break;
            case MovementState.BACKWARD:
                rb.velocity = -this.transform.forward * velocityMovement;
                break;
        }

        if(currentAttackState == AttackState.ATTACK && timeLastBullet >= 1.0f / rateFire) {
            GameObject shellToSend = null;
            foreach (GameObject s in shells)
            {
                if (!s.activeSelf)
                {
                    shellToSend = s;
                }
            }
            if (shellToSend != null)
            {
                shellToSend.transform.position = this.spawnBullet.position;
                shellToSend.transform.rotation = this.transform.rotation;
                ShellController controller = shellToSend.GetComponent<ShellController>();
                controller.BoomBoom(healthController);
                timeLastBullet = 0;
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = (int) currentRotationState;
        actions[1] = (int) currentMovementState;
        actions[2] = (int) currentAttackState;
    }

    

    private void Update() {
        timeLastBullet += Time.deltaTime;
        if (behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly)
        {
            return;
        }
        MovementState currentMovement = currentMovementState;
        AttackState currentAttack = currentAttackState;
        RotationState currentRotation = currentRotationState;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentAttack = AttackState.ATTACK;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            currentAttack = AttackState.NONE;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentRotation = RotationState.RIGTH;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            currentRotation = RotationState.NONE;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentRotation = RotationState.LEFT;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            currentRotation = RotationState.NONE;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentMovement = MovementState.FORWARD;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            currentMovement = MovementState.NONE;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            currentMovement = MovementState.BACKWARD;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            currentMovement = MovementState.NONE;
        }

        currentMovementState = currentMovement;
        currentAttackState = currentAttack;
        currentRotationState = currentRotation;
    }
}
