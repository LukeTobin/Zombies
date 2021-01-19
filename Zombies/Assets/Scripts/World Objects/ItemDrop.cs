using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{   
    [SerializeField] Color glowColor = new Color(0, 0, 0, 255);

    SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();    
        spriteRenderer.color = glowColor;
    }

    void Update(){
        transform.Rotate(Vector3.up*1f, 360f * Time.deltaTime / 1f);
    }

    public void PickupItem(){
        ItemPickupEffect();
        gameObject.SetActive(false);
    }

    public static void ItemPickupEffect(){
        
    }
}
