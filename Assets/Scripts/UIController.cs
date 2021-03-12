using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text txtStepCount;
    [SerializeField] private Text txtEpisode;
    [SerializeField] private Text txtTotalStepCount;
    void FixedUpdate() {
        txtStepCount.text = "Step count: " + Academy.Instance.StepCount.ToString();
        txtTotalStepCount.text = "Total step count: " + Academy.Instance.TotalStepCount.ToString();
        txtEpisode.text = "Episode count: " + Academy.Instance.EpisodeCount.ToString();
    }
}
