using p4g64.riseCutin.Native;
using Reloaded.Hooks.Definitions;
using static p4g64.riseCutin.Utils;
using static p4g64.riseCutin.Native.Battle;

namespace p4g64.riseCutin;

public unsafe class AssistHook
{
    private IHook<RunRiseAssistDelegate> _runAssistHook;
    private Battle _battle;
    
    internal AssistHook(Battle battle, IReloadedHooks hooks)
    {
        _battle = battle;
        
        SigScan("48 8B C4 48 89 58 ?? 48 89 70 ?? 48 89 78 ?? 55 41 54 41 55 41 56 41 57 48 8D A8 ?? ?? ?? ?? 48 81 EC 60 03 00 00 0F 29 70 ?? 0F 29 78 ?? 48 8B 05 ?? ?? ?? ??", "RunRiseAssist",
            address =>
            {
                _runAssistHook = hooks.CreateHook<RunRiseAssistDelegate>(RunAssist, address).Activate();
            });
    }

    private void RunAssist(nuint param1)
    {
        Log("Running assist!");
        _runAssistHook.OriginalFunction(param1);

        BtlCutinArgs cutinArgs = new()
        {
            CutinType = BtlCutinType.WeaknessHit,
            PartyMember = PartyMember.Rise
        };
        var cutinAction = _battle.NewCutinAction(&cutinArgs, 0);
        _battle.StartAction(cutinAction, 1);

        var drawAction = _battle.NewCutinDrawAction(0, 0);
        drawAction->Dependencies[0].Type = 4;
        drawAction->Dependencies[0].ActionId = cutinAction->Id;
        _battle.StartAction(drawAction, 1);

        // Copied from use in UseSkill
        var soundEffectAction = _battle.NewAction(0x902,6);
        soundEffectAction->RunFunc = _battle.PlaySoundEffectActionFunc;
        var soundEffectData = (PlaySoundEffectActionData*)soundEffectAction->DataPtr;
        soundEffectData->SoundId = 0xb;
        soundEffectData->Unk1 = 4;
        soundEffectData->Unk2 = 2;
        soundEffectAction->Dependencies[0].Type = 5;
        soundEffectAction->Dependencies[0].ActionId = drawAction->Id;
        _battle.StartAction(soundEffectAction, 1);
    }

    private delegate void RunRiseAssistDelegate(nuint param1);
}