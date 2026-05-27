using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, есть ли у объекта, который наступил на меч, компонент WeaponManager
        WeaponManager weaponManager = other.GetComponent<WeaponManager>();

        if (weaponManager != null)
        {
            // Вызываем метод экипировки меча
            weaponManager.EquipSword();

            // Удаляем меч с пола, так как он теперь в руках
            Destroy(gameObject);
        }
    }
}
