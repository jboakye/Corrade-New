﻿///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using String = wasSharp.String;
using System.Xml.Serialization;
using wasSharp;

namespace Corrade.Structures
{
    /// <summary>
    ///     Agent Structure.
    /// </summary>
    [XmlRoot(ElementName = "Agent")]
    public struct Agent : IEquatable<Agent>
    {
        [XmlElement(ElementName = "FirstName")]
        public string FirstName { get; set; }

        [XmlElement(ElementName = "LastName")]
        public string LastName { get; set; }

        [XmlElement(ElementName = "UUID")]
        public string UUID { get; set; }

        public bool Equals(Agent other)
        {
            return (String.Equals(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase)
                    && String.Equals(LastName, other.LastName, StringComparison.OrdinalIgnoreCase)) ||
                   UUID.Equals(other.UUID);
        }

        public override int GetHashCode()
        {
            return NetHash.Init.Hash(UUID);
        }
    }
}