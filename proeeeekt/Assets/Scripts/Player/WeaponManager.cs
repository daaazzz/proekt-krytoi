using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Status")]
    public bool hasSword = false;
    public bool isAttacking = false;
    public bool isReturning = false;

    [Header("References")]
    public GameObject weaponHolder;
    private PlayerController playerController;

    [Header("Damage Settings")]
    public Collider swordCollider; // Коллайдер на лезвии меча
    [SerializeField]public int attackDamage = 10;  // Урон 

    public PlayerStamina stamina;
    public float attackStaminaCost = 15f;

    [Header("НАСТРОЙКА АМПЛИТУДЫ")]
    [Range(0f, 180f)] public float strike1_Width = 120f;
    [Range(0f, 180f)] public float strike2_Width = 120f;
    [Range(0f, 180f)] public float strike3_Pitch = 85f;

    [Header("Timing")]
    public float attackDuration = 0.22f;
    public float returnDuration = 0.28f;
    public float comboResetWindow = 1.3f;

    private int comboStep = 0;
    private float lastAttackTime;
    private Vector3 idleAngles;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (weaponHolder != null)
        {
            idleAngles = weaponHolder.transform.localRotation.eulerAngles;
            weaponHolder.SetActive(false);
        }

        // Выключаем коллайдер меча на старте
        if (swordCollider != null)
        {
            swordCollider.isTrigger = true;
            swordCollider.enabled = false;
        }
    }

    void Update()
    {
        if (!hasSword) return;

        if (comboStep > 0 && !isAttacking && !isReturning)
        {
            if (Time.time - lastAttackTime > comboResetWindow || playerController.isRolling)
            {
                StartCoroutine(ReturnToIdleRoutine());
            }
        }

        if (isAttacking || isReturning || playerController.isRolling) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (stamina != null && stamina.TryConsume(attackStaminaCost))
            {
                StartCoroutine(PlayDynamicCombo());
            }
            else
            {
                Debug.Log("Недостаточно стамины для атаки!");
            }
        }
    }

    public void EquipSword()
    {
        hasSword = true;
        if (weaponHolder != null) weaponHolder.SetActive(true);
    }

    IEnumerator PlayDynamicCombo()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Включаем коллайдер меча во время взмаха
        if (swordCollider != null) swordCollider.enabled = true;

        Vector3 startAngles = weaponHolder.transform.localRotation.eulerAngles;
        Vector3 targetAngles = idleAngles;

        switch (comboStep)
        {
            case 0: targetAngles = idleAngles + new Vector3(15f, -strike1_Width, -20f); break;
            case 1: targetAngles = idleAngles + new Vector3(15f, strike2_Width, 20f); break;
            case 2: targetAngles = new Vector3(idleAngles.x + strike3_Pitch, idleAngles.y, idleAngles.z); break;
        }

        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            float t = elapsed / attackDuration;
            float smoothT = t * t * (3f - 2f * t);

            float x = Mathf.LerpAngle(startAngles.x, targetAngles.x, smoothT);
            float y = Mathf.LerpAngle(startAngles.y, targetAngles.y, smoothT);
            float z = Mathf.LerpAngle(startAngles.z, targetAngles.z, smoothT);

            weaponHolder.transform.localRotation = Quaternion.Euler(x, y, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        weaponHolder.transform.localRotation = Quaternion.Euler(targetAngles);

        // Выключаем коллайдер меча после взмаха
        if (swordCollider != null) swordCollider.enabled = false;

        comboStep++;

        if (comboStep >= 3)
        {
            isAttacking = false;
            StartCoroutine(ReturnToIdleRoutine());
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            isAttacking = false;
        }
    }

    IEnumerator ReturnToIdleRoutine()
    {
        isReturning = true;
        comboStep = 0;

        if (swordCollider != null) swordCollider.enabled = false;

        Vector3 startAngles = weaponHolder.transform.localRotation.eulerAngles;
        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            float x = Mathf.LerpAngle(startAngles.x, idleAngles.x, smoothT);
            float y = Mathf.LerpAngle(startAngles.y, idleAngles.y, smoothT);
            float z = Mathf.LerpAngle(startAngles.z, idleAngles.z, smoothT);

            weaponHolder.transform.localRotation = Quaternion.Euler(x, y, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        weaponHolder.transform.localRotation = Quaternion.Euler(idleAngles);
        isReturning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. ЗАЩИТА: Игнорируем себя и всё, что прикреплено к игроку
        if (other == swordCollider) return;
        if (other.transform.IsChildOf(this.transform) || other.gameObject == this.gameObject) return;

        // 2. Ищем скрипт здоровья на объекте, в который врезался меч
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        // 3. Если скрипт НАЙДЕН — значит это точно враг! Бьем его
        if (enemy != null)
        {
            Debug.Log("<color=red>[УДАР ПО ВРАГУ]</color> Наносим урон объекту: " + other.name);
            enemy.TakeDamage(attackDamage);
            StartCoroutine(HitStop(0.05f));
        }
    }
    IEnumerator HitStop(float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.1f; // Замедление времени
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale; // Возврат в нормальное время
    }
}
