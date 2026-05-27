using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Status")]
    public bool hasSword = false;
    public bool isAttacking = false;
    public bool isReturning = false; // Флаг: рука возвращается в стойку (блокирует спам)

    [Header("References")]
    public GameObject weaponHolder;
    private PlayerController playerController;

    [Header("Combo Settings")]
    public int comboStep = 0;
    public float comboResetWindow = 1.0f; // Сколько секунд меч «ждет» в конечной точке перед возвратом в стойку
    private float lastAttackTime;

    [Header("Timing")]
    public float strikeDuration = 0.14f;      // Скорость проноса меча при ударе
    public float returnToIdleDuration = 0.25f; // Скорость плавного убирания меча в стойку при сбросе комбо

    // Исходная позиция покоя (из Инспектора)
    private Vector3 idlePosition;
    private Quaternion idleRotation;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (weaponHolder != null)
        {
            idlePosition = weaponHolder.transform.localPosition;
            idleRotation = weaponHolder.transform.localRotation;
            weaponHolder.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasSword) return;

        // ЛОГИКА СБРОСА: Если мы застыли в комбо, но...
        if (comboStep > 0 && !isAttacking && !isReturning)
        {
            // ...вышло время ожидания ИЛИ игрок нажал перекат
            if (Time.time - lastAttackTime > comboResetWindow || playerController.isRolling)
            {
                StartCoroutine(ReturnToIdleRoutine());
            }
        }

        // ЖЕСТКИЙ ЗАПРЕТ НА АТАКУ (Защита от сбива анимации):
        // Нельзя бить, если: уже бьем, если рука возвращается в стойку, или если мы катимся
        if (isAttacking || isReturning || playerController.isRolling) return;

        // Нажатие ЛКМ — всегда начинает комбо с текущего шага
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(PlayFlowingCombo());
        }
    }

    public void EquipSword()
    {
        hasSword = true;
        if (weaponHolder != null)
        {
            weaponHolder.SetActive(true);
        }
    }

    IEnumerator PlayFlowingCombo()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // КРИТИЧЕСКИЙ МОМЕНТ: Стартуем НЕ из idle, а из ТЕКУЩЕГО положения меча на экране!
        Vector3 startPosition = weaponHolder.transform.localPosition;
        Quaternion startRotation = weaponHolder.transform.localRotation;

        // Конечные точки, где меч ОСТАНОВИТСЯ после удара
        Vector3 targetPosition = idlePosition;
        Quaternion targetRotation = idleRotation;

        switch (comboStep)
        {
            case 0:
                // УДАР 1: Меч разрубает воздух справа-налево и ОСТАЕТСЯ слева внизу экрана
                targetPosition = idlePosition + new Vector3(-0.5f, -0.2f, 0.2f);
                targetRotation = idleRotation * Quaternion.Euler(20f, -60f, 35f);
                break;

            case 1:
                // УДАР 2: Меч идёт НАЗАД (слева-направо) из точки УДАРА 1 и застывает справа вверху
                targetPosition = idlePosition + new Vector3(0.4f, 0.3f, 0.1f);
                targetRotation = idleRotation * Quaternion.Euler(-10f, 50f, -40f);
                break;

            case 2:
                // УДАР 3: Финальный удар сверху-вниз прямо из правого верхнего угла в пол
                targetPosition = idlePosition + new Vector3(0.0f, -0.6f, 0.4f);
                targetRotation = idleRotation * Quaternion.Euler(70f, 0f, 0f);
                break;
        }

        float elapsed = 0f;

        // Сама фаза удара (пронос оружия)
        while (elapsed < strikeDuration)
        {
            float t = elapsed / strikeDuration;
            float curve = Mathf.SmoothStep(0f, 1f, t); // Плавный, сочный взмах

            weaponHolder.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, curve);
            weaponHolder.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, curve);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Жестко фиксируем меч в конечной точке — он замирает и ждет!
        weaponHolder.transform.localPosition = targetPosition;
        weaponHolder.transform.localRotation = targetRotation;

        comboStep++;

        // Если это был последний (3-й) удар комбо — автоматически запускаем возврат в стойку
        if (comboStep >= 3)
        {
            isAttacking = false;
            StartCoroutine(ReturnToIdleRoutine());
        }
        else
        {
            // Если комбо продолжается, даем микро-паузу (0.05 сек) для тактильного отклика и разрешаем следующий клик
            yield return new WaitForSeconds(0.05f);
            isAttacking = false;
        }
    }

    // Мягкое возвращение меча в дефолтное положение (Стойку)
    IEnumerator ReturnToIdleRoutine()
    {
        isReturning = true;
        comboStep = 0; // Следующая атака гарантированно начнется с УДАРА 1

        Vector3 startPosition = weaponHolder.transform.localPosition;
        Quaternion startRotation = weaponHolder.transform.localRotation;

        float elapsed = 0f;

        while (elapsed < returnToIdleDuration)
        {
            float t = elapsed / returnToIdleDuration;
            float curve = Mathf.SmoothStep(0f, 1f, t);

            weaponHolder.transform.localPosition = Vector3.Lerp(startPosition, idlePosition, curve);
            weaponHolder.transform.localRotation = Quaternion.Slerp(startRotation, idleRotation, curve);

            elapsed += Time.deltaTime;
            yield return null;
        }

        weaponHolder.transform.localPosition = idlePosition;
        weaponHolder.transform.localRotation = idleRotation;

        isReturning = false; // Меч дома, кулдаун снят, можно снова воевать!
    }
}