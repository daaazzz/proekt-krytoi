using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Характеристики Врага")]
    // Переменная здоровья куба в инспекторе Врага
    public int health = 100;

    private float damageCooldown = 0.25f;
    private float lastDamageTime;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что коснулись меча
        if (other.name.Contains("Sword") || other.CompareTag("Weapon") || other.name.ToLower().Contains("collider"))
        {
            // Защита от двойного касания в одном кадре
            if (Time.time - lastDamageTime < damageCooldown) return;

            // Ищем скрипт WeaponManager на объекте, который нас ударил (или на его родителе)
            WeaponManager manager = other.GetComponentInParent<WeaponManager>();

            if (manager != null)
            {
                lastDamageTime = Time.time;

                // БЕРЕМ УРОН: вытаскиваем именно attackDamage из WeaponManager!
                int damageFromWeapon = manager.attackDamage;

                // Вызываем метод получения урона
                TakeDamage(damageFromWeapon);
            }
            else
            {
                Debug.LogWarning("[ОШИБКА] Меч коснулся куба, но скрипт WeaponManager на Игроке не найден!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"<color=red>[УРОН]</color> Куб получил {damage} урона. Осталось HP: {health}");

        if (health <= 0)
        {
            Debug.Log("Куб уничтожен!");
            Destroy(gameObject);
        }
    }
}