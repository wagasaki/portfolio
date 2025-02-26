using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOut : MonoBehaviour //이거 스크롤렉트 상에서 제대로 동작 안함. 쓰면 안됨 그냥 참고용
{
    private RectTransform _rect;
    float maxsize = 1, minsize = 0.6f;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();

    }
    //private void OnEnable()
    //{
    //    Input.multiTouchEnabled = true;
    //}
    //private void OnDisable()
    //{
    //    Input.multiTouchEnabled = false;
    //}

    private void Update()
    {
        if (Input.touchCount == 2 && Input.touches[0].phase == TouchPhase.Moved && Input.touches[1].phase == TouchPhase.Moved)
        {
            Touch touchZero = Input.GetTouch(0); //첫번째 손가락 터치를 저장
            Touch touchOne = Input.GetTouch(1); //두번째 손가락 터치를 저장

            //터치에 대한 이전 위치값을 각각 저장함
            //처음 터치한 위치(touchZero.position)에서 이전 프레임에서의 터치 위치와 이번 프로임에서 터치 위치의 차이를 뺌
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; //deltaPosition는 이동방향 추적할 때 사용
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 각 프레임에서 터치 사이의 벡터 거리 구함
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitude는 두 점간의 거리 비교(벡터)
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 거리 차이 구함(거리가 이전보다 크면(마이너스가 나오면)손가락을 벌린 상태_줌인 상태)
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;





            Vector3 localscale = _rect.localScale;
            localscale.x = Mathf.Clamp(localscale.x + deltaMagnitudeDiff * 0.1f, minsize, maxsize);
            localscale.y = localscale.x;
            localscale.z = localscale.x;
            _rect.localScale = localscale;
        }
        //float scroll = Input.GetAxis("Mouse ScrollWheel") * 1;
        //Vector3 localscale = _rect.localScale;
        //localscale.x = Mathf.Clamp(localscale.x + scroll, minsize, maxsize);
        //localscale.y = localscale.x;
        //localscale.z = localscale.x;
        //_rect.localScale= localscale;
        //Debug.Log(localscale);
    }
}
