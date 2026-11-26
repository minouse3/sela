using UnityEngine;

internal sealed class DitaConversationShot
{
    static readonly Collider[] overlaps = new Collider[16];
    static readonly RaycastHit[] sightlineHits = new RaycastHit[16];

    readonly Transform dita;
    readonly FirstPersonController player;
    readonly Camera camera;
    readonly Quaternion ditaRotation;
    readonly Vector3 playerPosition;
    readonly Quaternion playerRotation;
    readonly Vector3 cameraPosition;
    readonly Quaternion cameraRotation;
    readonly float fieldOfView;
    bool restored;

    DitaConversationShot(Transform dita, FirstPersonController player, Camera camera)
    {
        this.dita = dita;
        this.player = player;
        this.camera = camera;
        ditaRotation = dita.localRotation;
        playerPosition = player.transform.localPosition;
        playerRotation = player.transform.localRotation;
        cameraPosition = camera.transform.localPosition;
        cameraRotation = camera.transform.localRotation;
        fieldOfView = camera.fieldOfView;
    }

    internal static bool TryBegin(Transform dita, FirstPersonController player, Camera camera,
                                  out DitaConversationShot shot)
    {
        shot = null;
        if (!dita || !player || player.IsSeated || player.ConversationViewActive ||
            !camera || !camera.isActiveAndEnabled || !camera.transform.IsChildOf(player.transform) ||
            !player.TryGetComponent<CharacterController>(out var capsule)) return false;

        // Both Kamar 06 layouts have the same floor and clear aisle in front of Dita.
        var origin = dita.position;
        var heading = Quaternion.Euler(0f, dita.eulerAngles.y, 0f);
        var rakaMark = origin + heading * new Vector3(0f, -0.07f, 1.14f);
        var physics = dita.gameObject.scene.GetPhysicsScene();
        var playerRoot = player.transform;
        Physics.SyncTransforms();

        bool ClearSightline(Vector3 from, Vector3 to)
        {
            var delta = to - from;
            float distance = delta.magnitude;
            int count = physics.Raycast(from, delta / distance, sightlineHits, distance, ~0, QueryTriggerInteraction.Ignore);
            if (count == sightlineHits.Length) return false;
            for (int i = 0; i < count; i++)
            {
                var hit = sightlineHits[i].collider.transform;
                if (!hit.IsChildOf(dita) && !hit.IsChildOf(playerRoot)) return false;
            }
            return true;
        }

        bool ClearCameraMark(Vector3 mark) =>
            ClearOfWorld(physics.OverlapSphere(mark, 0.16f, overlaps, ~0, QueryTriggerInteraction.Ignore), playerRoot) &&
            ClearSightline(mark, origin + Vector3.up * 1.02f) &&
            ClearSightline(mark, origin + Vector3.up * 1.50f) &&
            ClearSightline(mark, rakaMark + Vector3.up * 1.02f) &&
            ClearSightline(mark, rakaMark + Vector3.up * 1.50f);

        float side = 1f;
        float shotFov = 44f;
        var cameraMark = origin + heading * new Vector3(1.78f, 1.42f, 0.56f);
        if (!ClearCameraMark(cameraMark))
        {
            side = -1f;
            shotFov = 48f;
            cameraMark = origin + heading * new Vector3(-1.62f, 1.42f, 0.56f);
            if (!ClearCameraMark(cameraMark)) return false;
        }

        // Open both shoulders slightly toward the camera without breaking the eyeline.
        var ditaYaw = heading * Quaternion.Euler(0f, side * 10f, 0f);
        var rakaYaw = heading * Quaternion.Euler(0f, 180f - side * 10f, 0f);
        var scale = playerRoot.lossyScale;
        var capsuleCenter = rakaMark + rakaYaw * Vector3.Scale(capsule.center, scale);
        float radius = capsule.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
        float extent = Mathf.Max(0f, capsule.height * Mathf.Abs(scale.y) * 0.5f - radius);
        var offset = Vector3.up * extent;
        if (!ClearOfWorld(physics.OverlapCapsule(capsuleCenter - offset, capsuleCenter + offset, radius,
                overlaps, ~0, QueryTriggerInteraction.Ignore), playerRoot)) return false;

        var aim = origin + heading * new Vector3(0f, 1.12f, 0.57f);
        shot = new DitaConversationShot(dita, player, camera);
        player.SetConversationView(true);
        dita.rotation = ditaYaw;
        playerRoot.SetPositionAndRotation(rakaMark, rakaYaw);
        camera.transform.SetPositionAndRotation(cameraMark, Quaternion.LookRotation(aim - cameraMark));
        camera.fieldOfView = shotFov;
        return true;
    }

    static bool ClearOfWorld(int count, Transform playerRoot)
    {
        // The real player can already be standing on either mark when E is pressed.
        bool clear = count < overlaps.Length;
        for (int i = 0; i < count; i++)
        {
            clear &= overlaps[i].transform.IsChildOf(playerRoot);
            overlaps[i] = null;
        }
        return clear;
    }

    internal void Restore()
    {
        if (restored) return;
        restored = true;
        if (dita) dita.localRotation = ditaRotation;
        if (player) player.transform.SetLocalPositionAndRotation(playerPosition, playerRotation);
        if (camera)
        {
            camera.transform.SetLocalPositionAndRotation(cameraPosition, cameraRotation);
            camera.fieldOfView = fieldOfView;
        }
        if (player) player.SetConversationView(false);
    }
}
