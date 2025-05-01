using System.Collections.Generic;
using System.Collections;
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

    private void Start()
    {
        tank = GetComponent<TrackTank>();

        tracksFillImage = FindObjectOfType<UITrackIndicator>().FillImage;
        brokenIcon = FindObjectOfType<UITrackIndicator>().BrokenIcon;

        leftTrack.Destroyed += OnLeftTrackDestroyed;
        rightTrack.Destroyed += OnRightTrackDestroyed;
        leftTrack.Recovered += OnLeftTrackRecovered;
        rightTrack.Recovered += OnRightTrackRecovered;

        UpdateTracksUI();
    }

    private void OnDestroy()
    {
        leftTrack.Destroyed -= OnLeftTrackDestroyed;
        rightTrack.Destroyed -= OnRightTrackDestroyed;

        leftTrack.Recovered -= OnLeftTrackRecovered;
        rightTrack.Recovered -= OnRightTrackRecovered;
    }

    private void OnLeftTrackDestroyed(Destructible arg0)
    {
        ChangeActiveObjects(leftTrackMesh, leftTrackDamageMesh);
        
        TakeAwayMobility();
        
        UpdateTracksUI(); // Обновляем UI после уничтожения
    }

    private void OnLeftTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(leftTrackMesh, leftTrackDamageMesh);
        
        if (leftTrack.HitPoint > 0)
            RegainMobility();

        UpdateTracksUI(); // Обновляем UI после восстановления
    }

    private void OnRightTrackDestroyed(Destructible arg0)
    {
        ChangeActiveObjects(rightTrackMesh, rightTrackDamageMesh);
        
        TakeAwayMobility();
        
        //UpdateTracksUI(); // Обновляем UI после уничтожения
    }

    private void OnRightTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(rightTrackMesh, rightTrackDamageMesh);
        
        if (rightTrack.HitPoint > 0)
            RegainMobility();

                //UpdateTracksUI(); // Обновляем UI после восстановления
   }

   private void ChangeActiveObjects(GameObject a, GameObject b)
   {
       a.SetActive(b.activeSelf);
       b.SetActive(!b.activeSelf);
   }

   private void TakeAwayMobility()
   {
       tank.enabled = false;
   }

   private void RegainMobility()
   {
       tank.enabled = true;
   }
   
   private void UpdateTracksUI()
   {
       tracksFillImage.fillAmount = leftTrack.RecoveredTime;
       
       StartCoroutine(FillSlider());

       //brokenIcon.SetActive(leftBroken || rightBroken);

       //float leftFillAmount = leftTrack.HitPoint > 0 ? (float)leftTrack.HitPoint / leftTrack.MaxHitPoint : 0;
       //float rightFillAmount = rightTrack.HitPoint > 0 ? (float)rightTrack.HitPoint / rightTrack.MaxHitPoint : 0;

       //tracksFillImage.fillAmount = (leftFillAmount + rightFillAmount) / 2; 
   }

   private IEnumerator FillSlider()
   {
       bool leftBroken = leftTrack.HitPoint <= 0;
       bool rightBroken = rightTrack.HitPoint <= 0;
       
       while (leftBroken)
       {
           brokenIcon.SetActive(true);
           
           tracksFillImage.fillAmount = leftTrack.RecoveredTime;
           yield return new WaitForSeconds(0.05f);
       }
       
       while (rightBroken)
       {
           brokenIcon.SetActive(true);
           
           tracksFillImage.fillAmount = rightTrack.RecoveredTime;
           yield return new WaitForSeconds(0.05f);
       }
       
       brokenIcon.SetActive(false);
   }
}