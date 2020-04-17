using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ActiveItemContainer : MonoBehaviour
{
    public RectTransform mask;
    public RectTransform green;
    public Image subdivisions;

    [SerializeField]
    [Range(0, 1)]
    private float maskPercent = 1f;

    private Vector3 initialLocalPosition;
    private float height;

    private void Awake()
    {
        initialLocalPosition = mask.localPosition;
        height = green.rect.height;
    }

    private void Update()
    {
        if(Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            maskPercent = 1f;
            Masking();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            maskPercent = 0.5f;
            Masking();
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            maskPercent = 0.3f;
            Masking();
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            maskPercent = 0f;
            Masking();
        }
    }

    private void Masking()
    {
        green.SetParent(transform);
        mask.localPosition = initialLocalPosition - new Vector3(0f, height * (1f - maskPercent), 0f);
        green.SetParent(mask);
    }
}
