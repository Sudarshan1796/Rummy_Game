using UnityEngine;

namespace com.Rummy.Ui
{
    public class UiRotator : MonoBehaviour
    {
        public bool shouldRotate;
        public Transform rotateTransform;
        public float rotationAngle = 0.3f;

        internal void Enable(bool shouldEnable)
        {
            gameObject.SetActive(shouldEnable);
        }

        private void Update()
        {
            if (shouldRotate)
            {
                rotateTransform.Rotate(new Vector3(0, 0, rotationAngle));
            }
        }
    }
}
