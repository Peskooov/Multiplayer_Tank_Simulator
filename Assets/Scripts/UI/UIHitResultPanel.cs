using UnityEngine;

public class UIHitResultPanel : MonoBehaviour
{
    [SerializeField] private Transform spawnPanel;
    [SerializeField] private UIHitResultPopup hitResultPopup;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
    }

    private void OnDestroy()
    {
//        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
    }

    private void OnMatchStart()
    {
        Player.Local.ProjectileHit += OnProjectileHit;
    }

    private void OnProjectileHit(ProjectileHitResult hitResult)
    {
        if (hitResult.Type == ProjectileHitType.Environment || 
            hitResult.Type == ProjectileHitType.ModuleNoPenetration || 
            hitResult.Type == ProjectileHitType.ModulePenetration) 
            return;

        // Создаем и настраиваем попап
        UIHitResultPopup hitPopup = Instantiate(hitResultPopup, spawnPanel);
        hitPopup.transform.localScale = Vector3.one;
        hitPopup.transform.position = VehicleCamera.Instance.Camera.WorldToScreenPoint(hitResult.Point);
        
        // Устанавливаем текст в зависимости от типа попадания
        switch (hitResult.Type)
        {
            case ProjectileHitType.Penetration:
                hitPopup.SetTypeResult("Пробитие!");
                break;
            case ProjectileHitType.Ricochet:
                hitPopup.SetTypeResult("Рикошет!");
                break;
            case ProjectileHitType.NonPenetration:
                hitPopup.SetTypeResult("Броня не пробита!");
                break;
        }

        // Отображаем урон
        hitPopup.SetDamageResult(hitResult.Damage);
    }
}
