using UnityEngine;
using System.Collections.Generic; // Para usar Dicionários

public class PlayerAmmoInventory : MonoBehaviour
{
    // Dicionário para guardar munição por TIPO de arma
    // Ex: [Pistol] = 10, [AR] = 60...
    private Dictionary<WeaponType, int> ammoPouch = new Dictionary<WeaponType, int>();

    void Awake()
    {
        // Inicializa o bolso com 0 para todas as armas
        ammoPouch.Add(WeaponType.Pistol, 0);
        ammoPouch.Add(WeaponType.AssaultRifle, 0);
        ammoPouch.Add(WeaponType.Shotgun, 0);
    }

    // Método para ADICIONAR munição (Vem da Caixa)
    public void AddAmmo(WeaponType type, int amount)
    {
        if (ammoPouch.ContainsKey(type))
        {
            ammoPouch[type] += amount;
            Debug.Log($"Guardou {amount} balas de {type}. Total no bolso: {ammoPouch[type]}");
        }
    }

    // Método para GASTAR munição (Vem da Recarga da Arma)
    // Retorna quantas balas conseguiu entregar
    public int TakeAmmo(WeaponType type, int amountWanted)
    {
        if (!ammoPouch.ContainsKey(type)) return 0;

        int currentReserve = ammoPouch[type];

        if (currentReserve >= amountWanted)
        {
            ammoPouch[type] -= amountWanted;
            return amountWanted; // Entregou tudo que pediu
        }
        else
        {
            ammoPouch[type] = 0; // Zerou o bolso
            return currentReserve; // Entregou só o que tinha
        }
    }

    // Método para CONSULTAR (Para UI ou Debug)
    public int GetAmmoCount(WeaponType type)
    {
        return ammoPouch.ContainsKey(type) ? ammoPouch[type] : 0;
    }
}