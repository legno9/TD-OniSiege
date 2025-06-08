using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyVisualHP : MonoBehaviour
{
    
    [SerializeField] private HealthSystem enemyHealth;
    [SerializeField] private SpriteRenderer healthBarSpriteRenderer;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private float activeTimeOnHit = 3f;
    [SerializeField] private float fadeTime = 0.5f;
    
    private float _activeTimer = 0f;
    private Coroutine _fadeCoroutine;
    private bool _fadedOut = false;

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        if (enemyHealth)
        {
            enemyHealth.OnHealthChanged += UpdateHealthBar;
        }
        ResetFadeCoroutine();
        _activeTimer = 0f;
        SetAlpha(0f);
        healthBarSpriteRenderer.gameObject.SetActive(true);
        backgroundSpriteRenderer.gameObject.SetActive(true);
        _fadedOut = false;
    }

    private void OnDisable()
    {
        if (enemyHealth)
        {
            enemyHealth.OnHealthChanged -= UpdateHealthBar;
        }
        ResetFadeCoroutine();
    }
    
    private void Update()
    {
        _activeTimer += Time.deltaTime;

        if (!(_activeTimer >= activeTimeOnHit)) return;
        if (!healthBarSpriteRenderer || !healthBarSpriteRenderer) return;
        if (_fadeCoroutine != null) return;
        
        _fadeCoroutine = StartCoroutine(FadeCoroutine(0f));
        _fadedOut = true;
    }

    private void UpdateHealthBar(float maxHealth, float currentHealth )
    {
        _activeTimer = 0f;
        
        if (GetAlpha() == 0f || _fadedOut)
        {
            _fadedOut = false;
            SetAlpha(0f);
            ResetFadeCoroutine();
            _fadeCoroutine = StartCoroutine(FadeCoroutine(1f));
        }
        
        float healthPercentage = currentHealth / maxHealth;

        Vector3 newScale = healthBarSpriteRenderer.transform.localScale;
        newScale.x = healthPercentage;
        healthBarSpriteRenderer.transform.localScale = newScale;

        if (healthPercentage >= 0) return;
        
        healthBarSpriteRenderer.gameObject.SetActive(false);
        backgroundSpriteRenderer.gameObject.SetActive(false);
    }
    
    private void ResetFadeCoroutine()
    {
        if (_fadeCoroutine == null) return;
        StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = null;
    }

    private IEnumerator FadeCoroutine(float endValue)
    {
        float timer = 0f;
        float startValue = healthBarSpriteRenderer.color.a;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeTime;
            float alpha = Mathf.Lerp(startValue, endValue, progress);

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(endValue);
        _fadeCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color fillColor = healthBarSpriteRenderer.color;
        fillColor.a = alpha;
        healthBarSpriteRenderer.color = fillColor;
        
        Color bgColor = backgroundSpriteRenderer.color;
        bgColor.a = alpha;
        backgroundSpriteRenderer.color = bgColor;
    }
    
    private float GetAlpha()
    {
        return healthBarSpriteRenderer.color.a;
    }
    
}
