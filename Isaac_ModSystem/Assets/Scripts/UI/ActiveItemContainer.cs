using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor;

public class ActiveItemContainer : MonoBehaviour
{
    public static ActiveItemContainer Instance;

    public Image activeItemIcon;
    public RectTransform mask;
    public RectTransform green;
    public Image subdivisions;

    public GameObject chargesGO;

    public List<Sprite> subdivisionSprites;

    [SerializeField]
    [Range(0, 1)]
    private float maskPercent = 1f;

    private Vector3 initialLocalPosition;
    private float height;
    private ActiveItem equippedItem;

    private void Awake()
    {
        Instance = this;

        initialLocalPosition = mask.localPosition;
        height = green.rect.height;
    }

    private void Update()
    {
        
    }

    private void Masking()
    {
        green.SetParent(transform);
        mask.localPosition = initialLocalPosition - new Vector3(0f, height * (1f - maskPercent), 0f);
        green.SetParent(mask);
    }

    public int GetCurrentCharges()
    {
        return equippedItem ? (int)equippedItem.currentCharges : -1;
    }

    public ActiveItem GetActiveItem()
    {
        return equippedItem;
    }

    public void SetActivePercent(float percent)
    {
        if (equippedItem == null)
            return;

        maskPercent = percent;

        equippedItem.currentCharges = (uint)maskPercent * equippedItem.numCharges;

        Masking();
    }

    public void ActiveItemEquipped(ActiveItem item)
    {
        equippedItem = item;

        activeItemIcon.gameObject.SetActive(true);
        chargesGO.gameObject.SetActive(true);

        activeItemIcon.sprite = item.sprite;
        item.equiped = true;

        subdivisions.sprite = subdivisionSprites[(int)item.numCharges - 1];

        maskPercent = (float)equippedItem.currentCharges / (float)equippedItem.numCharges;
        Masking();
    }

    public void ActiveItemUsed()
    {
        if(equippedItem.currentCharges == equippedItem.numCharges)
        {
            equippedItem.currentCharges = 0;
            maskPercent = 0f;
            Masking();
            equippedItem.OnUsed();
        }
    }

    public void OnNewRoomCleared()
    {      
        equippedItem.currentCharges = (uint)Mathf.Clamp(equippedItem.currentCharges + 1, 0, equippedItem.numCharges);
        maskPercent = (float)equippedItem.currentCharges / (float)equippedItem.numCharges;
        Masking();
    }
}
