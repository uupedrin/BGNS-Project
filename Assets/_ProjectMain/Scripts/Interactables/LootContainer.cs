using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class LootContainer : MonoBehaviour, IInteractable
{
    [SerializeField] private LootTableSO lootTable;
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private GameObject outlineSprite;

    [SerializeField] private float shakeStrength = 0.15f;
    [SerializeField] private float shakeDuration = 0.3f;

    [SerializeField] private SpriteRenderer containerRenderer;
    [SerializeField] private float disappearDelay = 0.7f;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool hasBeenLooted = false;

    Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        hasBeenLooted = false;
        outlineSprite.SetActive(false);
    }

    public void Interact(GameObject interactor)
    {
        if (hasBeenLooted) return;
        hasBeenLooted = true;

        transform.DOShakePosition(shakeDuration, shakeStrength);
        col.enabled = false;
        DropLoot();
        DisappearAfterDelay().Forget();
    }

    public void Highlight(bool active) => outlineSprite.SetActive(active);
    public bool ValidForInteraction() => !hasBeenLooted;

    private void DropLoot()
    {
        LootTableSO.LootEntry loot = lootTable.RollLoot();
        int amount = Random.Range(loot.minAmount, loot.maxAmount + 1);

        GameObject lootGO = ObjectPoolManager.Get(lootPrefab, transform.position, Quaternion.identity);
        lootGO.GetComponent<Loot>().SetItemData(loot.item, amount);
    }

    private async UniTask DisappearAfterDelay()
    {
        await UniTask.WaitForSeconds(disappearDelay);
        containerRenderer.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() => {
            ObjectPoolManager.Return(gameObject);
        });
    }
}
