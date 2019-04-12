﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Linq;

namespace extOSC.Editor
{
    public static partial class OSCEditorLayout
    {
        #region Static Private Vars

        private static readonly GUIContent _transmitterAddressContent = new GUIContent("OSC Transmitter Address:");

        private static readonly GUIContent _transmitterAddressContentSmall = new GUIContent("OSC Address:");

        private static readonly GUIContent _receiverAddressContent = new GUIContent("OSC Receiver Address:");

        private static readonly GUIContent _receiverAddressContentSmall = new GUIContent("OSC Address:");

        private static readonly GUIContent _transmitterContent = new GUIContent("OSC Transmitter:");

        private static readonly GUIContent _receiverContent = new GUIContent("OSC Receiver:");

        private static readonly GUIContent _emptyContent = new GUIContent("- Empty -");

        #endregion

        #region Static Public Methods

        public static void DrawLogo()
        {
            if (OSCEditorTextures.IronWall != null)
            {
                EditorGUILayout.Space();

                var rect = GUILayoutUtility.GetRect(0, 0);
                var width = OSCEditorTextures.IronWall.width * 0.2f;
                var height = OSCEditorTextures.IronWall.height * 0.2f;

                rect.x = rect.width * 0.5f - width * 0.5f;
                rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
                rect.width = width;
                rect.height = height;

                GUI.DrawTexture(rect, OSCEditorTextures.IronWall);
                EditorGUILayout.Space();
            }
        }

        public static void DrawProperties(SerializedObject serializedObject, params string[] exceptionsNames)
        {
            var serializedProperty = serializedObject.GetIterator();
            var isEmpty = true;
            var enterChild = true;

            while (serializedProperty.NextVisible(enterChild))
            {
                enterChild = false;

                if (serializedProperty.name == "m_Script" ||
                    exceptionsNames.Contains(serializedProperty.name))
                    continue;

                EditorGUILayout.PropertyField(serializedProperty, true);

                isEmpty = false;
            }

            if (isEmpty)
                EditorGUILayout.LabelField(_emptyContent, OSCEditorStyles.CenterLabel);
        }

        public static void TransmitterSettings(SerializedProperty property, SerializedProperty addressProperty, bool drawBox = true)
        {
	        if (drawBox) GUILayout.BeginVertical(OSCEditorStyles.Box);
            EditorGUILayout.PropertyField(addressProperty, EditorGUIUtility.currentViewWidth > 410 ?
                                          _transmitterAddressContent : _transmitterAddressContentSmall);

			//TransmittersPopup(transmitterProperty, _transmitterContent);
			EditorGUILayout.PropertyField(property, _transmitterContent);

			if (drawBox) EditorGUILayout.EndVertical();
        }

        public static void ReceiverSettings(SerializedProperty property, SerializedProperty addressProperty, bool drawBox = true)
        {
			if (drawBox) GUILayout.BeginVertical(OSCEditorStyles.Box);

            EditorGUILayout.PropertyField(addressProperty, EditorGUIUtility.currentViewWidth > 380 ?
                                          _receiverAddressContent : _receiverAddressContentSmall);

            //ReceiversPopup(property, _receiverContent);
	        EditorGUILayout.PropertyField(property, _receiverContent);

			if (drawBox) EditorGUILayout.EndVertical();
        }

        #endregion
    }
}