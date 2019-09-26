﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Net;
using System.Collections.Generic;

using extOSC.Core;

namespace extOSC
{
    public class OSCMessage : IOSCPacket
    {
        #region Static Public Methods

        public static OSCMessage Create(string address, params OSCValue[] values)
        {
            return new OSCMessage(address, values);
        }

        #endregion

        #region Public Vars

		public string Address { get; set; }

		public IPAddress Ip { get; set; }

		public int Port { get; set; }

		public List<OSCValue> Values { get; } = new List<OSCValue>();

        #endregion

		#region Public Methods

        public OSCMessage(string address)
		{
			Address = address;
		}

        public OSCMessage(string address, params OSCValue[] values)
        {
            Address = address;
			AddRange(values);
        }

		public void AddValue(OSCValue value)
		{
			if (value == null)
				throw new NullReferenceException(nameof(value));

			Values.Add(value);
		}

		public void AddRange(IEnumerable<OSCValue> values)
		{
			if (values == null)
				throw new NullReferenceException(nameof(values));

			Values.AddRange(values);
		}

		public OSCValue[] FindValues(params OSCValueType[] types)
        {
            var tempValues = new List<OSCValue>();

            foreach (var value in Values)
            {
                foreach (var type in types)
                {
                    if (value.Type == type)
                    {
                        tempValues.Add(value);
                    }
                }
            }

            return tempValues.ToArray();
        }

		public bool IsBundle() => false;

		public IOSCPacket Copy()
		{
			var valuesCount = Values.Count;
			var values = new OSCValue[valuesCount];

			for (var i = 0; i < valuesCount; ++i)
			{
				values[i] = Values[i].Copy();
			}

			return new OSCMessage(Address, values);
		}

        public override string ToString()
        {
            var stringValues = string.Empty;

            if (Values.Count > 0)
            {
                foreach (var value in Values)
                {
                    stringValues += $"{value.GetType().Name}({value.Type}) : \"{value.Value}\", ";
                }

                stringValues = $"({stringValues.Remove(stringValues.Length - 2)})";
            }

            return $"<{GetType().Name}:\"{Address}\"> : {(string.IsNullOrEmpty(stringValues) ? "null" : stringValues)}";
        }




		// OBSOLETE
        [Obsolete("Use FindValues method.")]
        public OSCValue[] GetValues(params OSCValueType[] types)
		{
			return FindValues(types);
		}
		
        [Obsolete] // TODO: Move in Utils.
		public OSCValueType[] GetTypes()
		{
			var types = new OSCValueType[Values.Count];

			for (var i = 0; i < Values.Count; i++)
			{
				types[i] = Values[i].Type;
			}

			return types;
		}

		[Obsolete] // TODO: Move in Editor Utils.
		public string GetTags()
		{
			var tags = string.Empty;

			foreach (var value in Values)
			{
				tags += value.Tag;
			}

			return tags;
		}


        #endregion
    }
}