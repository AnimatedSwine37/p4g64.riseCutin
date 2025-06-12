using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using static p4g64.riseCutin.Utils;

namespace p4g64.riseCutin.Native;

public unsafe class Battle
{
    internal RunActionDelegate StartAction { get; private set; }
    internal NewCutinActionDelegate NewCutinAction { get; private set; }
    internal NewCutinDrawActionDelegate NewCutinDrawAction { get; private set; }

    internal Battle(IReloadedHooks hooks)
    {
        SigScan("48 89 5C 24 ?? 57 48 83 EC 20 48 8B D9 8B FA 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? BA 14 00 00 00", "Btl::UI::NewCutinAction",
            address =>
            {
                NewCutinAction = hooks.CreateWrapper<NewCutinActionDelegate>(address, out _);
            });
        
        SigScan("48 89 5C 24 ?? 57 48 83 EC 20 8B D9 8B FA 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? BA 0C 00 00 00", "Btl::UI::NewCutinDrawAction",
            address =>
            {
                NewCutinDrawAction = hooks.CreateWrapper<NewCutinDrawActionDelegate>(address, out _);
            });
        
        SigScan("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B D9 0F B6 F2", "Btl::Action::RunAction", address =>
        {
            StartAction = hooks.CreateWrapper<RunActionDelegate>(address, out _);
        });
    }

    internal delegate BtlAction* NewCutinActionDelegate(BtlCutinArgs* cutinArgs, uint param2);

    internal delegate BtlAction* NewCutinDrawActionDelegate(int param1, int param2);

    internal delegate nuint RunActionDelegate(BtlAction* action, byte param2);

    [StructLayout(LayoutKind.Explicit)]
    internal struct BtlAction
    {
        [FieldOffset(0)]
        internal BtlActionDependencies Dependencies;

        [FieldOffset(8)] internal nuint Unk2;

        // The unique ID for this action
        [FieldOffset(0x98)] internal long Id;

        [FieldOffset(0xa0)] internal nuint Unk4;
    }

    [InlineArray(4)]
    internal struct BtlActionDependencies
    {
        private BtlActionDependenecy _dependenecy0;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    internal struct BtlActionDependenecy
    {
        // The type of dependency or something, not sure
        [FieldOffset(0)] internal byte Type;

        // The id of the action the dependency is for
        [FieldOffset(8)] internal long ActionId;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0xa)]
    internal struct BtlCutinArgs
    {
        [FieldOffset(0)]
        internal BtlCutinType CutinType;

        [FieldOffset(2)]
        internal PartyMember PartyMember;
    }

    internal enum BtlCutinType: short
    {
        Assist = 2,
        WeaknessHit = 4,
    }

    internal enum PartyMember : short
    {
        None = 0,
        Protag = 1,
        Yosuke = 2,
        Chie = 3,
        Yukiko = 4,
        Rise = 5,
        Kanji = 6,
        Naoto = 7,
        Teddie = 8
    }
}