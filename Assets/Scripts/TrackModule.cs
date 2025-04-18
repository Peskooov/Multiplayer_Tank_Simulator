using Mirror;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TrackTank))]
public class TrackModule : NetworkBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject leftTrackMesh;
    [SerializeField] private GameObject leftTrackDamageMesh;
    [SerializeField] private GameObject rightTrackMesh;
    [SerializeField] private GameObject rightTrackDamageMesh;

    [Header("UI")]
    [SerializeField] private Image tracksFillImage; // Общее изображение для обеих гусениц
    [SerializeField] private GameObject brokenIcon;
    
    [Space(5)]
    [SerializeField] private VehicleModule leftTrack;
    [SerializeField] private VehicleModule rightTrack;

    private TrackTank tank;
    private bool anyTrackRepairing;
    
    private void Start()
    {
        tank = GetComponent<TrackTank>();

        tracksFillImage = FindObjectOfType<UITrackIndicator>().FillImage;
        brokenIcon = FindObjectOfType<UITrackIndicator>().BrokenIcon;
        
        leftTrack.Destroyed += OnTrackDestroyed;
        rightTrack.Destroyed += OnTrackDestroyed;
        leftTrack.Recovered += OnTrackRecovered;
        rightTrack.Recovered += OnTrackRecovered;
        
        UpdateTracksUI();
    }

  private void Update()
    {
        bool repairing = false;
        float totalFill = 0f;
        int repairingTracks = 0;

        // Проверяем состояние обеих гусениц
        if (leftTrack.enabled)
        {
            repairing = true;
            repairingTracks++;
            totalFill += 1 - (leftTrack.remainingRecoveryTime / leftTrack.RecoveredTime);
        }

        if (rightTrack.enabled)
        {
            repairing = true;
            repairingTracks++;
            totalFill += 1 - (rightTrack.remainingRecoveryTime / rightTrack.RecoveredTime);
        }

        if (repairingTracks > 0)
        {
            tracksFillImage.fillAmount = totalFill / repairingTracks;
        }

        // Обновляем иконку поломки
        bool anyBroken = leftTrack.HitPoint <= 0 || rightTrack.HitPoint <= 0;
        brokenIcon.SetActive(anyBroken);

        anyTrackRepairing = repairing;
    }

    private void OnTrackDestroyed(Destructible destructible)
    {
        if (destructible == leftTrack)
        {
            ChangeActiveObjects(leftTrackMesh, leftTrackDamageMesh);
        }
        else
        {
            ChangeActiveObjects(rightTrackMesh, rightTrackDamageMesh);
        }
        
        TakeAwayMobility();
        UpdateTracksUI();
    }

    private void OnTrackRecovered(Destructible destructible)
    {
        if (destructible == leftTrack)
        {
            ChangeActiveObjects(leftTrackMesh, leftTrackDamageMesh);
        }
        else
        {
            ChangeActiveObjects(rightTrackMesh, rightTrackDamageMesh);
        }
        
        UpdateTracksUI();
        
        if (leftTrack.HitPoint > 0 && rightTrack.HitPoint > 0)
        {
            RegainMobility();
        }
    }

    private void UpdateTracksUI()
    {
        bool leftBroken = leftTrack.HitPoint <= 0;
        bool rightBroken = rightTrack.HitPoint <= 0;
        
        // Если хотя бы одна гусеница сломана - показываем иконку
        brokenIcon.SetActive(leftBroken || rightBroken);
        
        // Устанавливаем fillAmount в 0, если хотя бы одна гусеница сломана
        // Иначе - 1 (полностью заполнено)
        tracksFillImage.fillAmount = (leftBroken || rightBroken) ? 0 : 1;
    }

    private void ChangeActiveObjects(GameObject normalMesh, GameObject damageMesh)
    {
        normalMesh.SetActive(!damageMesh.activeSelf);
        damageMesh.SetActive(!normalMesh.activeSelf);
    }

    private void TakeAwayMobility()
    {
        tank.enabled = false;
    }

    private void RegainMobility()
    {
        tank.enabled = true;
    }

    private void OnDestroy()
    {
        leftTrack.Destroyed -= OnTrackDestroyed;
        rightTrack.Destroyed -= OnTrackDestroyed;
        leftTrack.Recovered -= OnTrackRecovered;
        rightTrack.Recovered -= OnTrackRecovered;
    }
}