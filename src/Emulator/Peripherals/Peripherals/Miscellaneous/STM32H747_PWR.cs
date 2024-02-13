//
// Copyright (c) 2010-2023 Antmicro
// Copyright (c) 2022 SICK AG
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//

using System.Collections.Generic;
using Antmicro.Renode.Core;
using Antmicro.Renode.Core.Structure.Registers;
using Antmicro.Renode.Peripherals.Bus;
using Antmicro.Renode.Logging;
using Antmicro.Renode.Utilities;

namespace Antmicro.Renode.Peripherals.Miscellaneous
{
    [AllowedTranslations(AllowedTranslation.ByteToDoubleWord | AllowedTranslation.WordToDoubleWord)]
    public sealed class STM32H747_PWR : BasicDoubleWordPeripheral, IKnownSize
    {
        public STM32H747_PWR(IMachine machine) : base(machine)
        {
            DefineRegisters();
        }
        
        private void DefineRegisters()
        {
            Registers.Control1.Define(this, 0xF000C000, name: "PWR_CR1")
                .WithTaggedFlag("LPDS", 0)
                .WithReservedBits(1, 3)
                .WithTaggedFlag("PVDE", 4)
                .WithEnumField<DoubleWordRegister, PvdLevelSelection>(5, 3, name: "PLS")
                .WithTaggedFlag("DBP", 8)
                .WithTaggedFlag("FLPS", 9)
                .WithReservedBits(10, 4)
                .WithEnumField<DoubleWordRegister, SystemStopVoltageScalingSelection>(14, 2, out svosValue, name: "SVOS", writeCallback: (_, value) =>
                {
                    if(value == SystemStopVoltageScalingSelection.Reserved)
                    {
                        svosValue.Value = SystemStopVoltageScalingSelection.ScaleMode3;
                    }
                })
                .WithTaggedFlag("AVDEN", 16)
                .WithEnumField<DoubleWordRegister, AnalogVoltageDetectionLevelSelection>(17, 2, name: "ALS")
                .WithReservedBits(19, 13);
                
            Registers.Status.Define(this, 0x4000, name: "PWR_CSR1")
                .WithReservedBits(0, 4)
                .WithTaggedFlag("PVDO", 4)
                .WithReservedBits(5, 8)
                .WithTaggedFlag("SBF", 1)
                .WithTaggedFlag("PVDO", 2)
                .WithTaggedFlag("BRR", 3)
                .WithReservedBits(4, 4)
                .WithTaggedFlag("EWUP", 8)
                .WithTaggedFlag("BER", 9)
                .WithReservedBits(10, 4)
                .WithTaggedFlag("VOSRDY", 14)
                .WithReservedBits(15, 1)
                .WithFlag(16, out odrdyValue, FieldMode.Read, name: "ODRDY")
                .WithFlag(17, out odswrdyValue, FieldMode.Read, name: "ODSWRDY")
                .WithEnumField<DoubleWordRegister, UnderDriveReady>(18, 2, FieldMode.Read | FieldMode.WriteOneToClear, name: "UDRDY")
                .WithReservedBits(20, 12);
        }

        public long Size => 0x400;

        private IEnumRegisterField<SystemStopVoltageScalingSelection> svosValue;
        private IEnumRegisterField<RegulatorVoltageScalingOutputSelection> vosValue;
        private IFlagRegisterField odswenValue;
        private IFlagRegisterField odrdyValue;
        private IFlagRegisterField odswrdyValue;

        private enum Registers
        {
            Control1 = 0x00,
            Status = 0x04,
            Control2 = 0x08,
            Control3 = 0x0C,
            Cpu1Control = 0x10,
            Cpu2Control = 0x14,
            Domain3Control = 0x18,
            WakeupStatusClear = 0x20,
            WakeupStatus = 0x24,
            WakeupControl = 0x28

        }

        private enum PvdLevelSelection
        {
            V1_95,
            V2_1,
            V2_25,
            V2_4,
            V2_55,
            V2_7,
            V2_85,
            ExternalInput
        }

        private enum RegulatorVoltageScalingOutputSelection
        {
            Reserved,
            ScaleMode3,
            ScaleMode2,
            ScaleMode1
        }

        private enum SystemStopVoltageScalingSelection
        {
            Reserved,
            ScaleMode5,
            ScaleMode4,
            ScaleMode3
        }

        private enum AnalogVoltageDetectionLevelSelection
        {
            V1_7,
            V2_1,
            V2_5,
            V2_8
        }

        private enum UnderDriveEnableInStopMode
        {
            UnderDriveDisable,
            Reserved1,
            Reserved2,
            UnderDriveEnable
        }

        private enum UnderDriveReady
        {
            UnderDriveDisabled,
            Reserved1,
            Reserved2,
            UnderDriveActivated
        }
    }
}
