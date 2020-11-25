using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Utility
    {
        public static void ChangeObjColor   (GameObject obj, Color color)       { ChangeObjColor(obj.GetComponent<Renderer>(), color); }
        public static void ChangeObjColor   (Renderer rend, Color color)        { rend.material.color = color; }
        public static void Alignment        (GameObject from, GameObject to)    { Alignment(from.transform, to.transform); }
        public static void Alignment        (Transform from, Transform to)      { from.position = to.position; }
        public static bool Distance2D_GT    (float x, float y, float r)         { return x * x + y * y > r * r; }

        public static void SetStage         (int era, GameObject[] castles, GameObject[] planes)
        {
            // castles => [0]1931_First, [1]1585, [2]1585_Ash, [3]1626, [4]1626_Broken, [5]1931_End
            // planes  => [0]Transform, [1]Fire, [2]Restore_1626, [3]Restore_1931, [4]Top, [5]Bottom
            GameObject plane_Top = planes[planes.Length-2], plane_Bottom = planes[planes.Length-1];

            if (era == 1585)
            {
                castles[0].SetActive(true);
                castles[1].SetActive(true);
                castles[2].SetActive(false);
                castles[3].SetActive(false);
                castles[4].SetActive(false);
                castles[5].SetActive(false);
                Alignment(planes[0], plane_Top);
            }
            else if (era == 1615)
            {
                castles[0].SetActive(false);
                castles[1].SetActive(false);
                castles[2].SetActive(true);
                castles[3].SetActive(false);
                castles[4].SetActive(false);
                castles[5].SetActive(false);
                Alignment(planes[1], plane_Top);
            }
            else if (era == 1626)
            {
                castles[0].SetActive(false);
                castles[1].SetActive(false);
                castles[2].SetActive(false);
                castles[3].SetActive(true);
                castles[4].SetActive(false);
                castles[5].SetActive(false);
                Alignment(planes[2], plane_Bottom);
            }
            else if (era == 1665)
            {
                castles[0].SetActive(false);
                castles[1].SetActive(false);
                castles[2].SetActive(false);
                castles[3].SetActive(false);
                castles[4].SetActive(true);
                castles[5].SetActive(false);
                Alignment(planes[2], plane_Top);
            }
            else if (era == 1931)
            {
                castles[0].SetActive(false);
                castles[1].SetActive(false);
                castles[2].SetActive(false);
                castles[3].SetActive(false);
                castles[4].SetActive(false);
                castles[5].SetActive(true);
                Alignment(planes[3], plane_Bottom);
            }
        }
    }

}

