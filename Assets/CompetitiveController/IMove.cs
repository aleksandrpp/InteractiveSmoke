using UnityEngine;

namespace AK.CompetitiveController
{
    public interface IMove
    {
        Vector2 MoveDelta { get; set; }
        Quaternion CameraRotation { get; set; }
        bool JumpPressed { get; set; }

        Vector3 Position { get; }
        Vector3 Velocity { get; }
        bool Grounded { get; }

        void Update();
    }
}