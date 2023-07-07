namespace BrawlhallaANMReader.Swz;
public class PRNG
{
    private readonly uint[] _state = new uint[17];

    public PRNG(uint seed)
    {
        _state[0] = seed;
        _state[16] = 0;

        for (uint i = 1; i < 16; i++)
        {
            uint prev = _state[i - 1];
            uint mod = prev ^ (prev >> 30);
            uint res = mod * 0x6C078965;
            _state[i] = i + res;
        }
    }

    public uint NextRandom()
    {
        uint randomOffset = _state[16];
        uint shiftOne = _state[(randomOffset - 3) & 0xF];
        uint shiftTwo = _state[randomOffset] ^ shiftOne ^ ((shiftOne ^ (_state[randomOffset] << 1)) << 15);
        uint shiftThree = (_state[(randomOffset - 7) & 0xF] >> 11) ^ _state[(randomOffset - 7) & 0xF];
        uint shiftIndex = (_state[16] - 1) & 0xF;

        _state[randomOffset] = shiftThree ^ shiftTwo;
        _state[16] = shiftIndex;
        _state[shiftIndex] ^= shiftTwo ^ shiftThree ^ shiftTwo ^ (32 * ((shiftThree ^ shiftTwo) & 0xFED22169)) ^ (4 * (_state[shiftIndex] ^ ((shiftTwo ^ (shiftThree << 10)) << 16)));

        return _state[_state[16]];
    }
}
