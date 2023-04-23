using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCDataSync.Crypto
{
    public class MersenneTwister
    {
        private const int N = 624;
        private const int M = 397;
        private const uint MatrixA = 0x9908B0DF;
        private const uint UpperMask = 0x80000000;
        private const uint LowerMask = 0x7FFFFFFF;

        private readonly uint[] _mt;
        private int _mti;

        public MersenneTwister() : this((uint)DateTime.Now.Ticks)
        {
        }
        public MersenneTwister(uint seed)
        {
            _mt = new uint[N];
            _mt[0] = seed;

            for (_mti = 1; _mti < N; _mti++)
            {
                _mt[_mti] = (uint)(1812433253 * (_mt[_mti - 1] ^ _mt[_mti - 1] >> 30) + _mti);
            }

            _mti = 0;
        }

        public uint Next()
        {
            if (_mti >= N)
            {
                GenerateNumbers();
            }

            var y = _mt[_mti++];
            y ^= y >> 11;
            y ^= y << 7 & 0x9d2c5680;
            y ^= y << 15 & 0xefc60000;
            y ^= y >> 18;

            return y;
        }

        private void GenerateNumbers()
        {
            for (var i = 0; i < N - M; i++)
            {
                var y = _mt[i] & UpperMask | _mt[i + 1] & LowerMask;
                _mt[i] = _mt[i + M] ^ y >> 1 ^ (y & 1) * MatrixA;
            }

            for (var i = N - M; i < N - 1; i++)
            {
                var y = _mt[i] & UpperMask | _mt[i + 1] & LowerMask;
                _mt[i] = _mt[i + (M - N)] ^ y >> 1 ^ (y & 1) * MatrixA;
            }

            var last = _mt[N - 1] & UpperMask | _mt[0] & LowerMask;
            _mt[N - 1] = _mt[M - 1] ^ last >> 1 ^ (last & 1) * MatrixA;

            _mti = 0;
        }
    }
}
