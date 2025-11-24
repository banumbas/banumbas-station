using Content.Shared._Sunrise.Flip;
using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._Sunrise.Flip;

public sealed partial class FlipSystem : SharedFlipSystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    private TimeSpan _lastFlipTime;
    private static TimeSpan _flipCooldown = TimeSpan.FromSeconds(0.5);

    private static readonly ProtoId<EmotePrototype> EmoteFlipProto = "Flip";

    public override void Initialize()
    {
        base.Initialize();

        CommandBinds.Builder
            .Bind(ContentKeyFunctions.Flip, InputCmdHandler.FromDelegate(Flip, handle: false, outsidePrediction: false))
            .Register<FlipSystem>();
    }

    private void Flip(ICommonSession? session)
    {
        var player = session?.AttachedEntity;

        if (!Exists(player))
            return;

        var currentTime = _gameTiming.CurTime;
        if (currentTime - _lastFlipTime < _flipCooldown)
            return;

        _lastFlipTime = currentTime;
        RaisePredictiveEvent(new PlayEmoteMessage(EmoteFlipProto));
    }
}

