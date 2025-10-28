namespace EVChargingStation.CARC.Domain.LongLQ.Enums
{

    /// <summary>
    // Các chuẩn sạc (connector type)
    //	    - CCS: Combined Charging System
    //	    - CHAdeMO: "CHArge de MOve" (a play on words: “charge for moving”).
    //	    - AC: Alternating Current
    /// </summary>
    public enum ConnectorType
    {
        CCS = 0,
        CHAdeMO = 1,
        AC = 2,
        Unknown = 3,
    }
}
