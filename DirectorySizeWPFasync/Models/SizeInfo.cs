using System;

namespace DirectorySizeWPFasync.Models
{
    internal struct SizeInfo
    {
        public enum SizeUnit { BYTE, KBYTE, MBYTE, GBYTE }

        private const short SizeMultiplier = 1024;

        private const long KbSize = SizeMultiplier;
        private const long MbSize = SizeMultiplier * KbSize;
        private const long GbSize = SizeMultiplier * MbSize;

        private const double KbSizeDivisor = SizeMultiplier;
        private const double MbSizeDivisor = KbSizeDivisor * SizeMultiplier;
        private const double GbSizeDivisor = MbSizeDivisor * SizeMultiplier;

        public double Size { get; }
        public SizeUnit Unit { get; }

        public SizeInfo(long bytesSize)
        {
            if (bytesSize >= GbSize)
            {
                Size = Math.Round(bytesSize / GbSizeDivisor, 2);
                Unit = SizeUnit.GBYTE;
            }
            else if (bytesSize >= MbSize)
            {
                Size = Math.Round(bytesSize / MbSizeDivisor, 1);
                Unit = SizeUnit.MBYTE;
            }
            else if (bytesSize >= KbSize)
            {
                Size = Math.Round(bytesSize / KbSizeDivisor, 0);
                Unit = SizeUnit.KBYTE;
            }
            else
            {
                Size = bytesSize;
                Unit = SizeUnit.BYTE;
            }
        }

        public override string ToString()
        {
            return $"{Size.ToString()} {GetUnitName()}";
        }

        private string GetUnitName()
        {
            switch (Unit)
            {
                case SizeUnit.BYTE:
                    return "byte";
                case SizeUnit.KBYTE:
                    return "Kb";
                case SizeUnit.MBYTE:
                    return "Mb";
                case SizeUnit.GBYTE:
                    return "Gb";
                default:
                    return string.Empty;
            }
        }
    }
}