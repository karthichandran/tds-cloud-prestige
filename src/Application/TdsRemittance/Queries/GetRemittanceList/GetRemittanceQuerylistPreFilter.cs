using ReProServices.Domain.Enums;
using System;
using System.Linq;

namespace ReProServices.Application.TdsRemittance.Queries.GetRemittanceList
{
    public static class GetRemittanceQueryListPreFilter
    {
        public static IQueryable<TdsRemittanceDto> PreFilterRemittanceBy(this IQueryable<TdsRemittanceDto> remittances,
            TdsRemittanceFilter filter)
        {
            IQueryable<TdsRemittanceDto> remittanceList = remittances.AsQueryable();

            if (!string.IsNullOrEmpty( filter.UnitNo))
            {
                remittanceList = remittanceList.Where(_ => _.UnitNo == filter.UnitNo);
            }
            if (!string.IsNullOrEmpty( filter.FromUnitNo) && !string.IsNullOrEmpty(filter.ToUnitNo ))
            {
               // remittanceList = remittanceList.Where(_ =>  string.Compare( _.UnitNo.Trim(),filter.FromUnitNo) >=0  && string.Compare(_.UnitNo.Trim(), filter.ToUnitNo) <= 0).OrderBy(o=>o.UnitNo);
              //  remittanceList = remittanceList.Where(_ =>  string.Compare( _.UnitNo.Trim(),filter.FromUnitNo) >=0  &&  _.UnitNo.Length>= filter.FromUnitNo.Length &&  string.Compare(_.UnitNo.Trim(), filter.ToUnitNo) <= 0 && _.UnitNo.Length <= filter.ToUnitNo.Length).OrderBy(o=>o.UnitNo);
            }
            if (filter.LotNo > 0)
            {
                remittanceList = remittanceList.Where(_ => _.LotNo == filter.LotNo);
            }

            if (filter.RemittanceStatusID.HasValue)
            {
                if (filter.RemittanceStatusID.Value == (int)ERemittanceStatus.ExcludeOnlyTDSpayments) {
                    remittanceList = remittanceList
                               .Where(_ => _.OnlyTDS==false);
                }
                else
                remittanceList = remittanceList
                                .Where(_ => _.RemittanceStatusID == filter.RemittanceStatusID.Value);
            }

            if (filter.TdsAmount > 0)
            {
                remittanceList = remittanceList.Where(_ => _.TdsAmount == filter.TdsAmount);
            }

            return remittanceList;
        }
    }
}
