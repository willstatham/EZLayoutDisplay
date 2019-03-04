﻿using System;
using System.Collections.Generic;
using System.Linq;
using InvvardDev.EZLayoutDisplay.Desktop.Model;
using InvvardDev.EZLayoutDisplay.Desktop.Model.Dictionary;
using InvvardDev.EZLayoutDisplay.Desktop.Model.Enum;

namespace InvvardDev.EZLayoutDisplay.Desktop.Helper
{
    public class EZLayoutMaker
    {
        private const string NoCommand = "KC_NO";
        private const string KeyCodeOSM = "OSM";
        private readonly KeyCategoryDictionary _keyCategoryDictionary;
        private readonly KeyDefinitionDictionary _keyDefinitionDictionary;

        public EZLayoutMaker()
        {
            _keyCategoryDictionary = new KeyCategoryDictionary();
            _keyDefinitionDictionary = new KeyDefinitionDictionary();
        }

        public EZLayout PrepareEZLayout(ErgodoxLayout ergodoxLayout)
        {
            var ezLayout = new EZLayout {
                                            HashId = ergodoxLayout.HashId,
                                            Name = ergodoxLayout.Title
                                        };

            foreach (var ergodoxLayer in ergodoxLayout.Revisions.First().Layers)
            {
                EZLayer ezLayer = PrepareEZLayer(ergodoxLayer);
                ezLayout.EZLayers.Add(ezLayer);
            }

            return ezLayout;
        }

        private EZLayer PrepareEZLayer(ErgodoxLayer ergodoxLayer)
        {
            var layer = new EZLayer {
                                        Index = ergodoxLayer.Position,
                                        Name = ergodoxLayer.Title,
                                        Color = ergodoxLayer.Color
                                    };

            for (var index = 0 ; index < ergodoxLayer.Keys.Count ; index++)
            {
                EZKey key = PrepareKeyLabels(ergodoxLayer.Keys[index], index);

                layer.EZKeys.Add(key);
            }

            return layer;
        }

        private EZKey PrepareKeyLabels(ErgodoxKey ergodoxKey, int keyIndex)
        {
            KeyDefinition keyDefinition = GetKeyDefinition(ergodoxKey.Code);

            /** Every category has a label, so no need to make a special case :
             *
             * KeyCategory.Autoshift
             * KeyCategory.Digit
             * KeyCategory.Letters
             * KeyCategory.Fn
             * KeyCategory.Fw
             * KeyCategory.Lang
             * KeyCategory.Numpad
             * KeyCategory.Other
             * KeyCategory.Punct
             * KeyCategory.ShiftedPunct
             * KeyCategory.System
             *
             **/
            EZKey key = new EZKey {
                                      KeyCategory = keyDefinition.KeyCategory,
                                      Label = new KeyLabel(keyDefinition.Label, keyDefinition.IsGlyph),
                                      Color = ergodoxKey.GlowColor,
                                      DisplayType = KeyDisplayType.SimpleLabel
                                  };

            switch (keyDefinition.KeyCategory)
            {
                case KeyCategory.DualFunction:

                    if (AddCommandLabel(ergodoxKey, key))
                    {
                        key.DisplayType = KeyDisplayType.ModifierUnder;
                    }
                    else
                    {
                        key.KeyCategory = KeyCategory.Modifier;
                    }

                    break;
                case KeyCategory.Layer:
                case KeyCategory.LayerShortcuts:
                    key.Label.Content = string.Format(key.Label.Content, ergodoxKey.Layer.ToString());

                    if (AddCommandLabel(ergodoxKey, key))
                    {
                        key.DisplayType = KeyDisplayType.ModifierUnder;
                    }

                    break;
                case KeyCategory.Modifier:

                    if (ergodoxKey.Code == KeyCodeOSM && !IsCommandEmpty(ergodoxKey.Command))
                    {
                        var commandDefinition = GetKeyDefinition(ergodoxKey.Command);
                        key.Modifier = new KeyLabel(commandDefinition.Label);
                        key.DisplayType = KeyDisplayType.ModifierOnTop;
                    }

                    break;
                case KeyCategory.Media:
                case KeyCategory.Mouse:
                case KeyCategory.Nav:
                case KeyCategory.Spacing:
                case KeyCategory.Shine:
                    key.DisplayType = KeyDisplayType.SimpleLabel;

                    break;
                case KeyCategory.Shortcuts:

                    if (!IsCommandEmpty(ergodoxKey.Command))
                    {
                        var commandDefinition = GetKeyDefinition(ergodoxKey.Command);
                        key.Label.Content = $"{key.Label.Content} + {commandDefinition.Label}";
                    }

                    break;
                case KeyCategory.French:
                    key.InternationalHint = "fr";

                    break;
                case KeyCategory.German:
                    key.InternationalHint = "de";

                    break;
                case KeyCategory.Spanish:
                    key.InternationalHint = "es";

                    break;
            }

            ProcessModifiers(ergodoxKey, key);

            return key;
        }

