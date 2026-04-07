using UnityEngine;

public class AppSurveillance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Surveillance()
    {
        //집중 모드일때만 적용
        if(GameManager.Instance.PlayMode == PlayMode.Concentration)
        {

        }
    }
}
