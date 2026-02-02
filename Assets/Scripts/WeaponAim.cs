using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    [Header("Configurações")]
    public float orbitRadius = 1.0f; // Distância da arma em relação ao personagem

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Se a arma não tiver um pai (WeaponHolder), não faz nada
        if (transform.parent == null) return;

        HandleAiming();
    }

    void HandleAiming()
    {
        // 1. Pega a posição do mouse no mundo
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Garante que o Z do mouse seja 0

        // 2. Descobre a origem (O pivô/WeaponHolder)
        Vector3 origin = transform.parent.position;

        // 3. Calcula a direção (Do personagem -> Mouse)
        Vector3 direction = (mousePos - origin).normalized;

        // --- PARTE NOVA: ORBITAR (OFFSET) ---
        // Move a arma para a direção do mouse, multiplicada pela distância (Raio)
        transform.position = origin + (direction * orbitRadius);

        // 4. Calcula o ângulo para a rotação da arma
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 5. Espelhar a arma (Flip Y) para não ficar de cabeça para baixo
        if (Mathf.Abs(angle) > 90)
        {
            sr.flipY = true;

            // Opcional: Ajuste fino se o sprite da arma não for centralizado verticalmente
            // sr.sortingOrder = 1; // Ex: Coloca atrás do player quando mira pra cima/traz (opcional)
        }
        else
        {
            sr.flipY = false;
        }
    }
}