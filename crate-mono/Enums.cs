namespace Crate
{
    public enum OrderDirection : byte
    {
        Asc = 1,
        Desc = 2
    }

    public enum ExtendedTypeCode : byte
    {
        Empty = 0, 
        Object = 1,
        DBNull = 2,
        Boolean = 3,
        Char = 4,
        SByte = 5,
        Byte = 6,
        Int16 = 7,
        UInt16 = 8,
        Int32 = 9,
        UInt32 = 10,
        Int64 = 11,
        UInt64 = 12,
        Single = 13,
        Double = 14,
        Decimal = 15,
        DateTime = 16,
        String = 18,
        Guid=19,
        Float=20,
        JObject=101
    }

    public enum IndexType : byte
    {
        None = 0,
        Plain = 1,
        FullText = 2,
        FullTextWithEnglishAnalyzer = 3
    }
    
    public enum ReplicationType : short
    {
        None=-99,
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        All = -1,
    }
    
}
