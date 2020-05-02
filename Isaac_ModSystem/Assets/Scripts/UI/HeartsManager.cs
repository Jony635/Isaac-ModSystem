using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsManager : MonoBehaviour
{
    public static HeartsManager Instance;

    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    private void Awake()
    {
        Instance = this;
    }

    public void OnCharacterHealthChanged()
    {
        float hearts = PlayerController.Instance.stats.hp / 2f - 1;
      
        for(int i = 0; i < transform.childCount; ++i)
        {
            if(i <= hearts)           
                transform.GetChild(i).GetComponent<Image>().sprite = fullHeart;          
            else if(i - 0.5 == hearts)
                transform.GetChild(i).GetComponent<Image>().sprite = halfHeart;
            else
                transform.GetChild(i).GetComponent<Image>().sprite = emptyHeart;
        }    
    }

    public void OnCharacterMaxHealthChanged()
    {
        float hearts = PlayerController.Instance.stats.maxHp / 2f;
        hearts = Mathf.Floor(hearts);

        if (hearts < 1) hearts = 1;

        if (hearts == transform.childCount)
            return;

        if(hearts < transform.childCount)
        {
            int destroyedHearts = 0;

            while (destroyedHearts < transform.childCount - hearts)
            {
                Destroy(transform.GetChild(transform.childCount - destroyedHearts - 1).gameObject);
                destroyedHearts++;
            }
        }
        else if(hearts > transform.childCount)
        {
            while (transform.childCount < hearts)
            {
                Instantiate(transform.GetChild(0).gameObject, this.transform);
            }
        }
    }
}
