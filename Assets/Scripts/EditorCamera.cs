using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Editor上でCameraをキーボード操作するためのスクリプト
/// </summary>
public class EditorCamera : MonoBehaviour
{
    const float Angle = 50f;
    public float baseSpeed = 4f;  // 基本の移動速度
    public float minSpeed = 1f;   // カメラが下を向いたときの最小速度
    public float maxDownAngle = 90f;  // 速度が最も遅くなるカメラの下向き角度（真下）
    public float minDownAngle = 0f;   // 速度が最大になるカメラの角度（水平）
    private CharacterController characon;
    private new Camera camera;

    private void Start()
    {
        if (this.name == "CenterEyeAnchor") Destroy(this);
        characon = this.GetComponent<CharacterController>();
        camera = GameObject.Find("CenterEyeAnchor").GetComponent<Camera>();
    }

    void Update()
    {
        // カメラのピッチ角度（上下の回転）を取得
        float cameraPitch = camera.transform.eulerAngles.x;

        // カメラの角度が180度を超える場合、360度から引いて下向きの角度に補正
        if (cameraPitch > 180)
        {
            cameraPitch = 360 - cameraPitch;
        }

        // カメラが0度（水平）から90度（真下）の範囲で速度を調整
        float normalizedSpeed = Mathf.InverseLerp(maxDownAngle, minDownAngle, cameraPitch);
        float currentSpeed = Mathf.Lerp(minSpeed, baseSpeed, normalizedSpeed);

        // 左Shiftキーでスピードを一時的に倍増
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= 2;
        }

        // スティック入力を取得
        Vector2 tmp = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

        // スティック入力がある場合のみ移動
        if (tmp.magnitude > 0.1f)
        {
            // カメラのY軸回転を取得し、X軸とZ軸は無視する（水平移動のみ）
            Quaternion q = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0);

            // XZ平面での移動ベクトルを計算
            var vec = new Vector3(tmp.x, 0, tmp.y);
            vec = q * vec * currentSpeed * Time.deltaTime;  // カメラのY軸回転を適用し、移動量を調整

            // 重力を適用（Y方向のベクトルを管理）
            if (!characon.isGrounded)
            {
                vec.y -= 15f * Time.deltaTime;
            }

            // キャラクターコントローラで移動
            characon.Move(vec);
        }

        // キーボードでの移動処理
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward * currentSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += -transform.right * currentSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += -transform.forward * currentSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right * currentSpeed * Time.deltaTime;
        }

        // キャラクターコントローラで移動（キーボード入力）
        characon.Move(moveDirection);

        // 矢印キーでの回転操作
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, Angle * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -Angle * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            camera.transform.Rotate(-Angle * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            camera.transform.Rotate(Angle * Time.deltaTime, 0, 0);
        }

        // ジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (characon.isGrounded)
            {
                characon.Move(Vector3.up * 5);
            }
        }
    }
}
