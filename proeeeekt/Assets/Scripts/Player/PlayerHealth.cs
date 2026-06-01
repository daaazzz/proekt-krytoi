using UnityEngine;
using UnityEngine.UI; // Для работы со Slider и Text
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Характеристики Здоровья")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Интерфейс (UI)")]
    public Slider healthSlider;
    public TMP_Text estusText; // Теперь это TMP_Text

    [Header("Настройки Эстуса (Хилки)")]
    public int estusMaxCharges = 3;
    private int currentEstusCharges;
    public int estusHealAmount = 40;

    void Start()
    {
        currentHealth = maxHealth;
        currentEstusCharges = estusMaxCharges;

        // Настраиваем полоску UI
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        // Обновляем текст эстуса при старте игры
        UpdateEstusUI();

        Debug.Log($"[ИГРОК] Старт. HP: {currentHealth}. Хилок: {currentEstusCharges}");
    }

    void Update()
    {
        // Лечение на клавишу 'R'
        if (Input.GetKeyDown(KeyCode.R))
        {
            UseEstus();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log($"[РАНЕНИЕ] HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void UseEstus()
    {
        if (currentHealth >= maxHealth) return;

        // Если глотков нет, текст может моргнуть (или просто ничего не делать)
        if (currentEstusCharges <= 0)
        {
            Debug.Log("Эстус закончился!");
            return;
        }

        currentEstusCharges--;
        currentHealth += estusHealAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        // Обновляем слайдер здоровья
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // ОБНОВЛЯЕМ ТЕКСТ НА ЭКРАНЕ
        UpdateEstusUI();

        Debug.Log($"[ЛЕЧЕНИЕ] Хилок осталось: {currentEstusCharges}");
    }

    // Отдельный маленький метод, который красиво обновляет надпись
    void UpdateEstusUI()
    {
        if (estusText != null)
        {
            estusText.text = ": " + currentEstusCharges;
        }
    }

    void Die()
    {
        Debug.Log("ИГРОК ПОГИБ!");
    }
}