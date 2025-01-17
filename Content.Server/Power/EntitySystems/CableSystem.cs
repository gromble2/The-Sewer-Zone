using Content.Server.Electrocution;
using Content.Server.Power.Components;
using Content.Server.Tools;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Power.EntitySystems;

public class CableSystem : EntitySystem
{
    [Dependency] private readonly ToolSystem _toolSystem = default!;
    [Dependency] private readonly ElectrocutionSystem _electrocutionSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CableComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<CableComponent, CuttingFinishedEvent>(OnCableCut);
        SubscribeLocalEvent<CableComponent, AnchorStateChangedEvent>(OnAnchorChanged);
    }

    private void OnInteractUsing(EntityUid uid, CableComponent cable, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        var ev = new CuttingFinishedEvent(args.User);
        _toolSystem.UseTool(args.Used, args.User, uid, 0, cable.CuttingDelay, new[] { cable.CuttingQuality }, doAfterCompleteEvent: ev, doAfterEventTarget: uid);
        args.Handled = true;
    }

    private void OnCableCut(EntityUid uid, CableComponent cable, CuttingFinishedEvent args)
    {
        if (_electrocutionSystem.TryDoElectrifiedAct(uid, args.User))
            return;

        Spawn(cable.CableDroppedOnCutPrototype, Transform(uid).Coordinates);
        QueueDel(uid);
    }

    private void OnAnchorChanged(EntityUid uid, CableComponent cable, ref AnchorStateChangedEvent args)
    {
        if (args.Anchored)
            return; // huh? it wasn't anchored?

        // anchor state can change as a result of deletion (detach to null).
        // We don't want to spawn an entity when deleted.
        if (!TryLifeStage(uid, out var life) || life >= EntityLifeStage.Terminating)
            return;

        // This entity should not be un-anchorable. But this can happen if the grid-tile is deleted (RCD, explosion,
        // etc). In that case: behave as if the cable had been cut.
        Spawn(cable.CableDroppedOnCutPrototype, Transform(uid).Coordinates);
        QueueDel(uid);
    }
}

public class CuttingFinishedEvent : EntityEventArgs
{
    public EntityUid User;

    public CuttingFinishedEvent(EntityUid user)
    {
        User = user;
    }
}
