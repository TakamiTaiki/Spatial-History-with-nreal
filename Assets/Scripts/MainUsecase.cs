using System.Collections;
using UnityEngine;
using static Utils.Utility;

public class MainUsecase : MonoBehaviour
{
    private Vector3 shrinkScale = Vector3.one * 0.23f;
    private Vector3 enlargeScale = Vector3.one * 1.5f;
    private Quaternion eraTextRotation = Quaternion.Euler(0, 0, 0);

    public IEnumerator PlaneLiftUp(MainManager.Node node)
    {
        Vector3 dir = Vector3.up * Time.deltaTime * 0.15f;
        while (node.panel.position.y < node.destination.position.y)
        {
            node.panel.Translate(dir);
            yield return null;
        }
    }

    public IEnumerator PlaneLiftDown(MainManager.Node node)
    {
        Vector3 dir = Vector3.down * Time.deltaTime * 0.15f;
        while (node.panel.position.y > node.destination.position.y)
        {
            node.panel.Translate(dir);
            yield return null;
        }
    }

    public IEnumerator ActivateNode(MainManager.Node node, FloatingMenuView view)
    {
        //縮小化
        yield return Shrink(view);
        //移動
        float t = 0;
        while (IsReaching(ref t, 0.001f, 0.1f))
        {
            view.plane_UI.position = Vector3.Lerp(view.plane_UI.position, node.transform.position, t);
            yield return null;
        }
        //拡大化
        yield return EnLarge(node, view);
    }

    private IEnumerator Shrink(FloatingMenuView view)
    {
        float t = 0;
        while (IsReaching(ref t, 0.02f, 1))
        {
            view.nodeIndicater.transform.localScale = Vector3.Lerp(view.nodeIndicater.transform.localScale, shrinkScale, t);
            yield return null;
        }
    }

    private IEnumerator EnLarge(MainManager.Node node, FloatingMenuView view)
    {
        float t = 0;
        while (IsReaching(ref t, 0.02f, 1))
        {
            view.nodeIndicater.transform.localScale = Vector3.Lerp(view.nodeIndicater.transform.localScale, enlargeScale, t);
            yield return null;
        }

        t = 0;
        Activate(node.eraText.gameObject);
        while (IsReaching(ref t, 0.02f, 1))
        {
            node.eraText.localRotation = Quaternion.Lerp(node.eraText.localRotation, eraTextRotation, t);
            yield return null;
        }
    }

    public IEnumerator On3DButtonDown(GameObject button)
    {
        ChangeObjColor(button, Color.green);
        yield return new WaitForSeconds(0.5f);
        ChangeObjColor(button, Color.white);
        DeActivate(button);
    }

    public IEnumerator ReleaseArrow(GameObject arrow1, GameObject arrow2)
    {
        Vector3 arrow1Pos = arrow1.transform.position;
        Vector3 arrow2Pos = arrow2.transform.position;
        yield return new WaitForSeconds(2f);
        Activate(arrow1);
        Activate(arrow2);
        for (int i = 0; i < 20; i++)
        {
            arrow1.transform.Translate(0, 0.001f, 0.001f);
            arrow2.transform.Translate(0, 0.001f, 0.001f);
            yield return null;
        }
        DeActivate(arrow1);
        DeActivate(arrow2);
        arrow1.transform.position = arrow1Pos;
        arrow2.transform.position = arrow2Pos;
    }

    private bool IsReaching(ref float t, float inc, float max) { return Mathf.Clamp01(t += inc) < max; }
}
