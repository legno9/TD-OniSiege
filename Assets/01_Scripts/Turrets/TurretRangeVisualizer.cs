using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TurretRangeVisualizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float baseVisualRadius = 0.5f;
    [SerializeField] private AllTurretsConfig allTurretsConfig;
    
    private void Awake()
    {
        Hide();
    }

    public void Show(TurretType turretType, Vector2 position, float range = 0f)
    {
        if (!(range > 0f))
        {
            if (!allTurretsConfig || !allTurretsConfig.TurretConfig.TryGetValue(turretType, out var config))
            {
                Debug.LogError($"No turret config found for type: {turretType}");
                return;
            }
            range = config.levelsData[0].actionRange;
        }
        
        transform.position = position;
        float scaleFactor = range * baseVisualRadius;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

        spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        spriteRenderer.enabled = false;
    }
}
