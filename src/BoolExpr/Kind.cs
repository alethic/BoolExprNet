namespace BoolExprNet
{

    public enum Kind : byte
    {

        ZERO = 0x00,
        ONE = 0x01,
        LOG = 0x04,
        ILL = 0x06,
        COMP = 0x08,
        VAR = 0x09,
        NOR = 0x10,
        OR = 0x11,
        NAND = 0x12,
        AND = 0x13,
        XNOR = 0x14,
        XOR = 0x15,
        NEQ = 0x16,
        EQ = 0x17,
        NIMPL = 0x18,
        IMPL = 0x19,
        NITE = 0x1A,
        ITE = 0x1B,

    }

}