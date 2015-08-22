using System;

namespace FantasySimulator.Simulator.Soccer
{
    public struct TransferDetails
    {
        public int TransfersIn { get; set; }

        public int TransfersOut { get; set; }

        public int TransfersDiff => TransfersIn - TransfersOut;

        public int TotalTransfers => TransfersIn + TransfersOut;

        public double TransferTrend
        {
            get
            {
                var res = (double) Math.Abs(TransfersIn) / TotalTransfers;
                if (TransfersDiff < 0)
                    res--;
                return res;
            }
        }

        public override string ToString()
        {
            return string.Format("Trend: {0:P2}", TransferTrend);
        }
    }
}
