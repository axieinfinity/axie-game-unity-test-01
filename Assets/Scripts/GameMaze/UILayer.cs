using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public string key;
    public GameObject frame;
    public TMPro.TextMeshProUGUI amoutText;
}

public class UILayer : MonoBehaviour
{
    [SerializeField] GameObject frameResult;
    [SerializeField] TMPro.TextMeshProUGUI resultText;
    [SerializeField] InventorySlot[] inventorys;

    public void SetInventoryStates(Dictionary<string, int> consumableItems)
    {
        foreach(var p in inventorys)
        {
            if(consumableItems.TryGetValue(p.key, out var val) && val > 0)
            {
                p.frame.SetActive(true);
                p.amoutText.text = $"{val}";
            }
            else
            {
                p.frame.SetActive(false);
            }
        }
    }

    public void SetResultFrame(bool b)
    {
        if (this.frameResult != null)
        {
            this.frameResult.SetActive(b);
        }
    }

    public void SetResultText(bool isWon)
    {
        if (this.resultText != null)
        {
            if (isWon)
            {
                this.resultText.text = "YOU WIN!!!";
            }
            else
            {
                this.resultText.text = "YOU LOSE!!!";
            }
        }
    }
}
