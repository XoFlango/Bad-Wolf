using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    // [HideInInspector] evita que o Unity tente desenhar o array e cause o erro
    // Se você precisar ver o debug, remova essa linha, mas saiba que o erro é visual.
    [HideInInspector]
    public WeaponController[] weaponSlots = new WeaponController[3];

    void Start()
    {
        // Garante que o array existe e tem 3 espaços vazios
        if (weaponSlots == null || weaponSlots.Length != 3)
        {
            weaponSlots = new WeaponController[3];
        }
    }

    void Update()
    {
        // Teclas 1, 2, 3
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);
    }

    public void EquipWeapon(int index)
    {
        // Se o slot estiver vazio, não faz nada
        if (weaponSlots[index] == null) return;

        // Desativa todas as armas
        foreach (var w in weaponSlots)
        {
            if (w != null) w.gameObject.SetActive(false);
        }

        // Ativa a escolhida
        weaponSlots[index].gameObject.SetActive(true);
    }

    public void CollectWeapon(WeaponController newWeapon)
    {
        int slotIndex = (int)newWeapon.weaponType;

        // Se já tem arma no slot, substitui (a lógica antiga é destruída ou jogada fora)
        weaponSlots[slotIndex] = newWeapon;

        // Configuração Visual (Traz para a mão)
        newWeapon.transform.SetParent(transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        // Equipa automaticamente a arma que acabou de pegar
        EquipWeapon(slotIndex);
    }

    // A CURA DO ERRO: Método para limpar o slot explicitamente
    public void RemoveWeaponFromInventory(WeaponController weaponToRemove)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            // Se achar a arma no array, anula o slot
            if (weaponSlots[i] == weaponToRemove)
            {
                weaponSlots[i] = null;
                return; // Encerra assim que achar
            }
        }
    }
}