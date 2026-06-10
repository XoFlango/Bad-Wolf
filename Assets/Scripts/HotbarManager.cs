using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    [System.Serializable]
    public class HotbarUISlot
    {
        public Image iconImage;       // A foto da arma na interface
        public GameObject outline;    // A borda de seleção amarela/branca
    }

    [Header("Conexão com o Player")]
    public WeaponInventory playerInventory;

    [Header("Configuração da UI")]
    public HotbarUISlot[] uiSlots = new HotbarUISlot[3];

    private int currentSelectedIndex = -1;

    void Update()
    {
        if (playerInventory == null) return;

        UpdateIcons();
        UpdateSelection();
    }

    // ================================================================
    // A MÁGICA DA TROCA (Lê o array do seu inventário)
    // ================================================================
    void UpdateIcons()
    {
        // O loop varre os 3 slots do WeaponInventory e espelha na UI
        for (int i = 0; i < 3; i++)
        {
            WeaponController weaponInSlot = playerInventory.weaponSlots[i];

            if (weaponInSlot != null)
            {
                // TEM ARMA: Puxa o ícone oficial que você configurou no WeaponController da arma
                uiSlots[i].iconImage.sprite = weaponInSlot.iconUI;
                uiSlots[i].iconImage.color = new Color(1, 1, 1, 1); // Fica 100% visível
            }
            else
            {
                // NÃO TEM ARMA (foi jogada fora ou está vazio): Apaga o ícone
                uiSlots[i].iconImage.sprite = null;
                uiSlots[i].iconImage.color = new Color(1, 1, 1, 0); // Fica invisível
            }
        }
    }

    // ================================================================
    // ESPELHA QUAL ARMA ESTÁ NA MÃO AGORA
    // ================================================================
    void UpdateSelection()
    {
        int activeIndex = -1;

        for (int i = 0; i < 3; i++)
        {
            // Se a arma existe no array e o GameObject dela está ativo, ela está na mão
            if (playerInventory.weaponSlots[i] != null && playerInventory.weaponSlots[i].gameObject.activeSelf)
            {
                activeIndex = i;
                break;
            }
        }

        if (activeIndex != currentSelectedIndex)
        {
            currentSelectedIndex = activeIndex;

            for (int i = 0; i < 3; i++)
            {
                if (uiSlots[i].outline != null)
                {
                    uiSlots[i].outline.SetActive(i == currentSelectedIndex);
                }
            }
        }
    }
}