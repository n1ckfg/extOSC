﻿using UnityEditor;

using UnityEngine;

using System;

using extOSC.Core;


namespace extOSC.Editor.Drawers
{
	[CustomPropertyDrawer(typeof(OSCSelectorAttribute))]
	public class OSCSelectorDrawer : PropertyDrawer
	{
		#region Static Private Vars

		private static Type _transmitterType = typeof(OSCTransmitter);

		private static Type _receiverType = typeof(OSCReceiver);

        #endregion

        #region Public Methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.ObjectReference)
			{
				// TODO: Exception.
				return;
			}

			var fieldType = fieldInfo.FieldType;
			if (fieldType == _transmitterType)
			{
				var content = (GUIContent[]) null;
				var objects = (OSCTransmitter[]) null;

				OSCEditorUtils.FindObjectsForPopup(TransmitterCallback, true, out content, out objects);

				property.objectReferenceValue = PopupLayout(position, label,
				                                            (OSCTransmitter) property.objectReferenceValue, content,
				                                            objects);
			}
			else if (fieldType == _receiverType)
			{
				var content = (GUIContent[]) null;
				var objects = (OSCReceiver[]) null;

				OSCEditorUtils.FindObjectsForPopup(ReceiverCallback, true, out content, out objects);

				property.objectReferenceValue =
					PopupLayout(position, label, (OSCReceiver) property.objectReferenceValue, content, objects);
			}
			else
			{
				// TODO: Exception.
			}
		}

		#endregion

		#region Private Methods

		private TOSC PopupLayout<TOSC>(Rect position, GUIContent label, TOSC currentObject, GUIContent[] content,
		                               TOSC[] objects) where TOSC : OSCBase
		{
			return objects[EditorGUI.Popup(position, label, Mathf.Max(objects.IndexOf(currentObject), 0), content)];
		}

		private string TransmitterCallback(OSCTransmitter transmitter)
		{
			return string.Format("Transmitter: {0}:{1}", transmitter.RemoteHost, transmitter.RemotePort);
		}

		private string ReceiverCallback(OSCReceiver receiver)
		{
			return string.Format("Receiver: {0}", receiver.LocalPort);
		}

		#endregion
	}
}