        private KeyDefinition GetKeyDefinition(string ergodoxKeyCode)
        {
            var keyDefinition = _keyDefinitionDictionary.KeyDefinitions.FirstOrDefault(k => k.KeyCode == ergodoxKeyCode) ?? GetKeyDefinition(NoCommand);

            return keyDefinition;
        }

        /// <summary>
        /// Apply the command label.
        /// </summary>
        /// <param name="ergodoxKey">The <see cref="ErgodoxKey"/> containing the command to be applied.</param>
        /// <param name="key">The <see cref="EZKey"/> to apply the command to.</param>
        /// <returns><c>True</c> if command has been applied.</returns>
        private bool AddCommandLabel(ErgodoxKey ergodoxKey, EZKey key)
        {
            if (IsCommandEmpty(ergodoxKey.Command)) return false;

            var commandDefinition = GetKeyDefinition(ergodoxKey.Command);
            key.Modifier = key.Label;
            key.Label = new KeyLabel(commandDefinition.Label, commandDefinition.IsGlyph);

            return true;
        }

        private void ProcessModifiers(ErgodoxKey ergodoxKey, EZKey key)
        {
            if (ergodoxKey.Modifiers == null) return;

            var mods = GetModifiersApplied(ergodoxKey.Modifiers);

            if (!mods.Any()) return;

            key.Modifier = new KeyLabel(AggregateModifierLabels(mods));
            key.DisplayType = KeyDisplayType.ModifierOnTop;
        }

        private List<EZModifier> GetModifiersApplied(ErgodoxModifiers ergodoxModifiers)
        {
            var keyModifiers = new KeyModifierDictionary();
            var mods = new List<EZModifier>();

            if (ergodoxModifiers.LeftAlt)
            {
                mods.Add(keyModifiers.EZModifiers.First(m => m.KeyModifier == KeyModifier.LeftAlt));
            }

            if (ergodoxModifiers.LeftCtrl)
            {
                mods.Add(keyModifiers.EZModifiers.First(m => m.KeyModifier == KeyModifier.LeftCtrl));
            }

            if (ergodoxModifiers.LeftShift)
            {
                mods.Add(keyModifiers.EZModifiers.First(m => m.KeyModifier == KeyModifier.LeftShift));
            }

            if (ergodoxModifiers.LeftWin)
            {
                mods.Add(keyModifiers.EZModifiers.First(m => m.KeyModifier == KeyModifier.LeftWin));
            }

            if (ergodoxModifiers.RightAlt)
            {
                mods.Add(keyModifiers.EZModifiers.First(m => m.KeyModifier == KeyModifier.RightAlt));
            }

            if (ergodoxModifiers.RightCtrl)
            {
                mods.Add(keyModifiers.EZModifiers.First(m => m.KeyModifier == KeyModifier.RightCtrl));
            }

            if (ergodoxModifiers.RightShift)
            {
                mods.Add(keyModifiers.EZModifiers.First(m => m.KeyModifier == KeyModifier.RightShift));
            }

            if (ergodoxModifiers.RightWin)
            {
                mods.Add(keyModifiers.EZModifiers.First(m => m.KeyModifier == KeyModifier.RightWin));
            }

            return mods.OrderBy(m => m.Index).ToList();
        }

        private string AggregateModifierLabels(List<EZModifier> mods)
        {
            var subLabel = "";

            switch (mods.Count)
            {
                case 1:
                    subLabel = mods.First().Labels[EZModifier.LabelSize.Large];

                    break;
                case 2:
                    subLabel = mods.Select(m => m.Labels[EZModifier.LabelSize.Medium]).Aggregate((seed, inc) => $"{seed}+{inc}");

                    break;
                default:
                    subLabel = mods.Select(m => m.Labels[EZModifier.LabelSize.Small]).Aggregate((seed, inc) => $"{seed}+{inc}");

                    break;
            }

            return subLabel;
        }

        private bool IsCommandEmpty(string command)
        {
            var isEmpty = string.IsNullOrWhiteSpace(command) || command == NoCommand || command == KeyCodeOSM;

            return isEmpty;
        }
    }
}