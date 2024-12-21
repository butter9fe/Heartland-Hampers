using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    private IEnumerator loadingCoroutine;

    private void OnEnable()
    {
        loadingCoroutine = LoadingAnimation(0.5f);
        StartCoroutine(loadingCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(loadingCoroutine);
        transform.rotation = Quaternion.identity;
    }

    private IEnumerator LoadingAnimation(float speed)
    {
        while (true)
        {
            var fromRot = transform.eulerAngles.z;
            var toRot = fromRot - 360.0f;
            float timeElapsed = 0;

            while (timeElapsed < speed)
            {
                float zRot = Mathf.Lerp(fromRot, toRot, timeElapsed / speed) % 360.0f;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRot);

                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
