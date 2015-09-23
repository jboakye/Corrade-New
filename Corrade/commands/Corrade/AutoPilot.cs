///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenMetaverse;
using Parallel = System.Threading.Tasks.Parallel;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class CorradeCommands
        {
            public static Action<CorradeCommandParameters, Dictionary<string, string>> autopilot =
                (corradeCommandParameters, result) =>
                {
                    if (
                        !HasCorradePermission(corradeCommandParameters.Group.Name,
                            (int) Permissions.Movement))
                    {
                        throw new ScriptException(ScriptError.NO_CORRADE_PERMISSIONS);
                    }
                    switch (
                        wasGetEnumValueFromDescription<Action>(
                            wasInput(
                                wasKeyValueGet(wasOutput(wasGetDescriptionFromEnumValue(ScriptKeys.ACTION)),
                                    corradeCommandParameters.Message)).ToLowerInvariant()))
                    {
                        case Action.START:
                            Vector3 position;
                            if (
                                !Vector3.TryParse(
                                    wasInput(wasKeyValueGet(
                                        wasOutput(wasGetDescriptionFromEnumValue(ScriptKeys.POSITION)),
                                        corradeCommandParameters.Message)),
                                    out position))
                            {
                                throw new ScriptException(ScriptError.INVALID_POSITION);
                            }
                            uint moveRegionX, moveRegionY;
                            Utils.LongToUInts(Client.Network.CurrentSim.Handle, out moveRegionX, out moveRegionY);
                            if (Client.Self.Movement.SitOnGround || !Client.Self.SittingOn.Equals(0))
                            {
                                Client.Self.Stand();
                            }
                            // stop all non-built-in animations
                            List<UUID> lindenAnimations = new List<UUID>(typeof (Animations).GetProperties(
                                BindingFlags.Public |
                                BindingFlags.Static).AsParallel().Select(o => (UUID) o.GetValue(null)).ToList());
                            Parallel.ForEach(Client.Self.SignaledAnimations.Copy().Keys, o =>
                            {
                                if (!lindenAnimations.Contains(o))
                                    Client.Self.AnimationStop(o, true);
                            });
                            Client.Self.AutoPilotCancel();
                            Client.Self.Movement.TurnToward(position, true);
                            Client.Self.AutoPilot(position.X + moveRegionX, position.Y + moveRegionY, position.Z);
                            break;
                        case Action.STOP:
                            Client.Self.AutoPilotCancel();
                            break;
                        default:
                            throw new ScriptException(ScriptError.UNKNOWN_MOVE_ACTION);
                    }
                    // Set the camera on the avatar.
                    Client.Self.Movement.Camera.LookAt(
                        Client.Self.SimPosition,
                        Client.Self.SimPosition
                        );
                };
        }
    }
}