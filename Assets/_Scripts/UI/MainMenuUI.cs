using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recordText;

    public void SetRecordText(int value)
    {
        recordText.text = "Record: " + value;
    }
}