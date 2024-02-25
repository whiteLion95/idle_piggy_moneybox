using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchasedGoalMenuBlock : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;

    public void Init(Sprite sprite, string name)
    {
        icon.sprite = sprite;
        nameText.text = name;
    }
}