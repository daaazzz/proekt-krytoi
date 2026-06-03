using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("PlayerStamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float regenRate = 20f;

    [Header("Настройки задержки")]
    public float regenDelay = 1.0f; // Задержка в 1 секунду
    private float regenTimer = 0f;

    [Header("UI")]
    public Slider staminaSlider;

    void Start()
    {
        currentStamina = maxStamina;
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
    }

    void Update()
    {
        // Если таймер задержки еще идет, уменьшаем его
        if (regenTimer > 0)
        {
            regenTimer -= Time.deltaTime;
        }
        else
        {
            // Регенерация работает только если таймер истек
            if (currentStamina < maxStamina)
            {
                currentStamina += regenRate * Time.deltaTime;
            }
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }

    public bool TryConsume(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;

            // Сбрасываем таймер при каждом расходе стамины
            regenTimer = regenDelay;

            return true;
        }
        return false;
    }
}