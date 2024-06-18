using UnityEngine;

namespace IG.Module.UI{
    public class SpriteFullScreen : MonoBehaviour{
        public Camera SpriteCamera;

        void Awake(){
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            float          cameraHeight   = SpriteCamera.orthographicSize * 2;
            Vector2        cameraSize     = new Vector2(SpriteCamera.aspect * cameraHeight, cameraHeight);
            Vector2        spriteSize     = spriteRenderer.sprite.bounds.size;
            transform.position = Vector3.zero; // Optional
            Vector3 localScale = new Vector3(cameraSize.x / spriteSize.x, cameraSize.y / spriteSize.y, 1);
            transform.localScale = localScale;
        }
    }
}