using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthUIManager : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;
    TextMeshProUGUI textMesh;

    private void Awake()
    {
        //playerManager = GetComponent<PlayerManager>();
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        textMesh.text = playerManager.health.ToString();
    }
}
