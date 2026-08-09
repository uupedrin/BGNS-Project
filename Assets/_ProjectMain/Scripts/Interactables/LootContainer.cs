using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class LootContainer : MonoBehaviour, IInteractable
{
    [SerializeField] private string containerId;
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
    private Color originalColor;

    private CancellationTokenSource disappearCts;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        originalColor = containerRenderer.color;
    }

    private void OnEnable()
    {
        hasBeenLooted = false;
        outlineSprite.SetActive(false);
        col.enabled = true;
        containerRenderer.color = originalColor;

        disappearCts?.Cancel();
        disappearCts = new CancellationTokenSource();

        if(!string.IsNullOrEmpty(containerId) && SaveManager.pendingLoad && SaveData.current.worldData.collectedContainerIds.Contains(containerId))
        {
            gameObject.SetActive(false);
        }
    }

    public void Interact(GameObject interactor)
    {
        if (hasBeenLooted) return;
        hasBeenLooted = true;

        if (!string.IsNullOrEmpty(containerId) && !SaveData.current.worldData.collectedContainerIds.Contains(containerId))
        {
            SaveData.current.worldData.collectedContainerIds.Add(containerId);
        }

            transform.DOShakePosition(shakeDuration, shakeStrength);
        col.enabled = false;
        DropLoot();
        DisappearAfterDelay(disappearCts.Token).Forget();
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

    private async UniTask DisappearAfterDelay(CancellationToken token)
    {
        await UniTask.WaitForSeconds(disappearDelay, cancellationToken: token);
        containerRenderer.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() => {
            ObjectPoolManager.Return(gameObject);
        });
    }

    public void ForceDespawn()
    {
        if (hasBeenLooted) return;
        hasBeenLooted = true;
        outlineSprite.SetActive(false);
        col.enabled = false;
        DisappearAfterDelay(disappearCts.Token).Forget();
    }

    public void CancelDisappear() => disappearCts?.Cancel();
}
