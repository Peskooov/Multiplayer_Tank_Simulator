using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerColoPalette : NetworkBehaviour
{
   public static PlayerColoPalette Instance;

   [SerializeField] private List<Color> allColors;

   private List<Color> avalibaleColors;
   
      private void Awake()
   {  
      if (Instance != null)
      {
         Destroy(gameObject);
         return;
      }

      Instance = this;

      avalibaleColors = new List<Color>();
      allColors.CopyTo(avalibaleColors);
   }

   public Color TakeRandomColor()
   {
      int index = Random.Range(0, avalibaleColors.Count);
      Color color = avalibaleColors[index];

      avalibaleColors.RemoveAt(index);

      return color;
   }

   public void PutColor(Color color)
   {
      if (allColors.Contains(color))
      {
         if (!avalibaleColors.Contains(color))
         {
            avalibaleColors.Add(color);
         }
      }
   } 
}
