namespace BoolExprNet
{

    public enum Kind : byte
    {

        Zero = 0x00,
        One = 0x01,
        Logical = 0x04,
        Illogical = 0x06,
        Complement = 0x08,
        Variable = 0x09,
        NotOr = 0x10,
        Or = 0x11,
        NotAnd = 0x12,
        And = 0x13,
        ExclusiveNotOr = 0x14,
        ExclusiveOr = 0x15,
        NotEqual = 0x16,
        Equal = 0x17,
        NotImplies = 0x18,
        Implies = 0x19,
        NotIfThenElse = 0x1A,
        IfThenElse = 0x1B,

    }

}