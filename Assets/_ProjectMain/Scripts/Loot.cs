using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class Loot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer itemRenderer;
    private CircleCollider2D itemCollider;
    [SerializeField] private float collectSpeed = 7f;

    [SerializeField] private string playerTag = "Player";

    bool onCooldown = false;
    bool isCollecting = false;
    [SerializeField] private float itemCollectionCooldown = 1f;

    [SerializeField] private ItemSO item;
    [SerializeField] private int itemAmount = 1;

    private void Start()
    {
        if(item != null)
        {
            SetItemData(item);
        }
    }

    private void OnEnable()
    {
        onCooldown = false;
        isCollecting = false;
    }

    public void SetItemData(ItemSO item)
    {
        this.item = item;
        itemRenderer.sprite = item.itemSprite;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (onCooldown || isCollecting) return;
        if (collision.CompareTag(playerTag))
        {
            _ = MoveAndCollect(collision.transform);
        }
    }

    private async UniTask MoveAndCollect(Transform target)
    {
        isCollecting = true;
        
        while(Vector2.Distance(transform.position, target.position) > 0.2f)
        {
            if (target == null) break;
            transform.position = Vector3.MoveTowards(transform.position, target.position, collectSpeed * Time.deltaTime);
            await UniTask.Yield();
        }

        int collectedAmount = InventoryManager.Instance.AddItem(item, itemAmount);

        itemAmount -= collectedAmount;

        bool collected = itemAmount <= 0;
        if (collected)
        {
            //ObjectPoolManager.Return(gameObject);
            Destroy(gameObject);
            isCollecting = false;
        }
        else
        {
            onCooldown = true;
            isCollecting = false;
            await UniTask.Delay(TimeSpan.FromSeconds(itemCollectionCooldown));
            onCooldown = false;
        }
    }
}
