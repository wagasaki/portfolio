using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public float AdsHeight { get; set; }
    //public float GetAdsHeight(float height)
    //{

    //}
    void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        //Debug.Log(rect.x + "/" + rect.y + "/" + rect.width + "/" + rect.height);
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)1080 / 2160); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        Debug.Log(rect.x + "/" + rect.y + "/" + rect.width + "/" + rect.height);
        float adsheightratio = 180 / 2280f;//세로로 매우 긴 경우. 0.059027777777777777777777, 즉 세로길이의 5.9% 보다 레터박스가 더 큰 경우. // 이거 근데 에디터에뜨는 광고로는 85/1440인데.. 실기기에선 180/2280
        float letterboxheight = rect.y;
        float heightdiff = adsheightratio - letterboxheight;

        AdsHeight = 0;
        if (heightdiff > 0)
        {
            AdsHeight = 2160 * heightdiff;
        }

        camera.rect = rect;
    }
    /// <summary>
    /// 현재 기기의 스크린이 정사각형, 즉 1:1비율인 경우 예시를 들면
    /// scaleheight 는 2가되고 scalewidth는 1/2이 된다. 
    /// camera rect의 height와 width의 초기값은 1, 1 인데, width가 1 * 1/2이 되야 한다는 것.
    /// rect.x는 xMin을 의미하는데(rect.y는 yMin) 전체 너비(1)에서 위의 width(1/2)일때 센터에 넣기 위해선, 전체에서 width부분을 제외한(1-width) 부분의 절반(1/4)이 xMin이 된다는것
    /// 따라서 0.25,0부터 화면을 그려서 0.75,1까지 화면이 그려지고 나머지 부분은 precull시 검은화면으로 출력하게 됨
    /// 
    /// 만약 3:2(너비:높이)인 화면에서 출력할 경우
    /// scaleheight = 3, scalewidth는 1/3이 되고. (1-1/3)의 절반인 1/3, 즉 0.333... , 0부터 그려서 0.666... ,1 좌표까지 화면이 그려진다.
    /// 
    /// 반대로 2:3인 화면에서는 (아직까지도 높이가 2배인 비율까지 도달하지 않아서 높이는 고정, 너비가 움직임. 위의 1:1, 3:2 다 마찬가지)
    /// scaleheight = 4/3, scalewidth = 3/4 너비가 3/4가 되고, xMin은 1/8인 지점, 즉 0.125,0부터 0.875,1 까지 그리게 됨
    /// 
    /// 2:4화면에서는 딱 fit하게 되고
    /// 
    /// 2:6화면에서는 (높이가 너비의 2배인 비율을 넘어섬. 따라서 너비가 고정되고 높이가 움직이게 됨
    /// scaleheight 2/3, rect.height = 2/3이 되고, width는 1 고정. height가 2/3이므로 yMin = (1-2/3)/2 인 1/6이 된다. 따라서 0, 0.166 부터 1, 0.833...까지 그린다.
    /// 
    /// </summary>

    void OnPreCull() => GL.Clear(true, true, Color.black);
}
