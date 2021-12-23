using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuDummyHexagon : MonoBehaviour
{
    Vector3 mover;
    Vector3 rotator;
    MainMenuBG mainMenuBG;
    public void Init(float lifeTime, MainMenuBG mainMenuBG)
    {
        transform.localPosition = Vector3.zero;
        this.mainMenuBG = mainMenuBG;
        GetComponent<Image>().color = new Color(Random.value, Random.value, Random.value);

        mover = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        rotator = new Vector3(0, 0, Random.Range(-10, 10));

        StartCoroutine(RotateAndDie(lifeTime));
    }

    private IEnumerator RotateAndDie(float lifeTime)
    {
        float elapsedTime = 0f;

        do
        {
            transform.position += mover;
            transform.eulerAngles += rotator;
            yield return new WaitForSeconds(0.025f);
            elapsedTime += 0.025f;
        } while (elapsedTime < lifeTime);

        gameObject.SetActive(false);
        mainMenuBG.OnDummyHexagonDestroyed(this);
    }
}
