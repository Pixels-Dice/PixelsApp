﻿using System.Runtime.InteropServices;

namespace Systemic.Unity.Pixels.Animations
{
    /// <summary>
    /// Procedural on off animation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [System.Serializable]
    public class AnimationSimple
        : IAnimationPreset
    {
        public AnimationType type { get; set; } = AnimationType.Simple;
        public byte padding_type { get; set; }
        public ushort duration { get; set; }
        public uint faceMask;
        public ushort colorIndex;
        public byte count;
        public byte fade;

        public AnimationInstance CreateInstance(DataSet.AnimationBits bits)
        {
            return new AnimationInstanceSimple(this, bits);
        }
    };

    /// <summary>
    /// Procedural on off animation instance data
    /// </summary>
    public class AnimationInstanceSimple
        : AnimationInstance
    {
        uint rgb = 0;

        public AnimationInstanceSimple(IAnimationPreset animation, DataSet.AnimationBits bits)
            : base(animation, bits)
        {
        }

        public override void start(int _startTime, byte _remapFace, bool _loop)
        {
            base.start(_startTime, _remapFace, _loop);
            var preset = getPreset();
            rgb = animationBits.getColor32(preset.colorIndex);
        }

        public override int updateLEDs(int ms, int[] retIndices, uint[] retColors)
        {
            var preset = getPreset();

            // Compute color
            uint black = 0;
            uint color = 0;
            int period = preset.duration / preset.count;
            int fadeTime = period * preset.fade / (255 * 2);
            int onOffTime = (period - fadeTime * 2) / 2;
            int time = (ms - startTime) % period;

            if (time <= fadeTime)
            {
                // Ramp up
                color = ColorUIntUtils.InterpolateColors(black, 0, rgb, fadeTime, time);
            }
            else if (time <= fadeTime + onOffTime)
            {
                color = rgb;
            }
            else if (time <= fadeTime * 2 + onOffTime)
            {
                // Ramp down
                color = ColorUIntUtils.InterpolateColors(rgb, fadeTime + onOffTime, black, fadeTime * 2 + onOffTime, time);
            }
            else
            {
                color = black;
            }

            // Fill the indices and colors for the anim controller to know how to update leds
            int retCount = 0;
            for (int i = 0; i < Constants.MaxLedsCount; ++i)
            {
                if ((preset.faceMask & (1 << i)) != 0)
                {
                    retIndices[retCount] = i;
                    retColors[retCount] = color;
                    retCount++;
                }
            }
            return retCount;
        }

        public override int stop(int[] retIndices)
        {
            var preset = getPreset();
            int retCount = 0;
            for (int i = 0; i < Constants.MaxLedsCount; ++i)
            {
                if ((preset.faceMask & (1 << i)) != 0)
                {
                    retIndices[retCount] = i;
                    retCount++;
                }
            }
            return retCount;
        }

        public AnimationSimple getPreset()
        {
            return (AnimationSimple)animationPreset;
        }
    };
}
