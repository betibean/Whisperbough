using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog : MonoBehaviour
{
    public static ConfirmDialog Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text message;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        noButton.onClick.AddListener(Close);
    }

    public void Show(string text, Action onConfirm)
    {
        message.text = text;
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { Close(); onConfirm?.Invoke(); });
        panel.SetActive(true);
    }

    private void Close() => panel.SetActive(false);
}