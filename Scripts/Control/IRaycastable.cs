using UnityEngine;

namespace RPG.Control
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController playerController, bool mouseClicked, bool mouseHeld);
        float GetRadius();

        Transform GetTransform();
    }
}
