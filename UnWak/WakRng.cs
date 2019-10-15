namespace UnWak
{
    public class WakRng
    {
        private double state;

        public WakRng(double seed)
        {
            state = seed;
            if (state >= 2147483647.0)
            {
                state *= 0.5;
            }
            Next();
        }

        public double Seed { set { state = value; } }

        public double Next()
        {
            int value = (int)state;
            uint temp = (uint)((int)(value + (uint)((ulong)(-2092037281L * value) >> 32)) >> 16);
            value = (int)(16807 * value - 0x7FFFFFFF * (temp + (temp >> 31)));
            if (value <= 0) value += 0x7FFFFFFF;
            state = value;
            return state * 4.656612875e-10;
        }
    }
}
