///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;
using wasOpenMetaverse;
using Inventory = wasOpenMetaverse.Inventory;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class RLVBehaviours
        {
            public static Action<string, RLVRule, UUID> getoutfit = (message, rule, senderUUID) =>
            {
                int channel;
                if (!int.TryParse(rule.Param, out channel) || channel < 1)
                {
                    return;
                }
                var wearables =
                    new HashSet<InventoryBase>(Inventory.GetWearables(Client, CurrentOutfitFolder));
                var response = new StringBuilder();
                switch (!string.IsNullOrEmpty(rule.Option))
                {
                    case true:
                        var RLVwearable = RLVWearables.AsParallel()
                            .FirstOrDefault(
                                o => string.Equals(rule.Option, o.Name, StringComparison.InvariantCultureIgnoreCase));
                        switch (!RLVwearable.Equals(default(RLVWearable)))
                        {
                            case true:
                                if (
                                    wearables.AsParallel()
                                        .Any(o => (o as InventoryWearable).WearableType.Equals(RLVwearable.WearableType)))
                                {
                                    response.Append(RLV_CONSTANTS.TRUE_MARKER);
                                    break;
                                }
                                goto default;
                            default:
                                response.Append(RLV_CONSTANTS.FALSE_MARKER);
                                break;
                        }
                        break;
                    default:
                        var data = new string[RLVWearables.Count];
                        Enumerable.Range(0, RLVWearables.Count).ToArray().AsParallel().ForAll(o =>
                        {
                            if (
                                !wearables.AsParallel()
                                    .Any(
                                        p =>
                                            p is InventoryWearable &&
                                            (p as InventoryWearable).WearableType.Equals(RLVWearables[o].WearableType)))
                            {
                                data[o] = RLV_CONSTANTS.FALSE_MARKER;
                                return;
                            }
                            data[o] = RLV_CONSTANTS.TRUE_MARKER;
                        });
                        response.Append(string.Join("", data.ToArray()));
                        break;
                }
                lock (Locks.ClientInstanceSelfLock)
                {
                    Client.Self.Chat(response.ToString(), channel, ChatType.Normal);
                }
            };
        }
    }
}