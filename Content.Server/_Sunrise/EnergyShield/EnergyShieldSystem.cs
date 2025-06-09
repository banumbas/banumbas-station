using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.PowerCell;
using Content.Server._Sunrise.EnergyShield;
using Content.Shared.Damage;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Popups;
using Content.Shared.Examine;
using Content.Shared.Timing;
using Content.Shared.IdentityManagement;
using Content.Shared.PowerCell.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameStates;

namespace Content.Server._Sunrise.EnergyShield;

public sealed class EnergyShieldSystem : EntitySystem
{
    [Dependency] private readonly BatterySystem _battery = default!;
    [Dependency] private readonly ItemToggleSystem _itemToggle = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<EnergyShieldComponent, DamageChangedEvent>(OnDamage);
        SubscribeLocalEvent<EnergyShieldComponent, ItemToggleActivateAttemptEvent>(OnToggleAttempt);
    }

    private void OnDamage(EntityUid uid, EnergyShieldComponent component, DamageChangedEvent args)
    {
        if (!_itemToggle.IsActivated(uid)
            || args.DamageDelta == null
            || !TryComp<BatteryComponent>(uid, out var battery))
            return;

        var cost = args.DamageDelta.GetTotal().Float() * component.EnergyCostPerDamage;
        if (cost <= 0)
        return;

        _battery.UseCharge(uid, cost, battery);

        _audio.PlayPvs(component.AbsorbSound, uid);

        if (battery.CurrentCharge <= 0)
           _itemToggle.Toggle(uid);
           _audio.PlayPvs(component.ShutdownSound, uid);
    }

    private void OnToggleAttempt(EntityUid uid, EnergyShieldComponent component, ref ItemToggleActivateAttemptEvent args)
    {
        if (!TryComp<BatteryComponent>(uid, out var battery) || battery.CurrentCharge < battery.MaxCharge * 0.5f)
        {
            _popup.PopupEntity(
                Loc.GetString("stunbaton-component-low-charge"),
                args.User ?? uid,
                args.User ?? uid,
                PopupType.Small
            );
            args.Cancelled = true;
        }
    }
}
