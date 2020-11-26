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

        public static void Activate(GameObject obj) { if (!obj.activeSelf) obj.SetActive(true); }
        public static void DeActivate(GameObject obj) { if (obj.activeSelf) obj.SetActive(false); }

        public static void SetStage(int era, List<GameObject> castles, List<GameObject> planes)
        {
            // castles => [0]1931_First, [1]1585, [2]1585_Ash, [3]1626, [4]1626_Broken, [5]1931_End
            // planes  => [0]Transform, [1]Fire, [2]Restore_1626, [3]Restore_1931, [4]Top, [5]Bottom
            GameObject plane_Top = planes[planes.Count-2], plane_Bottom = planes[planes.Count-1];

            if (era == 1585)
            {
                Activate(castles[0]);
                Activate(castles[1]);
                DeActivate(castles[2]);
                DeActivate(castles[3]);
                DeActivate(castles[4]);
                DeActivate(castles[5]);
                Alignment(planes[0], plane_Top);
            }
            else if (era == 1615)
            {
                DeActivate(castles[0]);
                DeActivate(castles[1]);
                Activate(castles[2]);
                DeActivate(castles[3]);
                DeActivate(castles[4]);
                DeActivate(castles[5]);
                Alignment(planes[1], plane_Top);
            }
            else if (era == 1626)
            {
                DeActivate(castles[0]);
                DeActivate(castles[1]);
                DeActivate(castles[2]);
                Activate(castles[3]);
                DeActivate(castles[4]);
                DeActivate(castles[5]);
                Alignment(planes[2], plane_Bottom);
            }
            else if (era == 1665)
            {
                DeActivate(castles[0]);
                DeActivate(castles[1]);
                DeActivate(castles[2]);
                DeActivate(castles[3]);
                Activate(castles[4]);
                DeActivate(castles[5]);
                Alignment(planes[2], plane_Top);
            }
            else if (era == 1931)
            {
                DeActivate(castles[0]);
                DeActivate(castles[1]);
                DeActivate(castles[2]);
                DeActivate(castles[3]);
                DeActivate(castles[4]);
                Activate(castles[5]);
                Alignment(planes[3], plane_Bottom);
            }
        }
    }

}

