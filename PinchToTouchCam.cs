using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PinchToTouchCam : MonoBehaviour
{

    // Start is called before the first frame update

    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.

    public float orthoZoomSpeed = 0.5f;

    public Camera _camera;

    public float movespeed = 2f;

    Vector2 prevPos = Vector2.zero;



    public float minPosX = -10;

    public float maxPosX = 10;

    public float minPosZ = -10;

    public float maxPosZ = 10;


    //영역제한용

    public float xOffset;

    public float yOffset;

    public float ZoomInMin;

    public float ZoomInMax;


    public bool showGizmo = true;


    bool isLimited;

    float fv;


    void Start()

    {

        Screen.orientation = ScreenOrientation.Portrait;
        _camera = Camera.main;
        _camera.orthographicSize = ZoomInMax;

    }




    public void SpeedChangeZoomPer()

    {

        movespeed = (ZoomInMax - _camera.orthographicSize) * 1f + 1000f;

    }


    void Update()

    {

        SpeedChangeZoomPer();




        if (Input.touchCount == 1)

        {


            Calculate(fv);


            if (prevPos == Vector2.zero)

            {

                prevPos = Input.GetTouch(0).position;



                return;

            }

            Vector2 dir = (Input.GetTouch(0).position - prevPos).normalized;

            Vector3 vec = new Vector3(dir.x, dir.y, 0);


            Vector3 tmp = _camera.transform.position;

            tmp -= vec * movespeed * Time.deltaTime;


            Debug.Log("dir" + dir);



            switch (Input.GetTouch(0).phase)

            {

                case TouchPhase.Moved:

                    if (dir.x != 0)

                    {

                        if (maxPosX >= tmp.x && tmp.x >= minPosX)

                        {

                            _camera.transform.position = tmp;

                        }

                        //좌우 움직임

                    }

                    if (dir.y != 0)

                    {

                        if (maxPosZ >= tmp.y && tmp.y >= minPosZ)

                        {

                            _camera.transform.position = tmp;

                        }

                        //상하 움직임

                    }

                    break;

            }


            MoveLimit();


            prevPos = Input.GetTouch(0).position;

        }

        else if (Input.touchCount == 2)

        {

            // Store both touches.

            Touch touchZero = Input.GetTouch(0);

            Touch touchOne = Input.GetTouch(1);


            // Find the position in the previous frame of each touch.

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;

            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;


            // Find the magnitude of the vector (the distance) between the touches in each frame.

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;

            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;


            // Find the difference in the distances between each frame.

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;





            // If the camera is orthographic...

            if (_camera.orthographic)

            {

                float temp = _camera.orthographicSize;

                temp += deltaMagnitudeDiff * orthoZoomSpeed;

                //camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;


                //fv = Mathf.Max(camera.orthographicSize, 0.1f);

                fv = Mathf.Max(temp, 0.1f);

                Calculate(fv);

                MoveLimit();


                if (fv > ZoomInMax)

                {

                    _camera.orthographicSize = ZoomInMax;

                }

                else if (ZoomInMin > fv)

                {

                    _camera.orthographicSize = ZoomInMin;

                }

                else

                {

                    _camera.orthographicSize = fv;

                }



            }

            else

            {

                // Otherwise change the field of view based on the change in distance between the touches.

                _camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.

                _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, 0.1f, 179.9f);

                fv = _camera.fieldOfView;

            }

        }


    }


    void MoveLimit()

    {

        Vector2 temp;

        temp.x = Mathf.Clamp(_camera.transform.position.x, minPosX, maxPosX);

        temp.y = Mathf.Clamp(_camera.transform.position.y, minPosZ, maxPosZ);

        _camera.transform.position = temp;

    }






    void OnDrawGizmos()

    {

        if (showGizmo)

        {



            Calculate(fv);


            Vector3 p1 = new Vector3(minPosX, maxPosZ, transform.position.z);

            Vector3 p2 = new Vector3(maxPosX, maxPosZ, transform.position.z);

            Vector3 p3 = new Vector3(maxPosX, minPosZ, transform.position.z);

            Vector3 p4 = new Vector3(minPosX, minPosZ, transform.position.z);





            //영역 테스트용

            Gizmos.color = Color.green;

            Gizmos.DrawLine(p1, p2);

            Gizmos.DrawLine(p2, p3);

            Gizmos.DrawLine(p3, p4);

            Gizmos.DrawLine(p4, p1);


        }

    }


    void Calculate(float size)

    {

        minPosX = -(ZoomInMax - _camera.orthographicSize) / xOffset;

        maxPosX = (ZoomInMax - _camera.orthographicSize) / xOffset;


        minPosZ = -(ZoomInMax - _camera.orthographicSize) / yOffset;

        maxPosZ = (ZoomInMax - _camera.orthographicSize) / yOffset;

    }

}
