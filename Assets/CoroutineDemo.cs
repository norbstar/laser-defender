using System.Collections;

using UnityEngine;

public class CoroutineDemo : MonoBehaviour
{
    private Coroutine heavyLiftingCoroutine;
    private bool isComplete;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start StartCoroutine");
        heavyLiftingCoroutine = StartCoroutine(Co_HeavyLifting());
        Debug.Log("Start StartCoroutine Executing");

        //Debug.Log("Start Method");
        //HeavyLifting();
        //Debug.Log("Start Method Executed");
    }

    // Update is called once per frame
    void Update()
    {
        if (isComplete)
        {
            isComplete = false;
            Debug.Log("Update StartCoroutine/Method Complete");
        }
    }

    private IEnumerator Co_HeavyLifting()
    {
        isComplete = false;
        int count = 1000, loopCount = 100, itrCount = 0;

        Debug.Log($"HeavyLiftingCoroutine Started");

        while (!isComplete)
        {
            Debug.Log($"HeavyLiftingCoroutine Iteration Count: {itrCount + 1} of {count}");

            int result = 0;
            
            for (int itr = 0; itr < loopCount; ++itr)
            {
                result += itr * itrCount;
            }
    
            Debug.Log($"HeavyLiftingCoroutine Itr: {itrCount + 1} Result: {result}");

            isComplete = (itrCount == (count - 1));
            ++itrCount;

            Debug.Log($"HeavyLiftingCoroutine Is Complete: {isComplete}");

            if (!isComplete)
            {
                // Relinquish control back to the system to free up CPU resource for other processes and return to the next line on the next scheduled cycle
                // This has the effect of spreading the CPU load over time
                yield return null;
            }
        }

        Debug.Log($"HeavyLiftingCoroutine Complete");
    }

    //private void HeavyLifting()
    //{
    //    isComplete = false;
    //    int count = 10000, loopCount = 1000, itrCount = 0;

    //    Debug.Log($"HeavyLifting Started");

    //    while (!isComplete)
    //    {
    //        Debug.Log($"HeavyLifting Iteration Count: {itrCount + 1} of {count}");

    //        int result = 0;

    //        for (int itr = 0; itr < loopCount; ++itr)
    //        {
    //            result += itr * itrCount;
    //        }

    //        Debug.Log($"HeavyLifting Itr: {itrCount + 1} Result: {result}");

    //        isComplete = (itrCount == (count - 1));
    //        ++itrCount;

    //        Debug.Log($"HeavyLifting Is Complete: {isComplete}");
    //    }

    //    Debug.Log($"HeavyLifting Complete");
    //}

    //IEnumerator HeavyLiftingCoroutine()
    //{
    //    Debug.Log($"HeavyLiftingCoroutine Started");

    //    // Relinquish control back to the system to free up CPU resource for other processes and return to the next line on the next scheduled cycle
    //    // This has the effect of spreading the CPU load over time
    //    yield return null;

    //    // Specifically yield to the function WaitForSeconds to delay for 1 second,m which when complete will return to the line after the yield
    //    yield return new WaitForSeconds(1);

    //    Debug.Log($"HeavyLiftingCoroutine Complete");
    //}
}