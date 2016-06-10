///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CorradeConfiguration;
using OpenMetaverse;
using wasOpenMetaverse;
using wasSharp;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class CorradeCommands
        {
            public static Action<CorradeCommandParameters, Dictionary<string, string>> setprimitivematerial =
                (corradeCommandParameters, result) =>
                {
                    if (
                        !HasCorradePermission(corradeCommandParameters.Group.UUID,
                            (int) Configuration.Permissions.Interact))
                    {
                        throw new ScriptException(ScriptError.NO_CORRADE_PERMISSIONS);
                    }
                    float range;
                    if (
                        !float.TryParse(
                            wasInput(KeyValue.Get(
                                wasOutput(Reflection.GetNameFromEnumValue(ScriptKeys.RANGE)),
                                corradeCommandParameters.Message)),
                            out range))
                    {
                        range = corradeConfiguration.Range;
                    }
                    Primitive primitive = null;
                    var item = wasInput(KeyValue.Get(
                        wasOutput(Reflection.GetNameFromEnumValue(ScriptKeys.ITEM)),
                        corradeCommandParameters.Message));
                    if (string.IsNullOrEmpty(item))
                    {
                        throw new ScriptException(ScriptError.NO_ITEM_SPECIFIED);
                    }
                    UUID itemUUID;
                    switch (UUID.TryParse(item, out itemUUID))
                    {
                        case true:
                            if (
                                !Services.FindPrimitive(Client,
                                    itemUUID,
                                    range,
                                    corradeConfiguration.Range,
                                    ref primitive, corradeConfiguration.ServicesTimeout,
                                    corradeConfiguration.DataTimeout,
                                    new Time.DecayingAlarm(corradeConfiguration.DataDecayType)))
                            {
                                throw new ScriptException(ScriptError.PRIMITIVE_NOT_FOUND);
                            }
                            break;
                        default:
                            if (
                                !Services.FindPrimitive(Client,
                                    item,
                                    range,
                                    corradeConfiguration.Range,
                                    ref primitive, corradeConfiguration.ServicesTimeout,
                                    corradeConfiguration.DataTimeout,
                                    new Time.DecayingAlarm(corradeConfiguration.DataDecayType)))
                            {
                                throw new ScriptException(ScriptError.PRIMITIVE_NOT_FOUND);
                            }
                            break;
                    }
                    Simulator simulator;
                    lock (Locks.ClientInstanceNetworkLock)
                    {
                        simulator = Client.Network.Simulators.AsParallel()
                            .FirstOrDefault(o => o.Handle.Equals(primitive.RegionHandle));
                    }
                    if (simulator == null)
                        throw new ScriptException(ScriptError.REGION_NOT_FOUND);
                    var materialFieldInfo = typeof (Material).GetFields(BindingFlags.Public |
                                                                        BindingFlags.Static)
                        .AsParallel().FirstOrDefault(
                            o =>
                                string.Equals(o.Name, wasInput(KeyValue.Get(
                                    wasOutput(Reflection.GetNameFromEnumValue(ScriptKeys.MATERIAL)),
                                    corradeCommandParameters.Message)),
                                    StringComparison.OrdinalIgnoreCase));
                    if (materialFieldInfo == null)
                        throw new ScriptException(ScriptError.UNKNOWN_MATERIAL_TYPE);
                    lock (Locks.ClientInstanceObjectsLock)
                    {
                        Client.Objects.SetMaterial(simulator,
                            primitive.LocalID, (Material) materialFieldInfo.GetValue(null));
                    }
                };
        }
    }
}