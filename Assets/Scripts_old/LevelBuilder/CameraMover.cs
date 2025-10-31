using UnityEngine;

namespace ChessRaid.LevelBuilder
{

    public class CameraMover : MonoBehaviour
    {
        [SerializeField] HexPlacer _placer;
        public float Speed = 2f;

        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.forward * Time.deltaTime * Speed;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.left * Time.deltaTime * Speed;
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.position += Vector3.back * Time.deltaTime * Speed;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * Time.deltaTime * Speed;
            }

            if (Input.GetMouseButtonDown(0))
            {
                var mouseRay = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                _placer.Place(mouseRay);
            }
        }
    }
}