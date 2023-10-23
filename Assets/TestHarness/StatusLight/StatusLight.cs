using UnityEngine;
using System.Collections;

public class StatusLight : MonoBehaviour
{

    public GameObject InactiveLight;
    public GameObject StrikeLight;
    public GameObject PassLight;

    public void SetPass()
    {
        StopAllCoroutines();
        InactiveLight.SetActive(false);
        PassLight.SetActive(true);
        StrikeLight.SetActive(false);
    }

    public void SetInActive()
    {
        StopAllCoroutines();
        InactiveLight.SetActive(true);
        PassLight.SetActive(false);
        StrikeLight.SetActive(false);
    }
    public void SetStrike()
    {
        StopAllCoroutines();
        InactiveLight.SetActive(false);
        PassLight.SetActive(false);
        StrikeLight.SetActive(true);
    }

    public void FlashStrike(float time = 1f)
    {
        if (PassLight.activeSelf) return;
        StopAllCoroutines();
        InactiveLight.SetActive(true);
        StrikeLight.SetActive(false);
        if (gameObject.activeInHierarchy)
            StartCoroutine(StrikeFlash(time));
    }
    public void FlashPass(float time = 1f)
    {
        if (PassLight.activeSelf) return;
        StopAllCoroutines();
        InactiveLight.SetActive(true);
        StrikeLight.SetActive(false);
        if (gameObject.activeInHierarchy)
            StartCoroutine(PassFlash(time));
    }

    protected IEnumerator StrikeFlash(float blinkTime)
    {
        StrikeLight.SetActive(true);
        InactiveLight.SetActive(false);
        yield return new WaitForSeconds(blinkTime);
        StrikeLight.SetActive(false);
        InactiveLight.SetActive(true);
    }
    protected IEnumerator PassFlash(float blinkTime)
    {
        PassLight.SetActive(true);
        InactiveLight.SetActive(false);
        yield return new WaitForSeconds(blinkTime);
        PassLight.SetActive(false);
        InactiveLight.SetActive(true);
    }
}
